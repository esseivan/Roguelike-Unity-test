using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage the display of a healthbar on the UI, static position
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    /// <summary>
    /// HealthSystem linked to the healthbar
    /// </summary>
    public HealthSystem health = null;

    [Required]
    public SliderSystem slider = null;

    /// <summary>
    /// Text to display the values
    /// </summary>
    public Text healthInfo = null;

    private void Update()
    {
        if (health == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Set text
        if (healthInfo != null)
            healthInfo.text = $"{health.GetHealth()}/{health.maxHealth}";

        float percent = (float)health.GetHealth() / health.maxHealth;

        // set bar size
        slider.SetPercentage(percent);
    }
}
