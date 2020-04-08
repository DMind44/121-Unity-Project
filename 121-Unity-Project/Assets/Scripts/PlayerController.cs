using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

// This script moves the player and camera, focusing on keyboard and mouse input
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float movementSpeed = 0f;
    [SerializeField]
    private float rotationSpeed = 0f;
    [SerializeField]
    private float jumpSpeed = 0f;
    public float gravity = 0f;

    [SerializeField]
    private float maxVelocityChange = 0f;

    private bool isGrounded = false;

    private Rigidbody rb;
    private PlayerThrow myThrow;

    [SerializeField]
    private Camera cam = null;

    // private GameStateController gameState = null;

    public bool canMove = true;

    // @TODO: Unserialize this field once testing on it is done
    [SerializeField]
    public float hp = 0;
    [SerializeField]
    public float max_hp = 0;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        myThrow = GetComponent<PlayerThrow>();
        rb.useGravity = false;  // We'll control gravity ourselves
        hp = max_hp;

    }

    /*
     * Physics-based movement
     */
    // // Perform physics updates at regular time intervals
    // void FixedUpdate() {
    //     // Translate player based on direction key input
    //     float horizontalMovementInput = Input.GetAxis("Horizontal");
    //     float verticalMovementInput = Input.GetAxis("Vertical");
    //
    //     Vector3 movement = new Vector3(horizontalMovementInput, 0, verticalMovementInput);
    //     rb.AddRelativeForce(movement * movementSpeed);
    //
    //     // Rotate player based on mouse
    //     transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
    //
    //     if (Input.GetKeyDown(KeyCode.Space)) {
    //         DamageMe(1);
    //     }
    // }

    void Update() {
        // Press h to die - for debugging purposes
        if (Input.GetButtonDown("Hurt")) {
            DamageMe(1);
        }

        // kill the player if their health drops below 0
        if (hp <= 0) {
            kill();
        }
    }

    // Run once per frame to move in response to user input.
    // Don't move if Player is actively lifting something.
    //   Source: http://wiki.unity3d.com/index.php/RigidbodyFPSWalker?_ga=2.269071159.757207726.1586110776-1944583397.1580664386
    void FixedUpdate() {
        // If we are currently lifting an object, ignore movement and jumping
        if (myThrow == null || myThrow.currentObject == null ||
                myThrow.currentObject.GetComponent<Interactable>() == null ||
                !myThrow.currentObject.GetComponent<Interactable>().lifting || !canMove) {
            // Calculate how fast we should be moving
            Vector3 targetVelocity;
            if (GameState.IsAcceptingMovement) {
                targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= movementSpeed;
            } else {
                targetVelocity = Vector3.zero;
            }

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump only if the Player is grounded and is pressing Jump.
            if (GameState.IsAcceptingMovement && isGrounded && Input.GetButton("Jump")) {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            }

            // Rotate in response to mouse
            if (GameState.IsAcceptingMovement) {
                Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                transform.Rotate(Vector3.up, mouseInput.x * rotationSpeed);
            }
        }

        // Add force from gravity
        rb.AddForce(new Vector3 (0, -gravity * rb.mass, 0));

        isGrounded = false;
    }

    void OnCollisionStay(Collision collisionInfo) {
        isGrounded = true;
        // Debug.Log("Grounded");
    }

    [Client] public void DamageMe(float amount) {
        hp -= amount;
        Debug.Log("health:" + hp);
    }

    void kill() {
        Debug.Log("You're dead!");
    }

    // Return a reference to this player's camera
    public Camera GetCamera() {
        return cam;
    }
}
