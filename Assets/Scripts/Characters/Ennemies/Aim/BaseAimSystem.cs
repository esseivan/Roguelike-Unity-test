using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponSystem))]
public class BaseAimSystem : MonoBehaviour
{
    /// <summary>
    /// The rotation needed before to shoot
    /// </summary>
    protected float shootRotation = 0;

    #region PredictionVariables

    /// <summary>
    /// The target the aiming system should aim
    /// </summary>
    [HideInInspector]
    public GameObject fixedTarget = null;

    /// <summary>
    /// The gameobject from where it's aiming
    /// </summary>
    [HideInInspector]
    public Transform shootFrom = null;

    /// <summary>
    /// The gameobject to rotate before shooting
    /// </summary>
    [HideInInspector]
    public Transform rotateFrom = null;

    /// <summary>
    /// The shoot speed. Used for prediction
    /// </summary>
    [HideInInspector]
    public float shootSpeed = 0;

    #endregion

    public virtual float GetAim()
    {
        return shootRotation;
    }
    
    public virtual void Aim()
    {
        rotateFrom.rotation = fixedTarget.transform.rotation;
    }
}