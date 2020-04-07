using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        GamePaused = true;
    }

    public void LoadMenu() {
        Debug.Log("Loading Menu");
    }

    public void QuitGame() {
        Debug.Log("Quitting Game");
    }
}
