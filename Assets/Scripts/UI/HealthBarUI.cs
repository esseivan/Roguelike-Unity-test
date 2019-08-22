using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public HealthSystem health = null;

    [Required]
    public GameObject healthBar = null;

    public Text healthInfo = null;

    private void Update()
    {
        if (health == null)
            return;

        if(healthInfo != null)
        {
            healthInfo.text = $"{health.health}/{health.maxHealth}";
        }

        float percent = (float)health.health / health.maxHealth;
        // set bar size
        healthBar.transform.localScale = new Vector3(percent, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
