using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float maxDistance = 1000;

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
        switch (source)
        {
            case Source.Transform:
                ray = new Ray(origin.position, origin.forward);
                break;
            default:
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                break;
        }

        bool result = false;
        RaycastHit hit;
        switch (raycastType)
        {
            case RaycastType.Box:
                result = Physics.BoxCast(ray.origin, Vector3.one * raycastSize, ray.direction, out hit, new Quaternion(), maxDistance, ~excludedLayers.value);
                if (result)
                    ExtDebug.DrawBoxCastOnHit(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, hit.distance, Color.red);
                else
                    ExtDebug.DrawBoxCastBox(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, maxDistance, Color.red);
                break;
            case RaycastType.Sphere:
                result = Physics.SphereCast(ray, raycastSize, out hit, maxDistance, ~excludedLayers.value);
                if (result)
                    ExtDebug.DrawBoxCastOnHit(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, hit.distance, Color.red);
                else
                    ExtDebug.DrawBoxCastBox(ray.origin, Vector3.one * raycastSize, new Quaternion(), ray.direction, maxDistance, Color.red);
                break;
            default:
            case RaycastType.Line:
                result = Physics.Raycast(ray, out hit, maxDistance, ~excludedLayers.value);
                if (result)
                    Debug.DrawLine(ray.origin, hit.point, Color.red);
                else
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                break;
        }

        if (result)
        {
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
            if (!isTargetLost)
            {
                isTargetLost = true;
                OnTargetLost?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
