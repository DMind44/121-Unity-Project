using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickupEffect;
    public int strengthMod;
    public int speedMod;
    public int healthMod;
    public float duration = 4f;

    void OnTriggerEnter (Collider other) {
        Debug.Log("Collision detecc");
        if (other.CompareTag("Player")) {
            StartCoroutine(Pickup(other));
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

        // player.speedMod = speedMod;
        // player.strengthMod = strengthMod;
        // player.health += healthMod;
        yield return new WaitForSeconds(duration);
        // reverse effects of power up
        Debug.Log("Done");

        Destroy(gameObject);
    }
}
