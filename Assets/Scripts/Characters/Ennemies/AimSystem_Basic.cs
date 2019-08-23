using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycastSystem))]
public class AimSystem_Basic : BaseAimSystem
{
    private RaycastSystem raycastSystem = null;

    protected string targetTag = "Player";

    private void Start()
    {
        raycastSystem = GetComponent<RaycastSystem>();
        raycastSystem.filterTag = targetTag;
        raycastSystem.OnNewTargetTag += RaycastSystem_OnNewTargetTag;
        raycastSystem.OnTargetLost += RaycastSystem_OnTargetLost;
    }

    private void RaycastSystem_OnTargetLost(object sender, EventArgs e)
    {
        TriggerOnTargetLost(this);
    }

    private void RaycastSystem_OnNewTargetTag(object sender, EventArgs e)
    {
        target = raycastSystem.lastHit;
        TriggerOnTargetAcquired(this);
    }
}
