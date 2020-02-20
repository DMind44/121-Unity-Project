using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

public class PlayerController : MonoBehaviour
{

    public float movementSpeed;
    public float rotationSpeed;
    private Rigidbody rb;

    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
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
    }

    void Update() {
        // Check what the camera is pointing at
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            print (hit.collider.name);
        }
    }
}
