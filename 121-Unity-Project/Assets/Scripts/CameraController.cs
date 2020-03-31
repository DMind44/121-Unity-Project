using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    // Target position RELATIVE TO THE PLAYER
    [SerializeField] private Vector3 targetPos;

    [SerializeField] private float positionUpdateRatio;

    [SerializeField] private float vertMouseSensitivity;
    [SerializeField] private float scrollMouseSensitivity;

    [SerializeField] private float maxVerticalRotation;
    [SerializeField] private float minVerticalRotation;

    [SerializeField] private float maxDistanceToPlayer;
    [SerializeField] private float minDistanceToPlayer;

    void Start() {
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        transform.position = transform.position + targetPos;
    }

    // Every frame as the last thing to do update the camera's position
    private void LateUpdate() {
        // First, react to inputs that change the camera's target position
        Vector3 nextTargetPos = targetPos;

        // Respond to mouse scrolling
        nextTargetPos.y += -scrollMouseSensitivity * Input.mouseScrollDelta.y;
        nextTargetPos.z += scrollMouseSensitivity * Input.mouseScrollDelta.y;
        // Ensure scrolling doesn't get too far or close
        if (nextTargetPos.magnitude < minDistanceToPlayer || nextTargetPos.magnitude > maxDistanceToPlayer) {
            nextTargetPos = targetPos;
        }
        
        // Respond to "change shoulder"
        if (Input.GetButtonDown("SwitchShoulder")) {
            nextTargetPos.x = - nextTargetPos.x;
        }
        targetPos = nextTargetPos;

        // Update position by moving towards targetPos.
        //   Third paramater below makes it so that the movement is a percentage
        //   of the total distance it needs to cover -> smooth camera movements
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos,
        (transform.localPosition - targetPos).magnitude * positionUpdateRatio * Time.deltaTime);

        // Rotate the camera based on mouse movement up or down
        // Reset rotation if it's gone too far
        Quaternion oldRot = transform.rotation;
        transform.Rotate(-Input.GetAxis("Mouse Y") * vertMouseSensitivity, 0, 0);
        float newX = transform.rotation.eulerAngles.x;
        if (newX > 180 && newX - 360 < minVerticalRotation || newX < 180 && newX > maxVerticalRotation) {
            transform.rotation = oldRot;
        }
    }
    
    // void LateUpdate()
    // {
    //     Transform updatedTransform = transform;

    //     // updatedTransform.position = player.transform.position - (defaultDistanceBehindPlayer * player.transform.forward) + (defaultHeightAbovePlayer * Vector3.up);
    //     // updatedTransform.RotateAround(player.transform.position, player.transform.right, Input.GetAxis("Mouse Y") * vertMouseSensitivity);
    //     updatedTransform.position = player.transform.position - (defaultDistanceBehindPlayer * player.transform.forward) + (defaultHeightAbovePlayer * Vector3.up);
    //     // updatedTransform.rotation = player.transform.rotation;

    //     // verticalRotationAmount = Mathf.Clamp(verticalRotationAmount - Input.GetAxis("Mouse Y") * verticalRotationSpeed * Time.deltaTime, -maxVerticalRotation, maxVerticalRotation);
    //     // updatedTransform.Rotate(0f, verticalRotationAmount, 0f);

    //     transform.position = updatedTransform.position;
    //     // transform.rotation = updatedTransform.rotation;
    // }
}
