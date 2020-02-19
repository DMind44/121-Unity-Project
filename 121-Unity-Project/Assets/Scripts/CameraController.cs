using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

public class CameraController : MonoBehaviour
{

    public GameObject player;
    public float cameraRotationSpeed;

    public float heightAbovePlayer;
    public float distanceBehindPlayer;

    private Vector3 playerToCameraOffset;

    // Update the camera position to track the player
    void LateUpdate()
    {
        // place the camera behind and above the player
        transform.position = player.transform.position - (distanceBehindPlayer * player.transform.forward) + (Vector3.up * heightAbovePlayer);

        // rotate the camera to look the same way the player is
        transform.LookAt(player.transform.position);
    }
}
