using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the use of raycast
/// </summary>
public class RaycastSystem : MonoBehaviour
{
    /// <summary>
    /// The last raycast that hit something
    /// </summary>
    public RaycastHit lastHit;
    private bool isTargetLost = false;

    /// <summary>
    /// The source of the raycast
    /// </summary>
    [EnumToggleButtons]
    public Source source = Source.Transform;
    /// <summary>
    /// The origin of the raycast (Transform mode only)
    /// </summary>
    [Required, ShowIf("source", Source.Transform)]
    public Transform origin = null;

    /// <summary>
    /// The type of the raycast
    /// </summary>
    [EnumToggleButtons]
    public RaycastType raycastType = RaycastType.Box;
    /// <summary>
    /// The size of the raycast (radius or box edge size)
    /// </summary>
    [HideIf("raycastType", RaycastType.Line)]
    public float raycastSize = 1f;

    /// <summary>
    /// Call OnNewTargetTag event when a raycast hit something with this specified tag
    /// </summary>
    public string filterTag = string.Empty;

    /// <summary>
    /// The layers to be excluded from the raycast
    /// </summary>
    public LayerMask excludedLayers = new LayerMask() { value = 388 };

    /// <summary>
    /// The maximum distance of the raycast
    /// </summary>
    private readonly float maxDistance = 1000;

    // Events
    public event EventHandler OnNewTarget = null;
    public event EventHandler OnNewTargetTag = null;
    public event EventHandler OnTargetLost = null;

    /// <summary>
    /// The source of the raycast
    /// </summary>
    public enum Source
    {
        /// <summary>
        /// Raycast done by mouse position on screen
        /// </summary>
        Mouse,
        /// <summary>
        /// Raycast is done forward from the transform
        /// </summary>
        Transform,
    }

    /// <summary>
    /// The type of the raycast
    /// </summary>
    public enum RaycastType
    {
        Line,
        Box,
        Sphere,
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        // Get source
        switch (source)
        {
            case Source.Transform:
                // Ray from transform
                ray = new Ray(origin.position, origin.forward);
                break;
            case Source.Mouse:
            default:
                // Ray from mouse position
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                break;
        }

        bool result;
        RaycastHit hit;
        // Get raycast
        switch (raycastType)
        {
            case RaycastType.Box:
                // Raycast a box
                result = Physics.BoxCast(ray.origin, Vector3.one * raycastSize, ray.direction, out hit, new Quaternion(), maxDistance, ~excludedLayers.value);
                // Draw debug
                if (result)
                    ExtDebug.DrawBoxCastOnHit(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, hit.distance, Color.red);
                else
                    ExtDebug.DrawBoxCastBox(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, maxDistance, Color.red);
                break;

            case RaycastType.Sphere:
                // Raycast a sphere
                result = Physics.SphereCast(ray, raycastSize, out hit, maxDistance, ~excludedLayers.value);
                // Draw debug
                if (result)
                    ExtDebug.DrawBoxCastOnHit(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, hit.distance, Color.red);
                else
                    ExtDebug.DrawBoxCastBox(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, maxDistance, Color.red);
                break;
            default:

            case RaycastType.Line:
                // Raycast a line
                result = Physics.Raycast(ray, out hit, maxDistance, ~excludedLayers.value);
                // Draw debug
                if (result)
                    Debug.DrawLine(ray.origin, hit.point, Color.red);
                else
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                break;
        }

        // If something is hit
        if (result)
        {
            // If new target
            if (lastHit.collider != hit.collider || isTargetLost)
            {
                lastHit = hit;
                OnNewTarget?.Invoke(this, EventArgs.Empty);
                if (lastHit.collider.tag == filterTag)
                    OnNewTargetTag?.Invoke(this, EventArgs.Empty);
            }
            else
                lastHit = hit;
            isTargetLost = false;
        }
        else
        {
            // Target lost
            if (!isTargetLost)
            {
                isTargetLost = true;
                OnTargetLost?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
