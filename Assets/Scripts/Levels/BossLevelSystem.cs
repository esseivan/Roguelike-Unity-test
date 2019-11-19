using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelSystem))]
public class BossLevelSystem : MonoBehaviour
{
    [Required]
    public GameObject bossObject = null;
    [Required]
    public GameObject wall = null;
    [Required]
    public OverlapCollider entryDetection = null;
    [Required]
    public GameObject bossSpawnLocation = null;
    [Required]
    public GameObject playerRespawnTeleporter = null;

    [HideInInspector]
    public GameObject bossHealthBar = null;

    private LevelSystem levelSystem = null;

    [HideInInspector]
    public PlayerSystem playerSystem = null;

    public event EventHandler OnLevelComplete = null;

    private bool bossSpawned = false;

    void Start()
    {
        levelSystem = GetComponent<LevelSystem>();
        levelSystem.OnPlayerEnter += LevelSystem_OnPlayerEnter;
        entryDetection.OnEnter.AddListener(EntryDetection_OnEnter);
    }

    public void EntryDetection_OnEnter(Collider collider)
    {
        if (bossSpawned)
            return;

        if (collider.tag == "Player")
        {
            StartBoss();
        }
    }

    private void StartBoss()
    {
        // Spawn boss
        bossSpawned = true;
        GameObject boss = Instantiate(bossObject, bossSpawnLocation.transform);
        boss.GetComponent<AimSystem_Prediction>().fixedTarget = playerSystem.gameObject;
        wall.SetActive(true);
        playerRespawnTeleporter.SetActive(true);
        bossHealthBar.GetComponent<HealthBarUI>().health = boss.GetComponent<HealthSystem>();
        bossHealthBar.SetActive(true);
        EnnemySystem bossEnnemySystem = boss.GetComponent<EnnemySystem>();
        bossEnnemySystem.weapon = new WeaponB_AssaultRifle();
        bossEnnemySystem.OnDied += BossEnemySystem_OnDied;
        levelSystem.OnPlayerFall += LevelSystem_OnPlayerFall;
    }

    private void BossEnemySystem_OnDied(object sender, EventArgs e)
    {
        bossHealthBar.SetActive(false);
        levelSystem.LevelClearCondition();
    }

    private void LevelSystem_OnPlayerEnter(object sender, EventArgs e)
    {
        TeleporterSystem tp = levelSystem.teleportersExit.transform.GetChild(0).GetComponent<TeleporterSystem>();
        tp.isBossTeleporter = true;
        tp.OnLevelComplete += Tp_OnLevelComplete;
    }

    private void LevelSystem_OnPlayerFall(object sender, Collider collider)
    {
        playerSystem.TeleportCharacter(playerRespawnTeleporter.transform.position);
        playerSystem.TakeDamage(10);
    }

    private void Tp_OnLevelComplete(object sender, System.EventArgs e)
    {
        OnLevelComplete?.Invoke(sender, e);
    }
}
