using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage a player
/// </summary>
[RequireComponent(typeof(PlayerInputSystem), typeof(WeaponSystem))]
[RequireComponent(typeof(HealthSystem), typeof(CharacterCollisionSystem))]
public class PlayerSystem : MonoBehaviour
{
    /// <summary>
    /// The level the player is in
    /// </summary>
    public LevelSystem CurrentLevel { get; private set; } = null;

    /// <summary>
    /// The current weapon equiped
    /// </summary>
    public BaseWeapon.BaseWeaponModifier weaponModifier = new Weapon_AssaultRifle();

    private BaseWeapon weapon = null;
    /// <summary>
    /// The movement system
    /// </summary>
    private PlayerInputSystem movementSystem = null;
    /// <summary>
    /// The character controller
    /// </summary>
    private CharacterController characterController = null;
    /// <summary>
    /// The health system
    /// </summary>
    private HealthSystem healthSystem = null;
    /// <summary>
    /// The health system
    /// </summary>
    private WeaponSystem weaponSystem = null;
    /// <summary>
    /// Collision system
    /// </summary>
    private CharacterCollisionSystem collisionSystem = null;
    /// <summary>
    /// The minimap camera
    /// </summary>
    public MinimapFollow minimapCamera = null;

    /// <summary>
    /// Event called when the player dies
    /// </summary>
    public event EventHandler OnDied = null;

    public event EventHandler OnLevelChanged = null;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementSystem = GetComponent<PlayerInputSystem>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDied += HealthSystem_OnDied;
        weaponSystem = GetComponent<WeaponSystem>();
        weapon = GetComponent<BaseWeapon>();
        weaponModifier.SetTarget(weapon);
        weaponSystem.EquipWeapon(weapon);
        collisionSystem = GetComponent<CharacterCollisionSystem>();
        collisionSystem.OnBulletCollisionEnter.AddListener(OnBulletCollision);
    }

    private void HealthSystem_OnDied(object sender, EventArgs e)
    {
        OnDied?.Invoke(this, EventArgs.Empty);
        //Destroy(gameObject);
    }

    /// <summary>
    /// Teleport the player to the specified position
    /// </summary>
    public void TeleportCharacter(Vector3 position)
    {
        // Disable character controller to enable manual position modification
        characterController.enabled = false;
        gameObject.transform.position = position;
        characterController.enabled = true;
    }

    /// <summary>
    /// Teleport the player to a new level
    /// </summary>
    public void SetLevel(GameObject level)
    {
        SetLevel(level.GetComponent<LevelSystem>());
    }

    /// <summary>
    /// Teleport the player to a new level
    /// </summary>
    public void SetLevel(LevelSystem level)
    {
        if (CurrentLevel != null)
        {
            // Call exit event to past level
            CurrentLevel.PlayerExit();
        }

        CurrentLevel = level;

        // Set the minimap
        if (minimapCamera != null)
        {
            minimapCamera.distance = CurrentLevel.minimapDistance;
            if (CurrentLevel.minimapAnchor != null)
            {
                minimapCamera.target = CurrentLevel.minimapAnchor;
            }
            else
            {
                minimapCamera.target = gameObject.transform;
            }
        }

        // Teleport the player to the next level
        TeleportCharacter(CurrentLevel.teleporterEntry.transform.position);
        // Call the enter event
        CurrentLevel.playerSystem = this;
        CurrentLevel.PlayerEnter();

        OnLevelChanged?.Invoke(CurrentLevel, EventArgs.Empty);
    }

    private void CurrentLevel_OnPlayerFall(object sender, EventArgs e)
    {
        TeleportCharacter(CurrentLevel.teleporterEntry.transform.position);
    }

    public void OnBulletCollision(Collider collider)
    {
        // Check bullet source
        BulletSystem bullet = collider.GetComponent<BulletSystem>();
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

    public void EquipWeapon(BaseWeapon.BaseWeaponModifier modifier)
    {
        modifier.SetTarget(weapon);
        weaponSystem.EquipWeapon(weapon);
    }

    public void TakeDamage(int amount)
    {
        healthSystem.TakeDamage(amount);
    }

    public void Heal(int amount)
    {
        healthSystem.GiveHealth(amount);
    }

    /// <summary>
    /// Kill the player
    /// </summary>
    public void Kill()
    {
        // Call the OnDied event
        OnDied?.Invoke(gameObject, EventArgs.Empty);
    }

    /// <summary>
    /// Disable input player movement
    /// </summary>
    public void DisableMovements()
    {
        gameObject.tag = "Untagged";
        movementSystem.enabled = false;
    }

    /// <summary>
    /// Enable input player movements
    /// </summary>
    public void EnableMovements()
    {
        gameObject.tag = "Player";
        movementSystem.enabled = true;
    }

    // Draw gizmos to easily locate player in scene
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up * 100, 5);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 95);
    }
}
