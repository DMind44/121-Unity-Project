using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject crosshair;

    // on start, hide the menu but don't actually change the game state
    // (we don't want the pause menu to be the item that technically makes the
    // call to start up gameplay)
    void Start() {
        Unpause();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Pause")) {
            if (GameState.IsPaused) {
                Unpause();
            } else {
                Pause();
            }
        }
    }

    // show the pause menu and update game state accordingly
    public void Pause() {
        pauseMenuUI.SetActive(true);
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        GameState.Pause();
    }

    // hide the pause menu and update game state accordingly
    public void Unpause() {
        pauseMenuUI.SetActive(false);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        GameState.Unpause();
    }

    public void LoadSettings() {
        Debug.Log("Loading Settings");
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    // TODO: Terminate client-server connection
    public void QuitGame() {
        Debug.Log("Quitting Game");
        SceneManager.LoadScene("Offline");
    }
}
