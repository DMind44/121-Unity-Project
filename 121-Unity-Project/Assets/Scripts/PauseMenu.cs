using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;

    void Start() {
        pauseMenuUI.SetActive(false);
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
        Cursor.lockState = CursorLockMode.Locked;
    }
    // TODO: freeze local client player
    void Pause() {
        pauseMenuUI.SetActive(true);
        GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMenu() {
        Debug.Log("Loading Menu");
    }

    // TODO: Terminate client-server connection
    public void QuitGame() {
        Debug.Log("Quitting Game");
        SceneManager.LoadScene("Offline");
    }
}
