using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    public float movementSpeed;
    public float rotationSpeed;
    private Rigidbody rb;
    public Transform guide;

    [SerializeField]
    private Camera cam = null;

    [SerializeField]
    private float interactableDistance = 0f;

    private bool hasObject = false;
    private Ray ray;
    private RaycastHit hit;
    private Interactable currentObject = null;

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

        // Update camera rotation
        cam.transform.Rotate(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0);
    }

    void Update() {
        // Check what the camera is pointing at
        ray = cam.ScreenPointToRay(Input.mousePosition);
        // if you are currently holding an object, you can't pick up another
        // this assumes that the raycast checking is unnecessary if we are holding an object
        if (!hasObject && Physics.Raycast(ray, out hit)) {
            Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
            // If we're close enough to the object and it is interactable
            if (hit.distance <= interactableDistance && inter != null) {
                // On mouse click, grab this object
                if (Input.GetMouseButtonDown(0)) {
                    inter.Grab(transform);
                    currentObject = inter;
                    hasObject = true;
                } else {
                    inter.BeginHover();
                }
            }
        } else if (currentObject != null && Input.GetMouseButtonDown(0)) {
            // if you are currently holding an object, click to throw it
            currentObject.Throw();
            hasObject = false;
        }
    }
}
