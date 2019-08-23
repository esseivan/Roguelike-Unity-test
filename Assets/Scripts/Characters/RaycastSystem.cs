using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSystem : MonoBehaviour
{
    private RaycastHit nullRaycastHit;

    public RaycastHit lastHit;
    private bool isTargetLost = false;

    [EnumToggleButtons]
    public Source source = Source.Transform;
    [Required, ShowIf("source", Source.Transform)]
    public Transform origin = null;

    [EnumToggleButtons]
    public RaycastType raycastType = RaycastType.Box;
    [HideIf("raycastType", RaycastType.Line)]
    public float raycastSize = 1f;

    public string filterTag = string.Empty;

    public LayerMask excludedLayers = new LayerMask() { value = 388 };

    private float maxDistance = 1000;

    public event EventHandler OnNewTarget = null;
    public event EventHandler OnNewTargetTag = null;
    public event EventHandler OnTargetLost = null;

    public enum Source
    {
        Mouse,
        Transform,
    }

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
