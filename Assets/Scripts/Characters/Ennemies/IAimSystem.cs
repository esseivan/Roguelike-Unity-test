using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAimSystem : MonoBehaviour
{
    public RaycastHit target;

    public GameObject fixedTarget = null;

    public Transform shootFrom = null;

    public float shootRotation = 0;

    public float shootSpeed = 0;

    public event EventHandler OnTargetAcquired;
    public event EventHandler OnTargetLost;

    protected void TriggerOnTargetAcquired(object sender)
    {
        OnTargetAcquired?.Invoke(sender, EventArgs.Empty);
    }
    protected void TriggerOnTargetLost(object sender)
    {
        OnTargetLost?.Invoke(sender, EventArgs.Empty);
    }
}