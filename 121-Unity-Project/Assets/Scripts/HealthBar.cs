using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public PlayerController localPlayer;

    /*    void Start() {
        // find the local player and assign them
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        NetworkManager networkManager = NetworkManager.singleton;
        List<PlayerController> players = NetworkManager.client.connection.playerControllers;
        foreach(GameObject player in players) {
            Debug.Log("fuck you");
            GameObject obj = player.gameObject;
            NetworkBehavior netBev = obj.GetComponent<NetworkBehavior>();
            if (player.IsValid && netBev != null && netBev.isLocalPlayer) {
                localPlayer = player;
                Debug.Log("found local player");
                break;
            }
        }
        setMaxHealth(localPlayer.max_hp);
    }*/

    void Update() {
        // find the local player if we haven't already
        // (the local player may not have been labeled by the time the health
        // bar is set up, so we keep checking until we find it)
        if (localPlayer == null) {
            configureLocalPlayer();
        } else {
            // TODO make the bar only update itself when it's actually changed
            // as opposed to on every frame... maybe using the event system?
            setHealth(localPlayer.hp);
        }
    }

    // update the healthbar to a specified value
    public void setHealth(float health) {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // look for the PlayerController of the local player and store it if found
    private void configureLocalPlayer() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<PlayerController>().isLocalPlayer) {
                localPlayer = player.GetComponent<PlayerController>();
                slider.maxValue = localPlayer.max_hp;
                break;
            }
        }
    }
}
