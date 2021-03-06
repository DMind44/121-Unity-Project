﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Adapted from the Roll-a-Ball tutorial at
// https://learn.unity.com/project/roll-a-ball-tutorial

// This script moves the player and camera, focusing on keyboard and mouse input
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {
    Animator anim;
    //
    // [SerializeField] private float movementSpeed = 0f;
    // [SerializeField] private float rotationSpeed = 0f;
    // [SerializeField] private float jumpSpeed = 0f;
    // public float gravity = 0f;
    //
    // [SerializeField] private float maxVelocityChange = 0f;

    // color the player changes to when they lose
    [SerializeField] private Color lostColor = Color.red;

    private bool isGrounded = false;
    public ParticleSystem dust;

    private Rigidbody rb;
    private PlayerThrow myThrow;
    private MeshRenderer[] rends;
    public PlayerProperties properties;

    [SerializeField] private Camera cam = null;

    private NewNetworkRoomManager roomManager;

    public bool canMove = true;
    private bool hasLost = false;

    [SyncVar(hook = nameof(EnterLoseGameState))] public int rank = 1;  // which place you came in

    

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        myThrow = GetComponent<PlayerThrow>();
        rends = GetComponentsInChildren<MeshRenderer>();
        roomManager = GameObject.Find("NetworkManager").GetComponent<NewNetworkRoomManager>();
        properties = GetComponent<PlayerProperties>();
        if (GameState.Instance.TotalNumPlayers < 0) {
            CmdInitializeNumPlayers();
        }

        rb.useGravity = false;  // We'll control gravity ourselves

        anim = GameObject.Find("Player Model").GetComponent<Animator>();
    }

    void Update() {
        // Press h to die - for debugging purposes
        if (Input.GetButtonDown("Hurt")) {
            DamageMe(5);
        }

        // Player loses when they lose all health
        if (properties.hp <= 0 && !hasLost) {
            hasLost = true;
            Lose();
        }
    }

    // Run once per frame to move in response to user input.
    // Don't move if Player is actively lifting something.
    //   Source: http://wiki.unity3d.com/index.php/RigidbodyFPSWalker?_ga=2.269071159.757207726.1586110776-1944583397.1580664386
    void FixedUpdate() {
        // Check if the player is out of bounds
        Vector3 pos = rb.position;
        //Debug.Log(pos);
        if (pos.x > 30) {
            //fix
            Debug.Log("Out of Bounds X!");
            pos.x = 28;
        } else if (pos.x < -30) {
            Debug.Log("Out of Boundsx!");
            pos.x = -28;
        } if (pos.z > 30) {
            Debug.Log("Out of BoundsZ!");
            pos.z = 28;
        } else if (pos.z < -30) {
            Debug.Log("Out of Boundsz!");
            pos.z = -28;
        } if (pos.y < 0) {
            Debug.Log("Out of Bounds y !");
            pos.y = 5;
        }
        rb.MovePosition(pos);


        // If we are currently lifting an object, ignore movement and jumping
        if (myThrow == null || myThrow.currentObject == null ||
                myThrow.currentObject.GetComponent<Interactable>() == null ||
                !myThrow.currentObject.GetComponent<Interactable>().lifting || !canMove) {
            // Calculate how fast we should be moving
            Vector3 targetVelocity;
            if (!GameState.Instance.UIIsOpen) {
                targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= properties.movementSpeed;
                targetVelocity *= properties.speedMult;
            } else {
                targetVelocity = Vector3.zero;
            }

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -properties.maxVelocityChange, properties.maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -properties.maxVelocityChange, properties.maxVelocityChange);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump only if the Player is grounded and is pressing Jump.
            if (GameState.Instance.PlayerControlsActive && isGrounded && Input.GetButton("Jump")) {
                rb.velocity = new Vector3(rb.velocity.x, properties.jumpSpeed, rb.velocity.z);
                createDust();
            }
            // animate the player!
            if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0) {
                anim.SetInteger("Speed", 2);
            } else {
                anim.SetInteger("Speed", 0);
            }
            if (!isGrounded) {
                anim.SetBool("Jumping", true);
            } else if (isGrounded) {
                anim.SetBool("Jumping", false);
            }
            // Rotate in response to mouse (i.e., to camera movement)
            if (GameState.Instance.PlayerControlsActive && GameState.Instance.CameraControlsActive) {
                Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                transform.Rotate(Vector3.up, mouseInput.x * properties.rotationSpeed);
            }
        }

        // Add force from gravity
        rb.AddForce(new Vector3 (0, -properties.gravity * rb.mass, 0));

        isGrounded = false;
    }

    void OnCollisionStay(Collision collisionInfo) {
        isGrounded = true;
        // Debug.Log("Grounded");
    }

    [Client] public void DamageMe(float amount) {
        CmdDamageMe(amount);
        Debug.Log("health:" + properties.hp);
        FindObjectOfType<AudioManager>().Play("PlayerHurt");
    }

    [Command] public void CmdDamageMe(float amount) {
        properties.hp -= amount;
    }

    // upon losing all health, change game state to Lose and recolor the player
    private void Lose() {
        Debug.Log("You lost!");
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        CmdUpdateRankAndNumPlayersRemaining();
        RpcRecolorOnLose();
    }

    // wrapper function: once your rank is set, enter the lose game state
    public void EnterLoseGameState(int oldValue, int newValue) {
        Debug.Log("EnterLoseGameState");
        // I believe since this is hooked to a SyncVar, if player 1 loses then
        // this script is called on player 1 on every single machine. However
        // we only want to trigger the lose condition on player 1's machine
        if (isLocalPlayer) {
            GameState.Instance.Lose();

            if (GameState.Instance.NumPlayersRemaining <= 1) {
                Debug.Log("ENDING THE GAME");
                CmdEndGame();
            }
        }
    }

    // TODO: This should happen only once per death
    // recolor the player in all game instances when they die
    private void RpcRecolorOnLose() {
        // FindObjectOfType<AudioManager>().Play("PlayerDeath");
        for (int i = 0; i < rends.Length; i++) {
            if (rends[i] != null) {
                rends[i].material.color = lostColor;
            }
        }
    }

    // Return a reference to this player's camera
    public Camera GetCamera() {
        return cam;
    }

    public void createDust() {
        dust.Play();
    }

    // when the first player loads, update the amount of players on each machine
    [Command] private void CmdInitializeNumPlayers() {
        // NOTE could have race condition here
        GameState.Instance.TotalNumPlayers = roomManager.numPlayers;
        GameState.Instance.NumPlayersRemaining = roomManager.numPlayers;
    }

    // when a player loses, decrease the number of remaining players
    // and update their rank from 1st place
    [Command] private void CmdUpdateRankAndNumPlayersRemaining() {
        print("CmdUpdateRankAndNumPlayersRemaining");
        rank = GameState.Instance.NumPlayersRemaining--;
    }

    [Command] private void CmdEndGame() {
        GameState.Instance.EndGame();
    }
}
