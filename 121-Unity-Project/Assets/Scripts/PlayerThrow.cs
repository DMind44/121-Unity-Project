﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerThrow : NetworkBehaviour
{
    private Ray ray;
    private RaycastHit hit;
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

    void Update() {
        // Check what the camera is pointing at
        ray = cam.ScreenPointToRay(Input.mousePosition);
        // if you are currently holding an object, you can't pick up another
        // this assumes that the raycast checking is unnecessary if we are holding an object
        if (!currentObject && Physics.Raycast(ray, out hit)) {
            Interactable inter = hit.collider.gameObject.GetComponent<Interactable>();
            // If we're close enough to the object and it is interactable
            if (hit.distance <= interactableDistance && inter != null) {
                // On mouse click, grab this object
                if (Input.GetMouseButtonDown(0) && !inter.lifted) {
                    CmdGrab(hit.collider.gameObject);
                    currentObject = hit.collider.gameObject;
                } else {
                    inter.BeginHover();
                }
            }
        } else if (currentObject != null && Input.GetMouseButtonDown(0)) {
            // if you are currently holding an object, click to throw it
            CmdThrow(currentObject);
            currentObject = null;
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