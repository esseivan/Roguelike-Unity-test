using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage an ennemy
/// </summary>
[RequireComponent(typeof(WeaponSystem), typeof(HealthSystem), typeof(BaseAimSystem))]
public class EnnemySystem : MonoBehaviour
{
    public int contactDamage = 5;
    public float contactDamageEveryXsec = 0.2f;
    public int targettingRange = 15;

    [Required]
    public GameObject rangeDetection = null;
    [Required]
    public EntryDetection bodyCollider = null;

    public bool faceTarget = true;
    /// <summary>
    /// Event for whenever the ennemy dies
    /// </summary>
    public event System.EventHandler OnDied = null;

    public BaseWeapon.BaseWeaponModifier weapon = new WeaponE_AssaultRifle();
    public BaseWeapon baseWeapon = null;
    public BaseWeapon scaledWeapon = null;

    private Collider lastCollider = null;
    private HealthSystem lastContact = null;
    private bool isInContact = false;
    private float timeCounter = 0;

    private WeaponSystem weaponSystem = null;
    private HealthSystem healthSystem = null;
    private EntryDetection entryDetector = null;
    private BaseAimSystem aimSystem = null;

    private Transform target = null;
    private bool isTargetAcquired = false;

    static float DifficultyScaler { get; set; }
    float difficulty;

    public static void SetDifficulty(float scaler)
    {
        DifficultyScaler = scaler;
    }

    private void Start()
    {
        weaponSystem = GetComponent<WeaponSystem>();
        weaponSystem.isEnnemy = true;
        // create scaled weapon
        difficulty = DifficultyScaler;
        baseWeapon = weapon.CreateTarget();
        scaledWeapon = weapon.CreateTarget().Scale(difficulty);
        // Equip new weapon
        weaponSystem.EquipWeapon(scaledWeapon);
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDied += HealthSystem_OnDied;
        entryDetector = rangeDetection.GetComponent<EntryDetection>();
        entryDetector.OnEnter += EntryDetection_OnEnter;
        entryDetector.OnExit += EntryDetection_OnExit;
        bodyCollider.OnEnter += BodyCollider_OnEnter;
        bodyCollider.OnExit += BodyCollider_OnExit;
        aimSystem = GetComponent<BaseAimSystem>();
        aimSystem.OnTargetAcquired += AimSystem_OnTargetAcquired;
        aimSystem.OnTargetLost += AimSystem_OnTargetLost;
        aimSystem.shootFrom = transform;
        aimSystem.shootSpeed = scaledWeapon.ShootSpeed;
        rangeDetection.transform.localScale = new Vector3(targettingRange, 1, targettingRange);
    }

    private void AimSystem_OnTargetLost(object sender, System.EventArgs e)
    {
        Debug.Log("Target lost");
        isTargetAcquired = false;
    }

    private void AimSystem_OnTargetAcquired(object sender, System.EventArgs e)
    {
        Debug.Log("Target Acquired");
        isTargetAcquired = true;
    }

    private void BodyCollider_OnExit(object sender, System.EventArgs e)
    {
        Collider collider = (Collider)sender;
        if (collider == lastCollider)
        {
            lastCollider = null;
            isInContact = false;
            lastContact = null;
            timeCounter = 0;
        }
    }

    private void BodyCollider_OnEnter(object sender, System.EventArgs e)
    {
        Collider collider = (Collider)sender;

        if (collider.tag == "Player")
        {
            lastContact = collider.gameObject.GetComponent<HealthSystem>();
            lastCollider = collider;
            isInContact = true;
            timeCounter = 0;
        }
        else if (collider.tag == "PlayerBullet" || collider.tag == "EnnemyBullet")
        {
            BulletSystem bulletSystem = collider.GetComponent<BulletSystem>();
            if (bulletSystem.isActive && bulletSystem.shooter != gameObject)
                healthSystem.TakeDamage(bulletSystem.damage);
        }
    }

    private void EntryDetection_OnExit(object sender, System.EventArgs e)
    {
        Collider tempTarget = (Collider)sender;
        if (tempTarget.tag == "Player")
        {
            target = null;
        }
    }

    private void EntryDetection_OnEnter(object sender, System.EventArgs e)
    {
        Collider tempTarget = (Collider)sender;
        if (tempTarget.tag == "Player")
        {
            target = tempTarget.transform;
        }
    }

    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        OnDied?.Invoke(sender, e);
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (target != null && faceTarget)
        {
            // Face the target
            var targetRotation = Quaternion.LookRotation(target.position - transform.position);
            var str = Mathf.Min(0.5f * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
        }
    }

    private void Update()
    {
        // Difficulty changed on the go
        if (difficulty != DifficultyScaler)
        {
            difficulty = DifficultyScaler;
            scaledWeapon.ScaleFrom(baseWeapon, difficulty);
            aimSystem.shootSpeed = scaledWeapon.ShootSpeed;
        }

        if (isTargetAcquired && target != null)
        {
            // Manager by aimsystem
            //transform.Rotate(transform.up, aimSystem.shootRotation);
            weaponSystem.Shoot(weaponSystem.shootFrom.transform.forward);
        }

        // Damage on contact to ennemy
        if (isInContact)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= contactDamageEveryXsec)
            {
                int count = Mathf.FloorToInt(timeCounter / contactDamageEveryXsec);
                timeCounter %= contactDamageEveryXsec;
                lastContact.TakeDamage(count * contactDamage);
            }
        }
    }

    /// <summary>
    /// Kill the ennemy
    /// </summary>
    [Button]
    public void Kill()
    {
        // Call the OnDied event when killed
        OnDied?.Invoke(this.gameObject, System.EventArgs.Empty);
        // Destroy the ennemy
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, targettingRange / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);
    }
}
