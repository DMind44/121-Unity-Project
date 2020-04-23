using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [SyncVar] public string username;

    [SerializeField] public int max_hp = 25;
    [SyncVar] public float hp;

    private TextMeshPro usernameTextMesh;

    // on load...
    void Start() {
        // display username above the player
        usernameTextMesh = GetComponentsInChildren<TextMeshPro>()[0];
        usernameTextMesh.text = username;
    }

    void Update() {
        // rotate username to face toward camera
        usernameTextMesh.transform.LookAt(Camera.main.transform.position);
    }

}
