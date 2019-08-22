using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Setup a camera to follow a target
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// The angle at which the camera looks at the target (in degrees)
    /// </summary>
    [MinValue(-90), MaxValue(90)]
    public float angle = 45;
    /// <summary>
    /// The same angle in radians
    /// </summary>
    private float angleRad = 0;

    /// <summary>
    /// The distane at which the camera is placed (straight line distance)
    /// </summary>
    [MinValue(1)]
    public float distance = 25;

    /// <summary>
    /// The target to be followed
    /// </summary>
    [Required]
    public Transform target = null;

    /// <summary>
    /// The min zoom (distance)
    /// </summary>
    [MinValue(1)]
    public int minZoom = 5;
    /// <summary>
    /// The max zoom (distance)
    /// </summary>
    [MinValue(1)]
    public int maxZoom = 50;

    private void Update()
    {
        // Get y and z coordinates
        angleRad = Mathf.Deg2Rad * angle;
        float z = -Mathf.Cos(angleRad) * distance;
        float y = Mathf.Sin(angleRad) * distance;

        // Set position and angles
        transform.position = new Vector3(target.position.x, target.position.y + y, target.position.z + z);
        transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// Zoom
    /// </summary>
    public void Zoom(bool fast)
    {
        if (distance == maxZoom)
            return;

        if (fast)
            distance += 5;
        else
            distance += 1;

        if (distance > maxZoom)
            distance = maxZoom;
    }

    /// <summary>
    /// Unzoom
    /// </summary>
    public void UnZoom(bool fast)
    {
        if (distance == minZoom)
            return;

        if (fast)
            distance -= 5;
        else
            distance -= 1;

        if (distance < minZoom)
            distance = minZoom;
    }
}
