using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkRoomPlayer.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkRoomPlayer.html
*/

/// <summary>
/// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
/// The RoomPrefab object of the NetworkRoomManager must have this component on it.
/// This component holds basic room player data required for the room to function.
/// Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.
/// </summary>
public class NewNetworkRoomPlayer : NetworkRoomPlayer
{

    private GameObject parentPanel;

    public GameObject usernameInputFieldPrefab;
    public GameObject usernameTextPrefab;

    public GameObject readyButtonPrefab;

    [SyncVar] string username;

    #region Room Client Callbacks

    /// <summary>
    /// This is a hook that is invoked on all player objects when entering the room.
    /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
    /// </summary>
    public override void OnClientEnterRoom() {
        // adapted from https://forum.unity.com/threads/how-to-create-ui-button-dynamically.393160/#post-2566312
        parentPanel = GameObject.Find("Panel");

        // set default username
        username = "Player " + (index + 1).ToString();

        // create spot for username and ready button
        if (isLocalPlayer) {
            GameObject usernameInputField = Instantiate(usernameInputFieldPrefab);
            // TODO modify coordinates where text is placed
            usernameInputField.transform.position = new Vector3(0, -100 * (index + 1), 0);
            usernameInputField.transform.SetParent(parentPanel.transform, false);
            usernameInputField.GetComponent<TMP_InputField>().text = username;

            // TODO modify coordinates where ready button is placed
            GameObject readyButton = Instantiate(readyButtonPrefab);
            readyButton.transform.position = new Vector3(index * 100, 0, 0);
            readyButton.transform.SetParent(parentPanel.transform, false);
        } else {
            GameObject usernameText = Instantiate(usernameTextPrefab);
            // TODO modify coordinates where text is placed
            usernameText.transform.position = new Vector3(20, 100 * (index + 1), 0);
            usernameText.transform.SetParent(parentPanel.transform, false);

            usernameText.GetComponent<TextMeshProUGUI>().text = username;
        }
        //
        // // create ready button
        // if (isLocalPlayer) {
        //     GameObject readyButton = Instantiate(readyButtonPrefab);
        //     readyButton.transform.position = new Vector3(index * 100, 0, 0);
        //     readyButton.transform.SetParent(parentPanel.transform, false);
        // }




        // Debug.Log(readyButton.transform.position);
    }

    /// <summary>
    /// This is a hook that is invoked on all player objects when exiting the room.
    /// </summary>
    public override void OnClientExitRoom() { }

    /// <summary>
    /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
    /// <para>This function is called when the a client player calls SendReadyToBeginMessage() or SendNotReadyToBeginMessage().</para>
    /// </summary>
    /// <param name="readyState">Whether the player is ready or not.</param>
    public override void OnClientReady(bool readyState) { }

    #endregion

    #region Optional UI

    // public override void OnGUI()
    // {
    //     NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
    //     if (room) {
    //
    //         if (SceneManager.GetActiveScene().name != room.RoomScene) {
    //             return;
    //         }
    //
    //         GUILayout.BeginArea(new Rect(20f + (index * 100), 200f, 90f, 130f));
    //
    //         GUILayout.Label($"Player [{index + 1}]");
    //
    //         if (readyToBegin) {
    //             GUILayout.Label("Ready");
    //         } else {
    //             GUILayout.Label("Not Ready");
    //         }
    //
    //         if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE")) {
    //             // This button only shows on the Host for all players other than the Host
    //             // Host and Players can't remove themselves (stop the client instead)
    //             // Host can kick a Player this way.
    //             GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
    //         }
    //
    //         GUILayout.EndArea();
    //
    //         if (NetworkClient.active && isLocalPlayer) {
    //             GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));
    //
    //             if (readyToBegin) {
    //                 if (GUILayout.Button("Cancel")) {
    //                     CmdChangeReadyState(false);
    //                 }
    //             } else {
    //                 if (GUILayout.Button("Ready")) {
    //                     CmdChangeReadyState(true);
    //                 }
    //             }
    //
    //             GUILayout.EndArea();
    //         }
    //     }
    // }

    #endregion
}
