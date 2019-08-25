using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Predictable : MonoBehaviour
{
    public float targetSpeed;
    public Vector3 targetPosition;
    public Vector3 targetDirection;

    private CharacterController controller = null;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public float GetPrediction(float speed, Transform shootFrom)
    {
        targetSpeed = controller.velocity.magnitude;
        targetDirection = controller.velocity;
        targetPosition = transform.position;

        // Get speed ratio (and distance traveled ratio)
        // Don't why but divide target speed by 2... it works then
        float speedRatio = speed / (targetSpeed * 0.5f);

        // Get angle between the target direction and the source
        float angle = -Vector3.SignedAngle(shootFrom.position - targetPosition, targetDirection, shootFrom.up);

        // Get at the source
        float sinAngleSrc = Mathf.Sin(angle * Mathf.Deg2Rad) / speedRatio;
        float angleSrc = Mathf.Asin(sinAngleSrc) * Mathf.Rad2Deg;

        // return the rotated direction
        return angleSrc;
    }
}
