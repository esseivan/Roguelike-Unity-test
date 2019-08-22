using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletSystem : MonoBehaviour
{
    public float damage = 1;
    public float lifeTime = 1;

    public float timeCounter = 0;
    public bool isActive = false;

    public bool isEnnemy = true;

    public BaseWeapon.AmmoType type = BaseWeapon.AmmoType.Normal;

    public GameObject shooter = null;

    public void Update()
    {
        if (!isActive)
            return;

        timeCounter += Time.deltaTime;
        if (timeCounter >= lifeTime)
            Destroy(this.gameObject);
    }
}
