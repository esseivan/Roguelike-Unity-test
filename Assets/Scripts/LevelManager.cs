using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string sceneName = "Level0";

    public int start = 10;
    public int increment = 10;

    private void Start()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        LevelGenerationSystem.levelCount = start;
        LoadLevel();
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        // Level unloaded, load new and increment
        LevelGenerationSystem.levelCount += increment;
        LoadLevel();
    }
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.SetActiveScene(arg0);
    }
}
