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
        HidePauseMenu();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Pause")) {
            if (GameState.IsPlaying) {
                Pause();
            } else if (GameState.IsPaused) {
                Play();
            }
        }
    }

    // helper: show the pause menu and lock the cursor (without changing gameplay state)
    private void ShowPauseMenu() {
        pauseMenuUI.SetActive(true);
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    // helper: hide the pause menu and unlock the cursor (without changing gameplay state)
    private void HidePauseMenu() {
        pauseMenuUI.SetActive(false);
        crosshair.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // pause the game
    public void Pause() {
        ShowPauseMenu();
        GameState.Pause();
    }

    // resume the game
    public void Play() {
        HidePauseMenu();
        GameState.Play();
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
