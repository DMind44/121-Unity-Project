using UnityEngine;
using Mirror;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

[RequireComponent(typeof(Rigidbody))]
public class Interactable : NetworkBehaviour
{

    public float interactableDistance;  // distance player must be within to interact

    [SerializeField]
    private Color hoverColor = Color.green;

    [SerializeField]
    private Color liftedColor = Color.blue;
    
    // private GameObject player;

    private bool lifted = false;
    private Color originalColor;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;

    [SerializeField]
    private Transform guide = null;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        originalColor = meshRenderer.material.color;
    }

    // Called when a mouse is hovering and is close enough
    public void BeginHover() {
        if (!lifted) {
            meshRenderer.material.color = hoverColor;
        }
    }

    // Called when the object is picked up
    public void Grab(Transform newGuide) {
        lifted = true;
        guide = newGuide;
        rb.MovePosition(guide.position + new Vector3(1, 0, 1));
        meshRenderer.material.color = liftedColor;

        GetComponent<Rigidbody>().useGravity = false;
    }

    // On Update, moves itself if it has been lifted
    void Update() {
        if (lifted) {
            rb.MovePosition(guide.position + new Vector3(1, 0, 1));
        }
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
