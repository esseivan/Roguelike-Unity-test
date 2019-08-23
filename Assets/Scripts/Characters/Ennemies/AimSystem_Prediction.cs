using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AimSystem_Prediction : BaseAimSystem
{
    private RaycastSystem raycastSystem = null;

    protected string targetTag = "Player";

    private bool isFound = false;

    private bool isTargetFixed = false;

    private Predictable predictable = null;

    private void Start()
    {
        if (fixedTarget == null)
        {
            isTargetFixed = false;
            raycastSystem = GetComponent<RaycastSystem>();
            raycastSystem.filterTag = targetTag;
            raycastSystem.OnNewTargetTag += RaycastSystem_OnNewTargetTag;
            raycastSystem.OnTargetLost += RaycastSystem_OnTargetLost;
        }
        else
        {
            isTargetFixed = true;
            predictable = fixedTarget.GetComponent<Predictable>();
        }
    }

    private void Update()
    {
        // Get prediction
        if (isTargetFixed)
        {
            if (!isFound)
            {
                isFound = true;
                TriggerOnTargetAcquired(this);
            }
            shootRotation = predictable.GetPrediction(shootSpeed, shootFrom);
            shootFrom.LookAt(fixedTarget.transform);
            Debug.Log(shootRotation);
            shootFrom.Rotate(shootFrom.up, shootRotation);
        }
        else
        {
            if (isFound)
            {
                if (raycastSystem.lastHit.collider.gameObject.TryGetComponent(out Predictable predictable))
                {
                    shootRotation = predictable.GetPrediction(shootSpeed, shootFrom);
                }
            }
        }
    }

    private void RaycastSystem_OnTargetLost(object sender, EventArgs e)
    {
        isFound = false;
        TriggerOnTargetLost(this);
    }

    private void RaycastSystem_OnNewTargetTag(object sender, EventArgs e)
    {
        isFound = true;
        target = raycastSystem.lastHit;
        TriggerOnTargetAcquired(this);
    }
}
