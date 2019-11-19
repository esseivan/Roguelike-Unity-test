using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Predictable : MonoBehaviour
{
    public float targetSpeed;
    public Vector3 targetPosition;
    public Vector3 targetDirection;

    public float alpha;
    public float beta;

    private CharacterController controller = null;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public float GetPrediction(float speed, Transform rotateFrom)
    {
        targetSpeed = controller.velocity.magnitude;
        targetDirection = controller.velocity;
        targetPosition = transform.position;

        // beta = arcsin(sin(alpha) * lambda)

        // Get speed ratio
        // lambda = vp / vt
        float lambda = (targetSpeed * 0.5f) / speed;

        // Get angle between the target direction and the source
        // alpha = angle(PtPp, Vt)
        alpha = -Vector3.SignedAngle(rotateFrom.position - targetPosition, targetDirection, rotateFrom.up);

        // Get at the source
        // sin(alpha) * lambda
        float sinAngleSrc = Mathf.Sin(alpha * Mathf.Deg2Rad) * lambda;
        beta = Mathf.Asin(sinAngleSrc) * Mathf.Rad2Deg;

        // return the rotated direction
        return beta;
    }
}
