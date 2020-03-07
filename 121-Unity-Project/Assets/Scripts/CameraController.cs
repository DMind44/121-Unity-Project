using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    // The default (or starting) height for the camera above the player
    [SerializeField] public float defaultHeightAbovePlayer;
    // The default (or starting) distance for the camera behind the player
    [SerializeField] public float defaultDistanceBehindPlayer;
    // [SerializeField] private float radius;

    // A zero to one value of where the camera is along its track
    [SerializeField] private float normalizedCameraPosition;

    [SerializeField] public float vertMouseSensitivity;
    [SerializeField] public float maxVerticalRotation;  // Angle on top of player
    [SerializeField] public float minVerticalRotation;  // Angle underneath player

    

    void Start() {
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void LateUpdate()
    {
        Transform updatedTransform = transform;

        // updatedTransform.position = player.transform.position - (defaultDistanceBehindPlayer * player.transform.forward) + (defaultHeightAbovePlayer * Vector3.up);
        // updatedTransform.RotateAround(player.transform.position, player.transform.right, Input.GetAxis("Mouse Y") * vertMouseSensitivity);
        updatedTransform.position = player.transform.position - (defaultDistanceBehindPlayer * player.transform.forward) + (defaultHeightAbovePlayer * Vector3.up);
        // updatedTransform.rotation = player.transform.rotation;

        // verticalRotationAmount = Mathf.Clamp(verticalRotationAmount - Input.GetAxis("Mouse Y") * verticalRotationSpeed * Time.deltaTime, -maxVerticalRotation, maxVerticalRotation);
        // updatedTransform.Rotate(0f, verticalRotationAmount, 0f);

        transform.position = updatedTransform.position;
        // transform.rotation = updatedTransform.rotation;
    }
}
