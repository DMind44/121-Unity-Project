using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentsToDisable = null;

    [SerializeField]
    Camera sceneCamera;

    // When the player is initialized, it needs to disable all the components
    //   that don't belong to it
    void Start() {
        if (!isLocalPlayer) {
            for (int i = 0; i < componentsToDisable.Length; i++) {
                componentsToDisable[i].enabled = false;
            }
        } else {
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    // When the player is disabled, re-enable the main camera
    private void OnDisable() {
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
