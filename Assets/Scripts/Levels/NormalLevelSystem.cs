using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelSystem))]
public class NormalLevelSystem : MonoBehaviour
{
    private LevelSystem levelSystem = null;

    void Start()
    {
        // Get level system and events
        levelSystem = GetComponent<LevelSystem>();
        levelSystem.OnPlayerEnter += LevelSystem_OnPlayerEnter;
        levelSystem.OnPlayerExit += LevelSystem_OnPlayerExit;
        levelSystem.OnEnnemiesDied += LevelSystem_OnEnnemiesDied;
    }

    private void LevelSystem_OnPlayerEnter(object sender, System.EventArgs e)
    {
        if (levelSystem.ennemyGenerationSystem == null)
            throw new System.ArgumentNullException("Ennemy generation cannot be null in a normal level");

        if (levelSystem.IsSublevelComplete)
            return;

        // Spawn ennemies
        levelSystem.SpawnEnnemies();
    }

    private void LevelSystem_OnPlayerExit(object sender, System.EventArgs e)
    {
        if (levelSystem.ennemyGenerationSystem == null)
            throw new System.ArgumentNullException("Ennemy generation cannot be null in a normal level");

        if (levelSystem.IsSublevelComplete)
            return;

        // Kill remaining ennemies if any
        levelSystem.KillEnnemies();
    }

    private void LevelSystem_OnEnnemiesDied(object sender, System.EventArgs e)
    {
        levelSystem.LevelClearCondition();
    }
}
