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

    void OnTriggerEnter (Collider other) {
        Debug.Log("Collision detecc");
        if (other.CompareTag("Player")) {
            StartCoroutine(Pickup(other));
        }
    }

    void Update() {
        // find the local player if we haven't already
        // (the local player may not have been labeled by the time the health
        // bar is set up, so we keep checking until we find it)
        if (localPlayer == null) {
            //configureLocalPlayer();
        }
    }

    IEnumerator Pickup(Collider player) {
        Debug.Log("Powerup Collected!");
        Instantiate(pickupEffect, transform.position, transform.rotation);

        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs) {
            r.enabled = false;
        }
        GetComponent<Collider>().enabled = false;

        PlayerController playerCont = GetComponent<PlayerController>();
        //playerCont.speedMult = speedMod;
        //playerCont.strengthMult = strengthMod;
        //playerCont.properties.hp += healthMod;

        //wait some time
        yield return new WaitForSeconds(duration);
        
        //reverse effects of power up
        Debug.Log("wait over");
        //localPlayer.speedMult = 1f;
        //localPlayer.strengthMult = 1f;
        // player.hp += healthMod;

        Debug.Log("Done");

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
