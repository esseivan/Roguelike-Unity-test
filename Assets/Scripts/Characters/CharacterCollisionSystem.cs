using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage collisions
/// </summary>
[RequireComponent(typeof(HealthSystem))]
public class CharacterCollisionSystem : MonoBehaviour
{
    /// <summary>
    /// The collider of the character
    /// </summary>
    [Required]
    public OverlapCollider bodyCollider = null;

    // Collision Enter and Exit events
    public OnTriggerEvent OnCollisionEnter = new OnTriggerEvent();
    public OnTriggerEvent OnCollisionExit = new OnTriggerEvent();

    // Player collision Enter and Exit events
    public OnTriggerEvent OnPlayerCollisionEnter = new OnTriggerEvent();
    public OnTriggerEvent OnPlayerCollisionExit = new OnTriggerEvent();

    // Bullet collision Enter and Exit events
    public OnTriggerEvent OnBulletCollisionEnter = new OnTriggerEvent();
    public OnTriggerEvent OnBulletCollisionExit = new OnTriggerEvent();

    private void Start()
    {
        bodyCollider.OnEnter.AddListener(OnBodyColliderEnter);
        bodyCollider.OnExit.AddListener(OnBodyColliderExit);
    }

    public void OnBodyColliderEnter(Collider collider)
    {
        OnCollisionEnter?.Invoke(collider);
        switch (collider.tag)
        {
            case "Bullet":
                // Bullet collision
                OnBulletCollisionEnter?.Invoke(collider);
                break;
            case "Player":
                // Player collision
                OnPlayerCollisionEnter?.Invoke(collider);
                break;
            default:
                break;
        }
    }

    public void OnBodyColliderExit(Collider collider)
    {
        OnCollisionExit?.Invoke(collider);
        switch (collider.tag)
        {
            case "Bullet":
                // Bullet collision
                OnBulletCollisionExit?.Invoke(collider);
                break;
            case "Player":
                // Player collision
                OnPlayerCollisionExit?.Invoke(collider);
                break;
            default:
                // Others
                break;
        }
    }
}
