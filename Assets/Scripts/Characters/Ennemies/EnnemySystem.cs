using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement weapons, aim
/// </summary>
[RequireComponent(typeof(WeaponSystem), typeof(CharacterSystem))]
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

    private GameObject target = null;
    private CharacterSystem characterSystem = null;

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
        characterSystem = GetComponent<CharacterSystem>();
        characterSystem.OnPlayerCollisionEnter.AddListener(BodyCollider_OnEnter);
        characterSystem.OnPlayerCollisionExit.AddListener(BodyCollider_OnExit);
        entryDetector = rangeDetection.GetComponent<OverlapCollider>();
        entryDetector.m_OnEnter.AddListener(EntryDetection_OnEnter);
        rangeDetection.transform.localScale = new Vector3(targettingRange, 1, targettingRange);
    }

    public void EquipWeapon(BaseWeapon.BaseWeaponModifier weapon)
    {
        baseWeapon = weapon.CreateTarget();
        scaledWeapon = weapon.CreateTarget().Scale(difficulty);
        weaponSystem.EquipWeapon(scaledWeapon);
    }

    public void BodyCollider_OnExit(GameObject collider)
    {
        lastCollider = null;
        isInContact = false;
        lastContact = null;
        timeCounter = 0;
    }

    public void BodyCollider_OnEnter(GameObject collider)
    {
        lastContact = collider.gameObject.GetComponent<HealthSystem>();
        lastCollider = collider;
        isInContact = true;
        timeCounter = 0;
    }

    public void EntryDetection_OnExit(GameObject collider)
    {
        if (collider.tag == "Player")
        {
            target = null;
        }
    }

    public void EntryDetection_OnEnter(GameObject collider)
    {
        if (collider.tag == "Player")
        {
            target = collider;
        }
    }

    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        OnDied?.Invoke(sender, e);
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        //if (target != null && faceTarget)
        //{
        //    // Face the target
        //    var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        //    //var str = Mathf.Min(0.5f * Time.deltaTime, 1);
        //    var str = 1;
        //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
        //}
    }

    private void Update()
    {
        // Difficulty changed on the go
        if (difficulty != DifficultyScaler)
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
