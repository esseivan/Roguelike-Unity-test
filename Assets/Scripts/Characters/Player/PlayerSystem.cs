using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputSystem), typeof(CharacterController), typeof(WeaponSystem))]
[RequireComponent(typeof(HealthSystem))]
public class PlayerSystem : MonoBehaviour
{
    /// <summary>
    /// The level the player is in
    /// </summary>
    public LevelSystem CurrentLevel { get; private set; } = null;

    public BaseWeapon.BaseWeaponModifier weapon = new Weapon_AssaultRifle();
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
    /// The minimap camera
    /// </summary>
    [Required]
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
        weaponSystem = GetComponent<WeaponSystem>();
        weaponSystem.EquipWeapon(weapon.CreateTarget());
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
        minimapCamera.distance = CurrentLevel.minimapDistance;
        if (CurrentLevel.minimapAnchor != null)
        {
            minimapCamera.target = CurrentLevel.minimapAnchor;
        }
        else
        {
            minimapCamera.target = gameObject.transform;
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "EnnemyBullet")
        {
            BulletSystem bs = collider.GetComponent<BulletSystem>();
            healthSystem.TakeDamage(bs.damage);

            if (bs.type != BaseWeapon.BulletType.Piercing)
                bs.Destroy();
        }
    }

    // Draw gizmos to easily locate player in scene
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up * 100, 5);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 95);
    }
}
