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

    // private GameObject player;

    public bool lifted { get; internal set; }
    public bool flying { get; internal set; }  // true after thrown, false after collision
    [SerializeField] private float speed = 0;

    private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 relativePos = Vector3.zero;
    public Transform playerT { get; internal set; }

    
    [SerializeField] private float cutoff_momentum = 10;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        originalColor = meshRenderer.material.color;
    }

    // Returns the amount of damage caused by this object
    [Server] public float Damage() {
        float momentum = rb.velocity.magnitude * rb.mass;
        if (momentum < cutoff_momentum) {
            return 0f;
        }
        // return (float)Math.Sqrt(momentum);
        return 1f;
    }

    [Server] public void HitSomething(GameObject target) {
        float dmg = Damage();
        RpcHitSomething(target, dmg);
    }
    // Called via a Client command to stop this thing once it hits something
    [Command] public void CmdHitSomething(GameObject target) {
        float dmg = Damage();
        RpcHitSomething(target, dmg);
    }

    [ClientRpc] void RpcHitSomething(GameObject target, float dmg) {
        flying = false;
        target.GetComponent<PlayerController>().DamageMe(dmg);
    }

    // Called when a mouse is hovering and is close enough
    //    Only on client so that only client sees hovering
    [Client] public void BeginHover() {
        if (!lifted) {
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
        meshRenderer.material.color = liftedColor;

        GetComponent<Rigidbody>().useGravity = false;
    }

    [Server] public void Throw() {
        RpcThrow();
    }

    [ClientRpc] private void RpcThrow() {
        meshRenderer.material.color = originalColor;
        lifted = false;
        flying = true;
        GetComponent<Rigidbody>().useGravity = true;
        rb.velocity = playerT.forward * speed;
    }

    [Server] private void UpdatePos() {
        if (lifted) {
            rb.MovePosition(playerT.position + relativePos);
            rb.MoveRotation(playerT.rotation);
            RpcUpdatePos(rb.position, rb.rotation);
        }
    }

    [ClientRpc] private void RpcUpdatePos(Vector3 pos, Quaternion rot) {
        // @TODO (Aely): Check this!
        rb.position = pos;
        rb.rotation = rot;
    }

    // On FixedUpdate, moves itself if it has been lifted
    [ServerCallback] void FixedUpdate() {
        UpdatePos();
    }

    // Return to original color when mouse leaves
    void OnMouseExit()
    {
        if (!lifted) {
            meshRenderer.material.color = originalColor;
        }
    }
}
