using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverScreenUI;
    public TextMeshProUGUI winnerText;  // text box displaying winner
    public TextMeshProUGUI rankText;  // text box displaying user's rank

    private PlayerController localPlayer;

    private bool gameOverScreenHasAppeared = false;


    // Start is called before the first frame update
    void Start() {
        HideGameOverScreen();
    }

    // Update is called once per frame
    void Update() {
        if (localPlayer == null) {
            configureLocalPlayer();
        } else if (GameState.Instance.GameHasEnded && !gameOverScreenHasAppeared) {
            ShowGameOverScreen();
            gameOverScreenHasAppeared = true;
        }
    }

    private void ShowGameOverScreen() {
        // display the winner
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<PlayerController>().rank == 1) {
                winnerText.text = "Winner: " + player.GetComponent<PlayerProperties>().username;
                break;
            }
        }

        rankText.text = "You ranked " + localPlayer.rank.ToString() + "/" + GameState.Instance.TotalNumPlayers.ToString();
        gameOverScreenUI.SetActive(true);
    }

    private void HideGameOverScreen() {
        gameOverScreenUI.SetActive(false);
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
