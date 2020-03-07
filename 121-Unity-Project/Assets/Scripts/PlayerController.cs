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
    [SyncVar, SerializeField]
    private float hp = 0;
    [SyncVar, SerializeField]
    private float max_hp = 0;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        hp = max_hp;
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

        // Update camera rotation
        // cam.transform.Rotate(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0);
    }

    // Runs everytime something bumps into this player
    private void OnCollisionEnter(Collision other) {
        Interactable inter = other.gameObject.GetComponent<Interactable>();
        if (inter != null) {
            if (inter.flying) {  // Take damage!
                hp -= inter.Damage();
                // @TODO: Check if need to disable right away
                CmdStopFlying(other.gameObject);
            }
        }
    }

    // Command the server to stop flying this Interactable
    [Command] void CmdStopFlying(GameObject other) {
        Interactable inter = other.GetComponent<Interactable>();
        if (inter != null) {
            inter.StopFlying();
        }
    }

    // Return a reference to this player's camera
    public Camera GetCamera() {
        return cam;
    }
}
