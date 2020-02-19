using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from Unity docs:
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html

public class Interactable : MonoBehaviour
{

    private Color originalColor;
    private Color hoverColor = Color.green;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    void OnMouseOver()
    {
        print("you're hovering over an interactable!");
        meshRenderer.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        meshRenderer.material.color = originalColor;
    }
}
