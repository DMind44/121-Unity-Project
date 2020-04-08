using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject crosshair;

    void Start() {
        Resume();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Pause")) {
            Debug.Log("Escape pressed!");
            if (GamePaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        GamePaused = false;
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // TODO: freeze local client player
    void Pause() {
        pauseMenuUI.SetActive(true);
        GamePaused = true;
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
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
