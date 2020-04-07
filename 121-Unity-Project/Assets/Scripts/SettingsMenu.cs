using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenuUI;
    public GameObject pauseMenuUI;
    public AudioMixer audioMixer;

    void Start() {
        settingsMenuUI.SetActive(false);
    }

    // Start is called before the first frame update
    public void SetVolume(float volume) {
        Debug.Log(volume);
        audioMixer.SetFloat("volume", volume);
    }

    public void Exit() {
        Debug.Log("exiting settings");
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
