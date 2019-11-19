using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aim at the target using the Aim system
/// </summary>
[RequireComponent(typeof(EnnemySystem))]
public class LookAtTarget : MonoBehaviour
{
    private EnnemySystem ennemySystem;
    public Transform rotateFrom;

    private void Start()
    {
        ennemySystem = GetComponent<EnnemySystem>();
    }

    private void LateUpdate()
    {
        GameObject target = ennemySystem.GetTarget();

        if (target == null)
            return;

        LookAt(rotateFrom, target.transform);
    }

    public static void LookAt(Transform rotateFrom, Transform target)
    {
        // Face the target
        var targetRotation = Quaternion.LookRotation(target.transform.position - rotateFrom.position);
        var str = Mathf.Min(0.5f * Time.deltaTime, 1);
        rotateFrom.rotation = Quaternion.Lerp(rotateFrom.rotation, targetRotation, str);

        // Quick
        //rotateFrom.LookAt(lookAt);

        //// Rotate
        //rotateFrom.LookAt(target.transform.position);
        //Vector3 dist = aimSystem.fixedTarget.transform.position - weaponSystem.rotateFrom.position;
        //dist.y = 0;

        //Quaternion from = weaponSystem.rotateFrom.rotation;

        //Quaternion rot = Quaternion.AngleAxis(angleDeg, Vector3.up);
        //Vector3 dest = rot * dist;
        //Quaternion to = Quaternion.LookRotation(dest, Vector3.up);

        //weaponSystem.rotateFrom.rotation = Quaternion.Lerp(from, to, 1);
    }

}