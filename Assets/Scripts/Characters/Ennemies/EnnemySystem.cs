using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement weapons, aim
/// </summary>
[RequireComponent(typeof(WeaponSystem), typeof(CharacterCollisionSystem), typeof(BaseAimSystem))]
public class EnnemySystem : MonoBehaviour
{
    /// <summary>
    /// Damage on contact
    /// </summary>
    public int contactDamage = 5;
    /// <summary>
    /// Damage every x seconds
    /// </summary>
    public float contactDamageEveryXsec = 0.2f;
    /// <summary>
    /// The range ennemies start attacking you
    /// </summary>
    public int targettingRange = 15;

    /// <summary>
    /// The range detection gameobject
    /// </summary>
    [Required]
    public GameObject rangeDetection = null;
    /// <summary>
    /// The body collider object. Used to detect collision on ennemy
    /// </summary>
    [Required]
    public OverlapCollider bodyCollider = null;

    ///// <summary>
    ///// Wheter ennemies automatically face the target
    ///// </summary>
    //public bool faceTarget = true;
    /// <summary>
    /// Event for whenever the ennemy dies
    /// </summary>
    public event System.EventHandler OnDied = null;

    public BaseWeapon.BaseWeaponModifier weapon = new WeaponE_AssaultRifle();
    public BaseWeapon baseWeapon = null;
    public BaseWeapon scaledWeapon = null;

    private GameObject lastCollider = null;
    private HealthSystem lastContact = null;
    private bool isInContact = false;
    private float timeCounter = 0;

    private WeaponSystem weaponSystem = null;
    private HealthSystem healthSystem = null;
    private OverlapCollider entryDetector = null;
    private BaseAimSystem aimSystem = null;

    private GameObject target = null;
    private CharacterCollisionSystem collisionSystem = null;

    public GameObject GetTarget() => target;

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
        baseWeapon = GetComponent<BaseWeapon>();
        scaledWeapon = baseWeapon.Clone().Scale(difficulty);
        // Equip new weapon
        weaponSystem.EquipWeapon(scaledWeapon);
        aimSystem = GetComponent<BaseAimSystem>();

        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDied += HealthSystem_OnDied;
        collisionSystem = GetComponent<CharacterCollisionSystem>();
        collisionSystem.OnPlayerCollisionEnter.AddListener(BodyCollider_OnEnter);
        collisionSystem.OnPlayerCollisionExit.AddListener(BodyCollider_OnExit);
        collisionSystem.OnBulletCollisionEnter.AddListener(OnBulletCollision);

        entryDetector = rangeDetection.GetComponent<OverlapCollider>();
        entryDetector.OnEnter.AddListener(EntryDetection_OnEnter);
        rangeDetection.transform.localScale = new Vector3(targettingRange, 1, targettingRange);
    }

    public void EquipWeapon(BaseWeapon.BaseWeaponModifier weapon)
    {
        weapon.SetTarget(baseWeapon);
        scaledWeapon = baseWeapon.Clone().Scale(difficulty);
        weaponSystem.EquipWeapon(scaledWeapon);
    }

    public void BodyCollider_OnExit(Collider collider)
    {
        lastCollider = null;
        isInContact = false;
        lastContact = null;
        timeCounter = 0;
    }

    public void BodyCollider_OnEnter(Collider collider)
    {
        lastContact = collider.gameObject.GetComponent<HealthSystem>();
        lastCollider = collider.gameObject;
        isInContact = true;
        timeCounter = 0;
    }

    public void EntryDetection_OnExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            target = null;
        }
    }

    public void EntryDetection_OnEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            target = collider.gameObject;
        }
    }

    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        OnDied?.Invoke(sender, e);
        Destroy(gameObject);
    }

    public void OnBulletCollision(Collider other)
    {
        // Check bullet source
        BulletSystem bullet = other.GetComponent<BulletSystem>();
        if (bullet.shooter != this.gameObject)
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

    private void Update()
    {
        // Difficulty changed on the go
        if (difficulty != DifficultyScaler || true)
        {
            difficulty = DifficultyScaler;
            scaledWeapon.ScaleFrom(baseWeapon, difficulty);
        }
        
        if (target != null)
        {
            weaponSystem.target = target;
            weaponSystem.Shoot();
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
