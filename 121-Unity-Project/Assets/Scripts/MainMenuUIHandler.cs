using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenuUIHandler : MonoBehaviour {

    public NetworkManager manager;  // object that we manipulate to perform networking tasks

    // When the user clicks the host button, create a room
    public void HostRoom() {
        manager.StartServer();
        print("hosting!");
    }

    // When the user clicks the join button, join an existing room
    public void JoinRoom() {
        manager.StartClient();
        manager.networkAddress = "localhost";
    }
}
