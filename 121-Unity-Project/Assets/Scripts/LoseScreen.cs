using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoseScreen : MonoBehaviour
{
    public GameObject loseScreenUI;
    public TextMeshProUGUI placeText;  // text box displaying user's place

    private PlayerController localPlayer;

    private bool loseScreenHasAppeared = false;

    // Start is called before the first frame update
    void Start() {
        HideLoseScreen();
    }

    // Update is called once per frame
    void Update() {
        if (localPlayer == null) {
            configureLocalPlayer();
        } else if (GameState.Instance.HasLost && !loseScreenHasAppeared) {
            ShowLoseScreen();
            loseScreenHasAppeared = true;
            Debug.Log("displayed lose screen");
        }

        if (loseScreenHasAppeared && Input.GetMouseButtonDown(0)) {
            HideLoseScreen();
            GameState.Instance.HasDismissedLoseScreen = true;
        }
    }

    private void ShowLoseScreen() {
        placeText.text = "you finished in rank " + localPlayer.rank.ToString() + "/" + GameState.Instance.TotalNumPlayers.ToString();
        loseScreenUI.SetActive(true);
    }

    private void HideLoseScreen() {
        loseScreenUI.SetActive(false);
    }

    // look for the PlayerController of the local player and store it if found
    private void configureLocalPlayer() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<PlayerController>().isLocalPlayer) {
                localPlayer = player.GetComponent<PlayerController>();
                break;
            }
        }
    }
}
