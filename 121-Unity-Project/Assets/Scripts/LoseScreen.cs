using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreen : MonoBehaviour
{
    public GameObject loseScreenUI;
    private bool loseScreenHasAppeared = false;
    // Start is called before the first frame update
    void Start() {
        HideLoseScreen();
    }

    // Update is called once per frame
    void Update() {
        if (GameState.HasLost && !loseScreenHasAppeared) {
            ShowLoseScreen();
            loseScreenHasAppeared = true;
        }
    }

    private void ShowLoseScreen() {
        loseScreenUI.SetActive(true);
    }

    private void HideLoseScreen() {
        loseScreenUI.SetActive(false);
    }
}
