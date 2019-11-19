using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public PlayerSystem playerSystem = null;
    public LevelGenerationSystem levelGenerationSystem = null;

    bool isInit = false;

    private void Update()
    {
        if(!isInit)
        {
            isInit = true;
        }
    }

    private void Start()
    {
        playerSystem.weaponModifier = new Weapon_GOD();
        playerSystem.GetComponent<PlayerInputSystem>().speedMod *= 4;
        playerSystem.OnLevelChanged += PlayerSystem_OnLevelChanged;
    }

    private void PlayerSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        playerSystem.CurrentLevel.teleportersExit.GetComponent<TeleporterManager>().SetLevelComplete(true, playerSystem.CurrentLevel.levelIndex);
    }

    [Button(ButtonHeight = 50)]
    public void ClearCurrentLevel()
    {
        playerSystem.CurrentLevel.LevelClearCondition();
    }

    [Button(ButtonHeight = 50)]
    public void SendToBoss()
    {
        playerSystem.SetLevel(levelGenerationSystem.generatedBossArea);
    }
}

