using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Predict where will be the target moving in a straight line. Easily countered by moving in circles.
/// Easily improved by increasing shoot speed
/// </summary>
public class AimSystem_Prediction : BaseAimSystem
{
    /// <summary>
    /// The target must have a predictable component
    /// </summary>
    private Predictable predictable = null;

    public override float GetAim()
    {
        if (fixedTarget != null)
        {
            // Get predictable object
            if (predictable == null)
                predictable = fixedTarget.GetComponent<Predictable>();

            // Get prediction
            shootRotation = predictable.GetPrediction(shootSpeed, shootFrom);
            //shootFrom.LookAt(fixedTarget.transform);
            //if(shootRotation != 0)
            //Debug.Log(shootRotation);
            //shootFrom.Rotate(shootFrom.up, shootRotation);
        }
        else
            predictable = null;

        return shootRotation;
    }
}
