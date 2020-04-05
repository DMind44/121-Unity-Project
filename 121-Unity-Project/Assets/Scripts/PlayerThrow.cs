using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerThrow : NetworkBehaviour
{
    private GameObject currentObject = null;

    [SerializeField]
    private float interactableDistance = 0f;

    private PlayerController playerCont = null;
    private Camera cam = null;

    // Start is called before the first frame update
    void Start()
    {
        playerCont = GetComponent<PlayerController>();
        cam = playerCont.GetCamera();
    }

    // Every frame, update what thing the player is grabbing/throwing
    void Update() {
        Ray ray;
        RaycastHit hit;
        // Check what the camera is pointing at
        ray = cam.ScreenPointToRay(Input.mousePosition);
        // If you are not holding an object and the Raycast hit something...
        if (!currentObject && Physics.Raycast(ray, out hit)) {
            // If we're close enough to the object and it is interactable...
            Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
            if (hit.distance <= interactableDistance && inter != null) {
                // On mouse click, grab this object. Otherwise, start hovering
                if (Input.GetMouseButtonDown(0) && !inter.lifted) {
                    CmdGrab(hit.collider.gameObject);
                    currentObject = hit.collider.gameObject;
                    cam.GetComponent<CameraController>().MoveToPickUpPosition();
                } else {
                    inter.BeginHover();
                }
            }
        } else if (currentObject != null && Input.GetMouseButtonDown(0)) {
            // if you are currently holding an object, click to throw it
            CmdThrow(currentObject);
            currentObject = null;
            cam.GetComponent<CameraController>().MoveToDefaultPosition();
        }
    }

    // Issue a command to the server to have this object get picked up
    [Command] public void CmdGrab(GameObject obj) {
        obj.GetComponent<Interactable>().Grab(transform);
    }

    // Issue a command to the server to have this object get thrown
    [Command] public void CmdThrow(GameObject obj) {
        obj.GetComponent<Interactable>().Throw();
    }
}
