using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [SyncVar] public string username;

    [SerializeField] public int max_hp = 50;
    [SyncVar] public float hp;

    [SyncVar] public int rank;  // which rank (place) you're in; in 1st until you lose

    // Player stats
    [SerializeField] public float strengthMult = 1f;
    [SerializeField] public float speedMult = 1f;
    
    public float movementSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float maxVelocityChange;
    public float gravity;
}
