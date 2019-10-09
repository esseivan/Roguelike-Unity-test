using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletSystem : MonoBehaviour
{
    /// <summary>
    /// The damage to be transmitted to the character
    /// </summary>
    public float damage = 1;
    /// <summary>
    /// The bullet's lifetime
    /// </summary>
    public float lifeTime { get; internal set; } = 1;
    /// <summary>
    /// The bullet's time counter
    /// </summary>
    public float timeCounter { get; internal set; } = 0;
    /// <summary>
    /// Whether the bullet is active (Lifetime counting down and deals damage)
    /// </summary>
    public bool isActive = false;

    /// <summary>
    /// The type of the bullet
    /// </summary>
    public BaseWeapon.BulletType type = BaseWeapon.BulletType.Normal;

    /// <summary>
    /// The shooter of this bullet. The shooters doesn't trigger his bullet's collisions
    /// </summary>
    public GameObject shooter = null;

    /// <summary>
    /// Destroy the bullet
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// The lifetime counter
    /// </summary>
    public void Update()
    {
        if (!isActive)
            return;

        timeCounter += Time.deltaTime;
        if (timeCounter >= lifeTime)
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Debug : Draw a line forward
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
    }
}
