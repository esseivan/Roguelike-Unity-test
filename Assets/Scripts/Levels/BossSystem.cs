using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelSystem))]
public class BossSystem : MonoBehaviour
{
    public GameObject bossObject = null;
    public GameObject wall = null;
    public EntryDetection entryDetection = null;
    public GameObject bossSpawnLocation = null;
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
        entryDetection.OnEnter += EntryDetection_OnEnter;
    }

    private void EntryDetection_OnEnter(object sender, EventArgs e)
    {
        if (bossSpawned)
            return;

        Collider collider = (Collider)sender;

        if(collider.tag =="Player")
        {
            StartBoss();
        }
    }

    private void StartBoss()
    {
        // Spawn boss
        bossSpawned = true;
        GameObject boss = Instantiate(bossObject, bossSpawnLocation.transform);
        wall.SetActive(true);
        playerRespawnTeleporter.SetActive(true);
        bossHealthBar.GetComponent<HealthBarUI>().health = boss.GetComponent<HealthSystem>();
        bossHealthBar.SetActive(true);
        EnnemySystem bossEnemySystem = boss.GetComponent<EnnemySystem>();
        bossEnemySystem.OnDied += BossEnemySystem_OnDied;
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
        levelSystem.OnPlayerFall += LevelSystem_OnPlayerFall;
    }

    private void LevelSystem_OnPlayerFall(object sender, EventArgs e)
    {
        playerSystem.TeleportCharacter(playerRespawnTeleporter.transform.position);
    }

    private void Tp_OnLevelComplete(object sender, System.EventArgs e)
    {
        OnLevelComplete?.Invoke(sender, e);
    }
}
