using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable = null;

    [SerializeField]
    private Camera sceneCamera;

    private Camera playerCamera;

    // When the player is initialized, it needs to disable all the components
    // that don't belong to it. If this player isn't the local player,
    // it's going to disable its controller.
    // If this is the local player, it disables main camera to use its own
    void Start() {
        playerCamera = GetComponentsInChildren<Camera>()[0];

        if (!isLocalPlayer) {
            for (int i = 0; i < componentsToDisable.Length; i++) {
                componentsToDisable[i].enabled = false;
            }

            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        } else {
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
                sceneCamera.tag = "Untagged";
                // set Camera.main to the current camera being used
                playerCamera.tag = "MainCamera";
            }

            gameObject.tag = "Player";

            // once the local player is set up, we can play
            GameState.Instance.Play();
        }
    }

    // When the player is disabled, re-enable the main camera
    private void OnDisable() {
        Debug.Log("got disabled");
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
            sceneCamera.tag = "MainCamera";
            playerCamera.tag = "Untagged";
        }
    }
}
