using UnityEngine;
using UnityEngine.UI;
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

    // input field where username is inputted
    public GameObject usernameInputFieldPrefab;
    private GameObject usernameInputField = null;

    // text box where username is displayed
    public GameObject usernameTextPrefab;
    private GameObject usernameText = null;

    // button user clicks to toggle whether or not they are ready
    public GameObject readyButtonPrefab;
    private GameObject readyButton = null;

    // text indicating whether or not the user is ready
    public GameObject readyTextPrefab;
    private GameObject readyText = null;

    [SyncVar(hook="UpdateUsernameDisplay")] public string username;

    #region SyncVar Hooks

    // once the username has changed, update the username displays accordingly
    void UpdateUsernameDisplay(string oldUsername, string newUsername) {
        if (usernameText != null) {
            usernameText.GetComponent<TextMeshProUGUI>().text = username;
        }
        if (usernameInputField != null) {
            usernameInputField.GetComponent<TMP_InputField>().text = username;
        }
    }

    #endregion


    #region Room Client Callbacks

    /// <summary>
    /// This is a hook that is invoked on all player objects when entering the room.
    /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
    /// </summary>
    public override void OnClientEnterRoom() {
        // adapted from https://forum.unity.com/threads/how-to-create-ui-button-dynamically.393160/#post-2566312
        parentPanel = GameObject.Find("Panel");

        // set default username
        if (string.IsNullOrEmpty(username)) {
            CmdUpdateUsername("Player " + (index + 1).ToString());
        }

        // where the username input box/text and ready button/text are displayed
        // TODO change
        Vector3 usernameCoordinates = new Vector3(20, -100 * (index + 1), 0);
        Vector3 readyCoordinates = new Vector3(225, -100 * (index + 1), 0);

        // username text
        usernameText = Instantiate(usernameTextPrefab);
        usernameText.transform.position = usernameCoordinates;
        usernameText.transform.SetParent(parentPanel.transform, false);

        if (isLocalPlayer) {
            // the local player should have a place to enter a username and
            // a ready button
            usernameInputField = Instantiate(usernameInputFieldPrefab);
            usernameInputField.transform.position = usernameCoordinates;
            usernameInputField.transform.SetParent(parentPanel.transform, false);

            readyButton = Instantiate(readyButtonPrefab);
            readyButton.transform.position = readyCoordinates;
            readyButton.transform.SetParent(parentPanel.transform, false);
            readyButton.GetComponent<Button>().onClick.AddListener(() => ToggleReadyState());
        } else {
            // non-local players should have text showing their ready state
            readyText = Instantiate(readyTextPrefab);
            readyText.transform.position = readyCoordinates;
            readyText.transform.SetParent(parentPanel.transform, false);
        }

        UpdateUsernameDisplay(null, username);
        SetPlayerElements(false);
    }


    /// <summary>
    /// This is a hook that is invoked on all player objects when exiting the room.
    /// </summary>
    public override void OnClientExitRoom() {
    }

    /// <summary>
    /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
    /// <para>This function is called when the a client player calls SendReadyToBeginMessage() or SendNotReadyToBeginMessage().</para>
    /// </summary>
    /// <param name="readyState">Whether the player is ready or not.</param>
    public override void OnClientReady(bool readyState) {
        // update the UI to reflect the new ready state
        SetPlayerElements(readyState);
    }

    #endregion

    #region Commands
    // update the username to the specified value
    [Command] void CmdUpdateUsername(string newUsername) {
        username = newUsername;
    }

    #endregion

    #region Helper Functions

    // when the ready button is pressed,
    // update the player's username and then mark the player as ready
    void ToggleReadyState() {
        // if we're making the player ready (they're not currently ready),
        // update the player's username
        if (isLocalPlayer) {
            CmdUpdateUsername(usernameInputField.GetComponent<TMP_InputField>().text);
            CmdChangeReadyState(!readyToBegin);
        }
    }

    // adjust username and ready buttons based on current ready state
    void SetPlayerElements(bool readyState) {
        if (isLocalPlayer) {
            usernameText.SetActive(readyState);
            usernameInputField.SetActive(!readyState);
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = readyState ? "Cancel" : "Ready";
        } else {
            // TODO not sure why this ever comes up null, but it causes errors when it does!
            if (readyText != null) {
                readyText.GetComponent<TextMeshProUGUI>().text = readyState ? "Ready" : "Not Ready";
            }
        }
    }

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
