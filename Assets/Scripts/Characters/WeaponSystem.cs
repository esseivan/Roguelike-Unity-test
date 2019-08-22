using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Required]
    public GameObject bulletObject = null;

    public BaseWeapon equipedWeapon = null;

    [Required]
    public Transform shootFrom = null;

    public bool isEnnemy = true;

    float timeCounter = 0;
    bool isWaiting = false;
    float waitTime = 0f;

    public void Shoot(Vector3 shootTo)
    {
        if (isWaiting)
            return;

        isWaiting = true;
        waitTime = equipedWeapon.FireRate;

        // Create object at shooter location
        GameObject bullet = Instantiate(bulletObject, shootFrom.transform.position, Quaternion.Euler(bulletObject.transform.eulerAngles));
        // Set tag
        bullet.tag = isEnnemy ? "EnnemyBullet" : "PlayerBullet";
        // Get default rotation
        Quaternion rotation = bullet.transform.rotation;
        // Orientate object
        Debug.DrawLine(bullet.transform.position, shootTo, Color.yellow);
        rotation.SetLookRotation(shootTo - bullet.transform.position, Vector3.up);
        // Remove x component
        Vector3 angles = rotation.eulerAngles;
        angles.x = 0;
        bullet.transform.rotation = Quaternion.Euler(angles);
        // Set parameters
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        bulletSystem.damage = equipedWeapon.DamagePerShot;
        bulletSystem.lifeTime = equipedWeapon.BulletLifeTime;
        bulletSystem.isEnnemy = isEnnemy;
        bulletSystem.shooter = gameObject;
        // Enable bullet
        bulletSystem.isActive = true;
        // Set force
        bullet.GetComponent<Rigidbody>().AddForce(100 * bullet.transform.forward * equipedWeapon.ShootSpeed, ForceMode.Force);
    }

    public bool CanShoot()
    {
        return !isWaiting;
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        equipedWeapon = weapon;
        waitTime = equipedWeapon.DelayWeaponSwitch;
        isWaiting = true;
    }

    private void Update()
    {
        if (!isWaiting)
            return;

        timeCounter += Time.deltaTime;
        if (timeCounter >= waitTime)
        {
            isWaiting = false;
            timeCounter = 0;
        }
    }
}
