using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    // Target position RELATIVE TO THE PLAYER
    [SerializeField] private Vector3 targetPos;

    // Changes the speed at which the camera moves to a new position
    //   You can think of it as taking 1/posUpdateRatio seconds to reach
    //   the new target position (i.e. with this = 4 it takes 0.25 seconds)
    [SerializeField] private float positionUpdateRatio;

    // Camera sensitivity to up and down motion
    [SerializeField] private float vertMouseSensitivity;
    // Camera sensitivity to mouse scroll
    [SerializeField] private float scrollMouseSensitivity;
    // Slows down vertical motion - keeps camera lower to the ground during zoomout
    [SerializeField] private float verticalScrollSlowdownRatio;

    // Maximum angle upwards camera can pan relative to horizontal
    [SerializeField] private float maxVerticalRotation;
    // Maximum angle downpwards camera can pan relative to horizontal
    [SerializeField] private float minVerticalRotation;

    // The maximum distance away from player camera can zoom out
    [SerializeField] private float maxDistanceToPlayer;
    // The minimum distance into the player camera can zoom in
    [SerializeField] private float minDistanceToPlayer;
    // The minimum y value allowed when your back is against the wall
    //    Aely: "This should be set to around the height of the player"
    [SerializeField] private float minRelativeHeight;

    void Start() {
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Begin at default position relative to the player
        transform.localPosition = transform.localPosition + targetPos;
    }

    // Every frame as the last thing to do update the camera's position
    private void LateUpdate() {
        // First, react to inputs that change the camera's target position
        Vector3 nextTargetPos = targetPos;

        // Respond to mouse scrolling
        nextTargetPos.y += -scrollMouseSensitivity * Input.mouseScrollDelta.y * verticalScrollSlowdownRatio;
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

        // Now, do a Raycast to ensure the camera can see the player
        // From the player, Raycast towards the camera by no farther than the 
        //   distance to the camera. If there is a hit, then something is in
        //   the way of the camera and it has to get moved closer
        RaycastHit hit;
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;
        if (Physics.Raycast(player.transform.position,
              transform.position - player.transform.position,
              out hit, distanceToPlayer)) {
            Debug.Log("Hitting something!");
            Debug.Log(hit.collider);
            Debug.Log(hit.distance);
            Vector3 nextLocalPos = transform.localPosition;
            nextLocalPos.y *= hit.distance / distanceToPlayer;
            if (nextLocalPos.y < minRelativeHeight) {
                nextLocalPos.y = minRelativeHeight;
            }
            nextLocalPos.z *= hit.distance / distanceToPlayer;
            transform.localPosition = nextLocalPos;
        }

        // Rotate the camera based on mouse movement up or down
        // Reset rotation if it's gone too far
        Quaternion oldRot = transform.rotation;
        transform.Rotate(-Input.GetAxis("Mouse Y") * vertMouseSensitivity, 0, 0);
        float newX = transform.rotation.eulerAngles.x;
        if (newX > 180 && newX - 360 < minVerticalRotation || newX < 180 && newX > maxVerticalRotation) {
            transform.rotation = oldRot;
        }
    }
}
