using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;

    void Update() {
        if (Input.GetMouseButton(0)) {
            crosshair.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 40);
            crosshair.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, 40);
        } else {
            crosshair.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, 50);
            crosshair.GetComponent< RectTransform >( ).SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, 50);
        }
    }
}
