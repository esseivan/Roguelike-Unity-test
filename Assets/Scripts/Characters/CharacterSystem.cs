using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The character system. Implements collision and health
/// </summary>
[RequireComponent(typeof(HealthSystem))]
public class CharacterSystem : MonoBehaviour
{
    [Required]
    public OverlapCollider bodyCollider = null;

    public OnTriggerEvent OnPlayerCollisionEnter = new OnTriggerEvent();
    public OnTriggerEvent OnPlayerCollisionExit = new OnTriggerEvent();

    private HealthSystem healthSystem = null;

    public void OnTriggerEnter(GameObject collider)
    {
        switch (collider.tag)
        {
            case "Bullet":
                OnBulletCollision(collider);
                break;
            case "Player":
                // Player collision
                OnPlayerCollisionEnter?.Invoke(collider);
                break;
            default:
                break;
        }
    }

    public void OnTriggerExit(GameObject collider)
    {
        switch (collider.tag)
        {
            case "Player":
                // Player collision
                OnPlayerCollisionExit?.Invoke(collider);
                break;
            default:
                break;
        }
    }

    private void OnBulletCollision(GameObject other)
    {
        // Check bullet source
        BulletSystem bullet = other.gameObject.GetComponent<BulletSystem>();
        if (bullet.isActive && bullet.shooter != this.gameObject)
        {
            // Take damage
            healthSystem.TakeDamage(bullet.damage);

            bool destroyBullet = true;

            // Check for bullet type
            switch (bullet.type)
            {
                case BaseWeapon.BulletType.Piercing:
                    destroyBullet = false;
                    break;
                case BaseWeapon.BulletType.Fire:
                    break;
                case BaseWeapon.BulletType.Ice:
                    break;
                default:
                    break;
            }

            if (destroyBullet)
                bullet.Destroy();
        }
    }
}
