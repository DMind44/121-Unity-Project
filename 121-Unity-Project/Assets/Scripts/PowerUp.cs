using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickupEffect;
    public float strengthMod = 1f;
    public float speedMod = 1f;
    public float healthMod = 0f;
    public float duration = 4f;

    public PlayerProperties localPlayer;

    public GameObject wing;
    public GameObject fire;
    public GameObject plus;

    void Start() {
        wing.SetActive(false);
        fire.SetActive(false);
        plus.SetActive(false);
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Collision detecc");
        if (other.CompareTag("Player")) {
            StartCoroutine(Pickup(other));
        }
    }

    void Update() {
        // find the local player if we haven't already
        if (localPlayer == null) {
            configureLocalPlayer();
        }
    }

    IEnumerator Pickup(Collider player) {
        Debug.Log("Powerup Collected!");
        Instantiate(pickupEffect, transform.position, transform.rotation);

        // Upon pickup, turn up all children renderers, colliders and lights
        // Creates the effect of the Power Up disappearing
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs) {
            r.enabled = false;
        }

        Collider[] cs = GetComponentsInChildren<Collider>();
        foreach(Collider c in cs) {
            c.enabled = false;
        }

        Light[] ls = GetComponentsInChildren<Light>();
        foreach(Light l in ls) {
            l.enabled = false;
        }

        FindObjectOfType<AudioManager>().Play("PickUp");

        if (speedMod > 1f) {
            wing.SetActive(true);
        } else if (strengthMod > 1f) {
            fire.SetActive(true);
        } else if (healthMod > 0f) {
            plus.SetActive(true);
        }

        // Modify the localplayer's stats accordingly
        PlayerController playerCont = GetComponent<PlayerController>();
        localPlayer.speedMult = speedMod;
        localPlayer.strengthMult = strengthMod;
        localPlayer.hp += healthMod;

        // wait some time
        yield return new WaitForSeconds(duration);
        
        // Reverse effects of power up
        Debug.Log("wait over");
        localPlayer.speedMult = 1f;
        localPlayer.strengthMult = 1f;

        Debug.Log("Done");

        if (speedMod > 1f) {
            wing.SetActive(false);
        } else if (strengthMod > 1f) {
            fire.SetActive(false);
        } else if (healthMod > 0f) {
            plus.SetActive(false);
        }

        Destroy(gameObject);
    }

    // look for the PlayerController of the local player and store it if found
    private void configureLocalPlayer() {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<PlayerController>().isLocalPlayer) {
                localPlayer = player.GetComponent<PlayerProperties>();
                break;
            }
        }
    }
}
