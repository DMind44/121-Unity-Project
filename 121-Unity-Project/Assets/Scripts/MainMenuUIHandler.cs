using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class MainMenuUIHandler : MonoBehaviour {

    public NetworkManager networkManager;  // object that we manipulate to join/leave rooms
    // public NetworkDiscovery networkDiscovery;  // object that we manipulate to find/advertise servers

    // readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();


    // create a room (as server)
    public void ServeRoom() {
        networkManager.StartServer();
        print("hosting!");
        // networkDiscovery.AdvertiseServer();
        // print("advertising!");
    }

    // host a game (serve and play)
    public void HostRoom() {
        networkManager.StartHost();
        print("hosting!");
        // networkDiscovery.AdvertiseServer();
        // print("advertising!");
    }

    // join an existing room (as client)
    public void JoinRoom() {
        // networkManager.StartClient();
        // networkManager.networkAddress = "localhost";

        // discoveredServers.Clear();
        // networkDiscovery.StartDiscovery();
        networkManager.StartClient();
        print("client!");
    }


    void DiscoverRooms() {
    }
}
