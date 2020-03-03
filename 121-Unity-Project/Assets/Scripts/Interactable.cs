﻿using UnityEngine;
using System;
using Mirror;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NetworkTransform))]
public class Interactable : NetworkBehaviour {
    private Color originalColor;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color liftedColor = Color.blue;

    // private GameObject player;

    public bool lifted { get; internal set; }
    [SerializeField] float speed = 0;

    private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 relativePos = Vector3.zero;
    public Transform playerT { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        originalColor = meshRenderer.material.color;
    }

    // Called when a mouse is hovering and is close enough
    //    Only on client so that only client sees hovering
    [Client] public void BeginHover() {
        if (!lifted) {
            meshRenderer.material.color = hoverColor;
        }
    }

    // Server-side request to grab this object
    [Server] public void Grab(Transform playerTransform) {
        if (!lifted) {
            RpcGrab(playerTransform);
        }
    }

    // Update all clients on new owner of this Interactable
    [ClientRpc] private void RpcGrab(Transform playerTransform) {
        lifted = true;
        playerT = playerTransform;
        rb.MovePosition(playerT.position + relativePos);
        meshRenderer.material.color = liftedColor;

        GetComponent<Rigidbody>().useGravity = false;
    }

    [Server] public void Throw() {
        RpcThrow();
    }

    [ClientRpc] private void RpcThrow() {
        meshRenderer.material.color = originalColor;
        lifted = false;
        GetComponent<Rigidbody>().useGravity = true;
        rb.velocity = playerT.forward * speed;
    }

    [Server] private void UpdatePos() {
        if (lifted) {
            rb.MovePosition(playerT.position + relativePos);
            rb.MoveRotation(playerT.rotation);
            RpcUpdatePos();
        }
    }

    [ClientRpc] private void RpcUpdatePos() {
        rb.MovePosition(playerT.position + relativePos);
        rb.MoveRotation(playerT.rotation);
    }

    // On FixedUpdate, moves itself if it has been lifted
    [ServerCallback] void FixedUpdate() {
        UpdatePos();
    }

    // // If the player is too far from a hovered object, return to original color
    // void Update()
    // {
    //     if (Vector3.Distance(transform.position, player.transform.position) > interactableDistance)
    //     {
    //         meshRenderer.material.color = originalColor;
    //     }
    // }

    // // Change color if the player hovers over the Interactable and is within interacting distance
    // void OnMouseOver()
    // {
    //     // 
    //     if (Vector3.Distance(transform.position, player.transform.position) <= interactableDistance
    //     && !lifted)
    //     {
    //         meshRenderer.material.color = hoverColor;
    //     }
    // }

    // Return to original color when mouse leaves
    void OnMouseExit()
    {
        if (!lifted) {
            meshRenderer.material.color = originalColor;
        }
    }
}
