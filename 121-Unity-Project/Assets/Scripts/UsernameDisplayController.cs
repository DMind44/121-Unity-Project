﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class UsernameDisplayController : NetworkBehaviour
{

    private TextMeshPro usernameTextMesh;

    // on load...
    void Start() {
        // display username above the player
        usernameTextMesh = GetComponentInChildren<TextMeshPro>();
        usernameTextMesh.text = GetComponent<PlayerProperties>().username;
    }

    void Update() {
        // rotate username to face toward camera
        usernameTextMesh.transform.LookAt(Camera.main.transform.position);
    }
}
