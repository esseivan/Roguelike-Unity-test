using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon system to manage weapons and shooting
/// </summary>
[RequireComponent(typeof(BaseAimSystem))]
public class WeaponSystem : MonoBehaviour
{
    /// <summary>
    /// The bullet object that is shot
    /// </summary>
    [Required]
    public GameObject bulletObject = null;

    /// <summary>
    /// The currently equiped weapon
    /// </summary>
    public BaseWeapon equipedWeapon = null;

    /// <summary>
    /// From where is the bullet shot
    /// </summary>
    [Required]
    public Transform shootFrom = null;

    /// <summary>
    /// Is the parent an ennemy
    /// </summary>
    public bool isEnnemy = true;
    /// <summary>
    /// From where the rotation is made
    /// </summary>
    [Required, ShowIf("isEnnemy")]
    public Transform rotateFrom = null;

    /// <summary>
    /// The target to be shoot at
    /// </summary>
    [HideInInspector]
    public GameObject target = null;

    // Time counter variables
    private float timeCounter = 0;
    private bool isWaiting = false;
    private float waitTime = 0;

    /// <summary>
    /// The aim system used
    /// </summary>
    private BaseAimSystem aimSystem = null;

    private void Start()
    {
        if (isEnnemy)
        {
            // Get the aim system
            aimSystem = GetComponent<BaseAimSystem>();
            aimSystem.shootFrom = this.shootFrom;
            aimSystem.rotateFrom = this.rotateFrom;
        }
    }

    /// <summary>
    /// Shoot if available
    /// </summary>
    public void Shoot()
    {
        Vector3 shootTo = shootFrom.position + shootFrom.forward * 10;
        Debug.DrawLine(shootFrom.position, shootTo, Color.green);

        // If waiting, don't shoot
        if (isWaiting)
            return;

        // Wait the specified time : Fire rate
        isWaiting = true;
        waitTime = equipedWeapon.FireRate;

        aimSystem.shootSpeed = equipedWeapon.ShootSpeed;
        aimSystem.fixedTarget = target;
        rotateFrom.LookAt(target.transform);
        Vector3 angles = rotateFrom.eulerAngles;
        angles.x = 0;
        rotateFrom.rotation = Quaternion.Euler(angles);
        rotateFrom.Rotate(rotateFrom.up, aimSystem.GetAim());

        // Create object at shooter location with default rotation
        GameObject bullet = Instantiate(bulletObject, shootFrom.position, Quaternion.Euler(bulletObject.transform.eulerAngles));
        // Set tag, whether ennemy or player bullet
        bullet.tag = "Bullet";
        // Set the same rotation as the ShootFrom object
        bullet.transform.rotation = Quaternion.Euler(bullet.transform.rotation.eulerAngles + shootFrom.rotation.eulerAngles);
        // Set parameters
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        bulletSystem.damage = equipedWeapon.DamagePerShot;
        bulletSystem.lifeTime = equipedWeapon.BulletLifeTime;
        // Set shooter, shooter don't trigger collisions from his bullets
        bulletSystem.shooter = gameObject;
        // Enable bullet lifetime
        bulletSystem.isActive = true;
        // Set force
        bullet.GetComponent<Rigidbody>().AddForce(100 * shootFrom.forward * equipedWeapon.ShootSpeed, ForceMode.Force);
    }

    /// <summary>
    /// Shoot if available. Without aim
    /// </summary>
    public void Shoot(Vector3 shootTo)
    {
        // If waiting, don't shoot
        if (isWaiting)
            return;

        // Wait the specified time : Fire rate
        isWaiting = true;
        waitTime = equipedWeapon.FireRate;

        // Create object at shooter location
        GameObject bullet = Instantiate(bulletObject, shootFrom.transform.position, Quaternion.Euler(bulletObject.transform.eulerAngles));
        // Set tag, whether ennemy or player bullet
        bullet.tag = "Bullet";
        // Get default rotation
        Quaternion rotation = bullet.transform.rotation;
        // Orientate object
        Debug.DrawLine(bullet.transform.position, shootTo, Color.red);
        rotation.SetLookRotation(shootTo - bullet.transform.position, Vector3.up);
        // Remove x component, shoot parallel to ground
        Vector3 angles = rotation.eulerAngles;
        angles.x = 0;
        bullet.transform.rotation = Quaternion.Euler(angles);
        // Set parameters
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        bulletSystem.damage = equipedWeapon.DamagePerShot;
        bulletSystem.lifeTime = equipedWeapon.BulletLifeTime;
        // Set shooter, shooter don't trigger collisions from his bullets
        bulletSystem.shooter = gameObject;
        // Enable bullet lifetime
        bulletSystem.isActive = true;
        // Set force
        bullet.GetComponent<Rigidbody>().AddForce(100 * bullet.transform.forward * equipedWeapon.ShootSpeed, ForceMode.Force);
    }

    /// <summary>
    /// Whether shoot can be made right now
    /// </summary>
    public bool CanShoot()
    {
        return !isWaiting;
    }

    /// <summary>
    /// Equip a new weapon
    /// </summary>
    public void EquipWeapon(BaseWeapon weapon)
    {
        equipedWeapon = weapon;
        waitTime = equipedWeapon.DelayWeaponSwitch;
        isWaiting = true;
    }

    /// <summary>
    /// Wait the required amount of time (firerate, weapon switch, reload)
    /// </summary>
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
