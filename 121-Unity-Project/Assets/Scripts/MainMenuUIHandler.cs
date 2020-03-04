using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;

public class MainMenuUIHandler : MonoBehaviour {

    public NetworkManager networkManager;  // object that we manipulate to join/leave rooms
    public NewNetworkDiscovery networkDiscovery;  // object that we manipulate to find/advertise servers

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    // create a room (as server)
    public void ServeRoom() {
        networkManager.StartServer();
        networkDiscovery.AdvertiseServer();
    }

    // host a game (serve and play)
    public void HostRoom() {
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    // join an existing room (as client)
    public void JoinRoom() {
        networkDiscovery.StartDiscovery();
    }

    // This function is referenced in the NetworkDiscovery component and is
    // called when a server is discovered.
    // If not connected to a server already, connect to the one we just
    // discovered.
    // TODO make this more robust? Stopping discovery altogether with
    // StopDiscovery() crashes the program. Is this because it shuts down the
    // NetworkManager, since NetworkManager and NetworkDiscovery are done by
    // the same object?
    public void OnDiscoveredServer(DiscoveryResponse info) {
        if (!networkManager.isNetworkActive) {
            networkManager.StartClient(info.uri);
        }
    }
}
