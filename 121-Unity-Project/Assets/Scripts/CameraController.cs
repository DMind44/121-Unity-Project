using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

public class CameraController : MonoBehaviour
{

    public GameObject player;

    public float heightAbovePlayer;
    public float distanceBehindPlayer;
    public float verticalRotationSpeed;
    public float maxVerticalRotation;  // how many degrees above/below center the camera can look

    private float verticalRotationAmount;
    private Transform originalTransform;

    void Start() {
        // begin the camera angles neither up nor down
        verticalRotationAmount = 0;
        originalTransform = transform;
    }

    // Update the camera position to track the player after it moves
    void LateUpdate() {
        Transform updatedTransform = transform;

        // If a player object hasn't been found, look for one.
        // If there isn't a player object, go to original camera position.
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) {
                transform.position = originalTransform.position;
                transform.rotation = originalTransform.rotation;
                return;
            }
        }

        // place and rotate the camera to look the same way as the player
        updatedTransform.position = player.transform.position - (distanceBehindPlayer * player.transform.forward) + (heightAbovePlayer * Vector3.up);
        updatedTransform.rotation = player.transform.rotation;

        // rotate the camera around the player to look further up/down based on the user's vertical mouse movement
        verticalRotationAmount = Mathf.Clamp(verticalRotationAmount - Input.GetAxis("Mouse Y") * verticalRotationSpeed * Time.deltaTime, -maxVerticalRotation, maxVerticalRotation);
        updatedTransform.RotateAround(player.transform.position, player.transform.right, verticalRotationAmount);

        transform.position = updatedTransform.position;
        transform.rotation = updatedTransform.rotation;
    }
}
