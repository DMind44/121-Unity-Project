using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable = null;

    [SerializeField]
    private Camera sceneCamera;

    // When the player is initialized, it needs to disable all the components
    //   that don't belong to it. If this player isn't the local player,
    //   it's going to disable its controller.
    // If this is the local player, it disables main camera to use its own
    void Start() {
        if (!isLocalPlayer) {
            for (int i = 0; i < componentsToDisable.Length; i++) {
                componentsToDisable[i].enabled = false;
            }
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        } else {
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
            gameObject.tag = "Player";
        }
    }

    // When the player is disabled, re-enable the main camera
    private void OnDisable() {
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
