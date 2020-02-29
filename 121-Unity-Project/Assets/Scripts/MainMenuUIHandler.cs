using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class MainMenuUIHandler : MonoBehaviour {

    public NetworkManager networkManager;  // object that we manipulate to join/leave rooms
    public NetworkDiscovery networkDiscovery;  // object that we manipulate to find/advertise servers

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();


    // When the user clicks the host button, create a room
    public void HostRoom() {
        networkManager.StartServer();
        print("hosting!");
        networkDiscovery.AdvertiseServer();
        print("advertising!");
    }

    // When the user clicks the join button, join an existing room
    public void JoinRoom() {
        // networkManager.StartClient();
        // networkManager.networkAddress = "localhost";

        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }


    void DiscoverRooms() {
    }
}
