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

    private void LateUpdate()
    {
        Aim();
    }

    public override float GetAim()
    {
        if (fixedTarget != null)
        {
            // Get predictable object
            if (predictable == null)
                predictable = fixedTarget.GetComponent<Predictable>();

            // Get prediction
            shootRotation = predictable.GetPrediction(shootSpeed, rotateFrom);
        }
        else
            predictable = null;

        return shootRotation;
    }

    public override void Aim()
    {
        if (fixedTarget == null)
            return;

        if (shootSpeed == 0)
            return;

        if (predictable == null)
            predictable = fixedTarget.GetComponent<Predictable>();

        // angle beta
        float beta = GetAim();

        if (beta != 0)
        {
            float alpha = predictable.alpha;

            Vector3 distanceVect = fixedTarget.transform.position - rotateFrom.position;
            distanceVect.y = 0;

            // Get time of impact
            float h = distanceVect.magnitude * Mathf.Sin(alpha * Mathf.Deg2Rad);
            float vpx = shootSpeed * Mathf.Sin((alpha + beta) * Mathf.Deg2Rad);
            float t = h / vpx;

            // distance RotateFrom - Target'
            float r_vp = 0.5f / (shootSpeed * t);
            float epsilon = beta - Mathf.Asin(r_vp);

            // Rotation of angleDeg deg
            Quaternion rot = Quaternion.AngleAxis(epsilon, Vector3.up);
            Vector3 destRotated = rot * distanceVect;
            var targetRotation = Quaternion.LookRotation(destRotated);

            // Get parameter str
            var str = Mathf.Min(10 * Time.fixedDeltaTime, 1);
            rotateFrom.rotation = Quaternion.Lerp(rotateFrom.rotation, targetRotation, str);
            // Draw line
            Debug.DrawLine(shootFrom.localPosition, destRotated * 10, Color.red);
        }
    }
}
