using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickupEffect;
    public int strengthMod;
    public int speedMod;
    public int healthMod;

    void OnTriggerEnter (Collider other) {
        Debug.Log("Collision detecc");
        if (other.CompareTag("Player")) {
            Pickup(other);
        }
    }

    void Pickup(Collider player) {
        Debug.Log("Powerup Collected!");
        // Instantiate(pickupEffect, transform.position, transform.rotation);

        player.speedMod = speedMod;
        player.strengthMod = strengthMod;
        player.health += healthMod;

        Destroy(gameObject);
    }
}
