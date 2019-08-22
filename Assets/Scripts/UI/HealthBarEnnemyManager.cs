using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarEnnemyManager : MonoBehaviour
{
    [Required]
    public GameObject foreground = null;

    public float defaultScale = 2;
    public float defaultLocation = 0;

    public void SetPercentage(float percentage)
    {
        float newScale = percentage * defaultScale;
        float newLocation = defaultLocation + defaultScale * (1 - percentage) / 2f;

        Vector3 currScale = foreground.transform.localScale;
        currScale.x = newScale;
        Vector3 currPos = foreground.transform.localPosition;
        currPos.x = newLocation;

        foreground.transform.localScale = currScale;
        foreground.transform.localPosition = currPos;
    }

    private void Update()
    {
        Camera camera = Camera.main;

        Vector3 delta = camera.transform.position - this.gameObject.transform.position;

        Quaternion currRota = this.gameObject.transform.rotation;
        currRota.SetLookRotation(delta);
        this.gameObject.transform.rotation = currRota;
    }
}
