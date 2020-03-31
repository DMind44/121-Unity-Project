using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

// This script moves the player and camera, focusing on keyboard and mouse input
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    public float movementSpeed;
    public float rotationSpeed;
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
        hp = max_hp;
        healthBar.setMaxHealth(max_hp);
    }

    // Perform physics updates at regular time intervals
    void FixedUpdate() {
        // Translate player based on direction key input
        float horizontalMovementInput = Input.GetAxis("Horizontal");
        float verticalMovementInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalMovementInput, 0, verticalMovementInput);
        rb.AddRelativeForce(movement * movementSpeed);

        // Rotate player based on mouse
        transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);

        if (Input.GetKeyDown(KeyCode.Space)) {
            DamageMe(1);
        }
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
