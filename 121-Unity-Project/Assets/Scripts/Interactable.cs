using UnityEngine;
using Mirror;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

[RequireComponent(typeof(Rigidbody))]
public class Interactable : NetworkBehaviour
{

    public float interactableDistance;  // distance player must be within to interact

    private Color originalColor;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color liftedColor = Color.blue;

    // private GameObject player;

    private bool lifted = false;
    [SerializeField] float speed = 0;

    private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 relativePos = Vector3.zero;
    [SerializeField] private Transform playerT;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        originalColor = meshRenderer.material.color;
    }

    // Called when a mouse is hovering and is close enough
    public void BeginHover() {
        RpcBeginHover();
    }
    [ClientRpc] private void RpcBeginHover() {
        if (!lifted) {
            meshRenderer.material.color = hoverColor;
        }
    }

    // Called when the object is picked up
    [Server] public void Grab(Transform playerTransform) {
        if (!lifted) {
            lifted = true;
            playerT = playerTransform;
            rb.MovePosition(playerT.position + relativePos);
            meshRenderer.material.color = liftedColor;

            GetComponent<Rigidbody>().useGravity = false;

            RpcGrab(playerTransform);
        }
    }

    [ClientRpc] private void RpcGrab(Transform playerTransform) {
        lifted = true;
        playerT = playerTransform;
        rb.MovePosition(playerT.position + relativePos);
        meshRenderer.material.color = liftedColor;

        GetComponent<Rigidbody>().useGravity = false;
    }

    [Server] public void Throw() {
        meshRenderer.material.color = originalColor;
        lifted = false;
        GetComponent<Rigidbody>().useGravity = true;
        rb.velocity = playerT.forward * speed;
        RpcThrow();
    }

    [ClientRpc] private void RpcThrow() {
        meshRenderer.material.color = originalColor;
        lifted = false;
        GetComponent<Rigidbody>().useGravity = true;
        rb.velocity = playerT.forward * speed;
    }

    // [ClientRpc] private void 
    [Server] private void UpdatePos() {
        if (lifted) {
            rb.MovePosition(playerT.position + relativePos);
            RpcUpdatePos();
        }
    }

    [ClientRpc] private void RpcUpdatePos() {
        rb.MovePosition(playerT.position + relativePos);
    }



    // On FixedUpdate, moves itself if it has been lifted
    [Server] void FixedUpdate() {
        UpdatePos();
    }

    // // If the player is too far from a hovered object, return to original color
    // void Update()
    // {
    //     if (Vector3.Distance(transform.position, player.transform.position) > interactableDistance)
    //     {
    //         meshRenderer.material.color = originalColor;
    //     }
    // }

    // // Change color if the player hovers over the Interactable and is within interacting distance
    // void OnMouseOver()
    // {
    //     // 
    //     if (Vector3.Distance(transform.position, player.transform.position) <= interactableDistance
    //     && !lifted)
    //     {
    //         meshRenderer.material.color = hoverColor;
    //     }
    // }

    // Return to original color when mouse leaves
    void OnMouseExit()
    {
        if (!lifted) {
            meshRenderer.material.color = originalColor;
        }
    }
}
