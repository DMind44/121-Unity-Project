using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

public class CameraController : MonoBehaviour
{

    public GameObject player;

    private Vector3 playerToCameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        playerToCameraOffset = transform.position - player.transform.position;
    }

    // Update the camera position to track the player
    void LateUpdate()
    {
        transform.position = player.transform.position + playerToCameraOffset;
    }
}
