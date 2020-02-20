using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

public class Interactable : MonoBehaviour
{

    public float interactableDistance;  // distance player must be within to interact
    public GameObject player;

    private Color originalColor;
    private Color hoverColor = Color.green;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }


    // If the player is too far from a hovered object, return to original color
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > interactableDistance)
        {
            meshRenderer.material.color = originalColor;
        }
    }

    // Change color if the player hovers over the Interactable and is within interacting distance
    void OnMouseOver()
    {
        // Find which palu
        if (Vector3.Distance(transform.position, player.transform.position) <= interactableDistance)
        {
            meshRenderer.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        meshRenderer.material.color = originalColor;
    }
}
