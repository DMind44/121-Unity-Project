using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverScreenUI;
    public TextMeshProUGUI endGameRankTextPrefab;

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
        } else if (GameState.Instance.GameHasEnded && !gameOverScreenHasAppeared &&
                    (GameState.Instance.HasDismissedLoseScreen || !GameState.Instance.HasLost)) {
            ShowGameOverScreen();
            gameOverScreenHasAppeared = true;
        }
    }

    private void ShowGameOverScreen() {
        // display the winner
        int rank;
        string username;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            rank = player.GetComponent<PlayerController>().rank;
            username = player.GetComponent<PlayerProperties>().username;

            TextMeshProUGUI rankText = Instantiate(endGameRankTextPrefab);

            if (rank == 1) {
                rankText.color = Color.yellow;
            }

            if (player.GetComponent<PlayerController>().isLocalPlayer) {
                rankText.text = rank.ToString() + ": " + username + " (YOU!)";
            } else {
                rankText.text = rank.ToString() + ": " + username;
            }

            // TODO change position
            rankText.transform.position = new Vector3(20, -30 * (rank + 1), 0);
            rankText.transform.SetParent(gameOverScreenUI.transform, false);
        }

        // rankText.text = "You ranked " + localPlayer.rank.ToString() + "/" + GameState.Instance.TotalNumPlayers.ToString();
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
