using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelSystem))]
public class StartAreaSystem : MonoBehaviour
{
    private LevelSystem levelSystem = null;

    void Start()
    {
        levelSystem = GetComponent<LevelSystem>();
        levelSystem.OnPlayerEnter += LevelSystem_OnPlayerEnter;
    }

    private void LevelSystem_OnPlayerEnter(object sender, System.EventArgs e)
    {
        levelSystem.LevelClearCondition();
    }
}
