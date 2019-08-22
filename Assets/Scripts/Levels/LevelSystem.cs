using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    /// <summary>
    /// The ennemy generator. Disbled if null
    /// </summary>
    public EnnemyGenerationSystem ennemyGenerationSystem = null;

    /// <summary>
    /// The fall detection script
    /// </summary>
    [Required]
    public EntryDetection fallDetector = null;

    /// <summary>
    /// The exit teleporters parent
    /// </summary>
    [Required]
    public GameObject teleportersExit = null;

    /// <summary>
    /// The (only) entry teleporter (one-way). Where the player spawn on the level
    /// </summary>
    [Required]
    public GameObject teleporterEntry = null;
    /// <summary>
    /// The location the minimap hover
    /// </summary>
    public Transform minimapAnchor = null;
    [MinValue(1)]
    public float minimapDistance = 15;

    [EnumToggleButtons]
    public SublevelType sublevelType = SublevelType.Ennemy;

    [HideInInspector]
    public int levelIndex = 0;

    /// <summary>
    /// The amount of teleporters
    /// </summary>
    public int TeleporterCount { get; private set; }

    /// <summary>
    /// The size of the level
    /// </summary>
    [Required]
    public LevelSize levelSize = null;

    [MinValue(0)]
    public float spawnChance = 1f;

    /// <summary>
    /// When player enter the level
    /// </summary>
    public event EventHandler OnPlayerEnter = null;
    /// <summary>
    /// When player exit the level
    /// </summary>
    public event EventHandler OnPlayerExit = null;
    /// <summary>
    /// When the player fall in the void
    /// </summary>
    public event EventHandler OnPlayerFall = null;
    /// <summary>
    /// All ennemies died event
    /// </summary>
    public event EventHandler OnEnnemiesDied = null;
    /// <summary>
    /// One ennemy died event
    /// </summary>
    public event EventHandler OnEnnemyDied = null;
    /// <summary>
    /// On boss teleporter taken
    /// </summary>
    public event EventHandler OnLevelComplete = null;

    /// <summary>
    /// The player system used to teleport the player when falling
    /// </summary>
    [HideInInspector]
    public PlayerSystem playerSystem = null;

    /// <summary>
    /// Indicate if the level is completed
    /// </summary>
    public bool IsSublevelComplete { get; private set; } = false;

    public enum SublevelType
    {
        StartArea,
        Ennemy,
        Loot,
        BossArea
    }

    /// <summary>
    /// Set events on start
    /// </summary>
    private void Start()
    {
        fallDetector.OnEnter += FallDetector_OnEnter;
        if (ennemyGenerationSystem != null)
        {
            ennemyGenerationSystem.OnEnnemyDied += EnnemyGenerationSystem_OnEnnemyDied;
            ennemyGenerationSystem.OnAllEnnemiesDied += EnnemyGenerationSystem_OnAllEnnemiesDied;
        }
    }

    /// <summary>
    /// When player enter, initialize the level and spawn ennemies
    /// </summary>
    public void PlayerEnter()
    {
        // Indicate teleporter manager that the player is here
        TeleporterManager tpm = teleportersExit.GetComponent<TeleporterManager>();
        tpm.SetPlayerHere(true);
        tpm.SetLevelComplete(IsSublevelComplete,levelIndex);

        if (OnPlayerEnter == null)
            tpm.EnableTeleporters();
        else
        {
            OnPlayerEnter?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// When player exit, disable teleporters and kill ennemies (if any) to reduce lag and remove events
    /// </summary>
    public void PlayerExit()
    {
        TeleporterManager tpm = teleportersExit.GetComponent<TeleporterManager>();
        tpm.SetPlayerHere(false);

        OnPlayerExit?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Spawn ennemies
    /// </summary>
    public void SpawnEnnemies()
    {
        ennemyGenerationSystem.SpawnEnnemies();
    }

    /// <summary>
    /// Kill ennemies
    /// </summary>
    public void KillEnnemies()
    {
        ennemyGenerationSystem.KillEnnemies();
    }

    /// <summary>
    /// Manual trigger of event clear
    /// </summary>
    [Button(Name = "Trigger clear condition")]
    public void LevelClearCondition()
    {
        // Enable teleporters
        teleportersExit.GetComponent<TeleporterManager>().EnableTeleporters();
        IsSublevelComplete = true;
        OnLevelComplete?.Invoke(this, EventArgs.Empty);
    }

    #region Events redirection

    private void EnnemyGenerationSystem_OnEnnemyDied(object sender, EventArgs e)
    {
        OnEnnemyDied?.Invoke(sender, e);
    }

    private void EnnemyGenerationSystem_OnAllEnnemiesDied(object sender, EventArgs e)
    {
        OnEnnemiesDied?.Invoke(sender, e);
    }

    /// <summary>
    /// Fall detected event
    /// </summary>
    private void FallDetector_OnEnter(object sender, EventArgs e)
    {
        Collider collider = (Collider)sender;

        if (collider.tag == "Player")
        {
            if (OnPlayerFall == null && playerSystem != null)
                playerSystem.TeleportCharacter(teleporterEntry.transform.position);
            else
                OnPlayerFall.Invoke(sender, e);
        }
        else if (collider.tag == "Ennemy")
        {
            // Kill the ennemy
            collider.GetComponent<EnnemySystem>().Kill();
        }
    }

    #endregion
}
