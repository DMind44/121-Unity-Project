using UnityEngine;
using System;
using Mirror;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NetworkTransform))]
public class Interactable : NetworkBehaviour {
    private Color originalColor;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color liftedColor = Color.blue;

    public bool lifted { get; internal set; }
    public bool flying { get; internal set; }  // true after thrown, false after collision
    [SerializeField] private float speed = 0;

    private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 relativePos = Vector3.zero;
    public Transform playerT { get; internal set; }

    
    [SerializeField] private float cutoff_momentum = 10;

    private float dmgAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        if(meshRenderer != null) 
            originalColor = meshRenderer.material.color;
    }

    // Called every time another object is hit
    [ServerCallback] private void OnCollisionEnter(Collision other) {
        // Deal damage if a flying interactable collides with a Player
        if(flying && other.gameObject.CompareTag("Player")) {
            RecalculateDamage();
            RpcHitSomething();
            RpcDamageSomething(other.gameObject, dmgAmount);
        }
    }

    [ClientRpc] private void RpcDamageSomething(GameObject target, float amount) {
        target.GetComponent<PlayerController>().DamageMe(amount);
    }

    // Returns the amount of damage caused by this object
    [Server] public void RecalculateDamage() {
        float momentum = rb.velocity.magnitude * rb.mass;
        if (momentum < cutoff_momentum) {
            dmgAmount = 0f;
        }
        dmgAmount = (float)Math.Sqrt(momentum);
    }

    [ClientRpc] private void RpcHitSomething() {
        flying = false;
    }

    // Called when a mouse is hovering and is close enough
    //    Only on client so that only client sees hovering
    [Client] public void BeginHover() {
        if (!lifted) {
            if(meshRenderer != null)
                meshRenderer.material.color = hoverColor;
        }
    }

    // Server-side request to grab this object
    [Server] public void Grab(Transform playerTransform) {
        if (!lifted) {
            RpcGrab(playerTransform);
        }
    }

    // Update all clients on new owner of this Interactable
    [ClientRpc] private void RpcGrab(Transform playerTransform) {
        lifted = true;
        playerT = playerTransform;
        rb.MovePosition(playerT.position + relativePos);
        if(meshRenderer != null)
            meshRenderer.material.color = liftedColor;

        GetComponent<Rigidbody>().useGravity = false;
    }

    [Server] public void Throw() {
        RpcThrow();
    }

    [ClientRpc] private void RpcThrow() {
        if(meshRenderer != null)
            meshRenderer.material.color = originalColor;
        lifted = false;
        flying = true;
        GetComponent<Rigidbody>().useGravity = true;
        rb.velocity = playerT.forward * speed;
    }

    // On FixedUpdate, moves itself if it has been lifted
    void FixedUpdate() {
        if (lifted) {
            rb.MovePosition(playerT.position + relativePos);
            rb.MoveRotation(playerT.rotation);
        }
    }

    // Return to original color when mouse leaves
    void OnMouseExit() {
        if (!lifted) {
            if(meshRenderer != null)
                meshRenderer.material.color = originalColor;
        }
    }
}
