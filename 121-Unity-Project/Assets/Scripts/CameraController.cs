using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    [SerializeField] public float heightAbovePlayer;
    [SerializeField] public float distanceBehindPlayer;
    [SerializeField] public float verticalRotationSpeed;
    [SerializeField] public float maxVerticalRotation;

    // private float verticalRotationAmount = 0;

    void Start() {
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void LateUpdate()
    {
        Transform updatedTransform = transform;

        updatedTransform.position = player.transform.position - (distanceBehindPlayer * player.transform.forward) + (heightAbovePlayer * Vector3.up);
        // updatedTransform.rotation = player.transform.rotation;

        // verticalRotationAmount = Mathf.Clamp(verticalRotationAmount - Input.GetAxis("Mouse Y") * verticalRotationSpeed * Time.deltaTime, -maxVerticalRotation, maxVerticalRotation);
        // updatedTransform.Rotate(0f, verticalRotationAmount, 0f);

        transform.position = updatedTransform.position;
        // transform.rotation = updatedTransform.rotation;
    }
}
