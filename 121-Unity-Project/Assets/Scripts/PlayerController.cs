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

    [SerializeField]
    private Camera cam = null;

    // @TODO: Unserialize this field once testing on it is done
    [SerializeField]
    private float hp = 0;
    [SerializeField]
    private float max_hp = 0;
    public HealthBar healthBar;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // We'll control gravity ourselves
        hp = max_hp;
        healthBar.setMaxHealth(max_hp);
    }

    /*
     * Physics-based movement
     */
    // // Perform physics updates at regular time intervals
    // void FixedUpdate() {
    //     // Translate player based on direction key input
    //     float horizontalMovementInput = Input.GetAxis("Horizontal");
    //     float verticalMovementInput = Input.GetAxis("Vertical");

    //     Vector3 movement = new Vector3(horizontalMovementInput, 0, verticalMovementInput);
    //     rb.AddRelativeForce(movement * movementSpeed);

    //     // Rotate player based on mouse
    //     transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);

    //     if (Input.GetKeyDown(KeyCode.Space)) {
    //         DamageMe(1);
    //     }
    // }

    // Run once per frame to move in response to user input
    //   Source: http://wiki.unity3d.com/index.php/RigidbodyFPSWalker?_ga=2.269071159.757207726.1586110776-1944583397.1580664386
    void Update() {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= movementSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        // Debug.Log(velocityChange);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Jump
        if (isGrounded && Input.GetButton("Jump")) {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }

        // Gravity
        rb.AddForce(new Vector3 (0, -gravity * rb.mass, 0));

        // Rotate in response to mouse
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.Rotate(Vector3.up, mouseInput.x * rotationSpeed);

        isGrounded = false;
    }

    void OnCollisionStay(Collision collisionInfo) {
        isGrounded = true;
        // Debug.Log("Grounded");
    }

    [Client] public void DamageMe(float amount) {
        hp -= amount;
        healthBar.setHealth(hp);
        Debug.Log("health:" + hp);
    }

    // Return a reference to this player's camera
    public Camera GetCamera() {
        return cam;
    }
}
