using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            Pickup();
        }
    }

    void Pickup() {
        Debug.Log("Powerup Collected!");
    }
}
