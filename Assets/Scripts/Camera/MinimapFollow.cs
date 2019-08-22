using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    /// <summary>
    /// The distane at which the camera is placed
    /// </summary>
    [MinValue(1)]
    public float distance = 25;

    /// <summary>
    /// The target to be followed
    /// </summary>
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

    public float height = 100;

    private Camera cam = null;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // Set distance
        cam.orthographicSize = distance;

        // Set position and angles
        transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
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
