using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    // sets the healthbar to full when player is at max health on startup
    public void setMaxHealth(float health) {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    // update the healthbar when player's health changes
    public void setHealth(float health) {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
