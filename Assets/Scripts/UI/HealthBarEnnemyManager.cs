using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage health bar above ennemies
/// </summary>
public class HealthBarEnnemyManager : MonoBehaviour
{
    [Required]
    public SliderSystem slider = null;

    public void SetPercentage(float percentage)
    {
        slider.SetPercentage(percentage);
    }

    private void Update()
    {
        // Face the camera
        Camera camera = Camera.main;

        Vector3 delta = camera.transform.position - this.gameObject.transform.position;

        Quaternion currRota = this.gameObject.transform.rotation;
        currRota.SetLookRotation(delta);
        this.gameObject.transform.rotation = currRota;
    }
}
