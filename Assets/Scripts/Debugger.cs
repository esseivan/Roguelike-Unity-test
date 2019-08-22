using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public PlayerSystem playerSystem = null;
    public LevelGenerationSystem levelGenerationSystem = null;

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
