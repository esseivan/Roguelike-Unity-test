using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage weapons and shooting
/// </summary>
[RequireComponent(typeof(BaseWeapon))]
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
    /// The aim system used
    /// </summary>
    [Required, ShowIf("isEnnemy")]
    public BaseAimSystem aimSystem = null;
    /// <summary>
    /// From where the rotation is made when aiming. Should be a parent of shootFrom
    /// </summary>
    [Required, ShowIf("isEnnemy")]
    public Transform rotateFrom = null;

    /// <summary>
    /// The target to be shoot at (when the parent is an ennemy)
    /// </summary>
    [HideInInspector]
    public GameObject target = null;


    // Time counter variables to wait the firerate time
    private float timeCounter = 0;
    private bool isWaiting = false;
    private float waitTime = 0;

    private void Start()
    {
        if (isEnnemy)
        {
            // Get the aim system for the ennemy
            aimSystem = GetComponent<BaseAimSystem>();
            aimSystem.shootFrom = this.shootFrom;
            aimSystem.rotateFrom = this.rotateFrom;
        }
    }

    /// <summary>
    /// Shoot if available. With aim
    /// </summary>
    public void Shoot()
    {
        // If waiting, don't shoot
        if (isWaiting)
            return;

        // Where to shoot
        Vector3 shootTo = shootFrom.position + shootFrom.forward;
        Debug.DrawLine(shootFrom.position, shootTo, Color.green);

        // Wait the specified time : Fire rate
        isWaiting = true;
        waitTime = equipedWeapon.FireRate;

        //// Get aim
        aimSystem.shootSpeed = equipedWeapon.ShootSpeed;
        aimSystem.fixedTarget = target;

        aimSystem.Aim();

        TerminateShoot(shootTo);
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

        TerminateShoot(shootTo);
    }

    private void TerminateShoot(Vector3 shootTo)
    {
        // Create object at shooter location
        GameObject bullet = Instantiate(bulletObject, shootFrom.transform.position, Quaternion.Euler(bulletObject.transform.eulerAngles));
        // Get default rotation
        Quaternion rotation = bullet.transform.rotation;
        // Orientate object
        Debug.DrawLine(bullet.transform.position, shootTo, Color.red);
        rotation.SetLookRotation(shootTo - bullet.transform.position, Vector3.up);
        // Remove x component, shoot parallel to ground
        Vector3 angles = rotation.eulerAngles;
        angles.x = angles.z = 0;
        bullet.transform.rotation = Quaternion.Euler(angles);
        // Set parameters
        BulletSystem bulletSystem = bullet.GetComponent<BulletSystem>();
        bulletSystem.damage = equipedWeapon.DamagePerShot;
        bulletSystem.LifeTime = equipedWeapon.BulletLifeTime;
        // Set shooter, shooter don't trigger collisions from his bullets
        bulletSystem.shooter = gameObject;
        // Enable bullet lifetime
        bulletSystem.isActive = true;
        // Set force
        bullet.GetComponent<Rigidbody>().AddForce(BaseWeapon.SHOOT_SPEED_CONST_MOD * bullet.transform.forward * equipedWeapon.ShootSpeed, ForceMode.Force);
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
