using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the main process
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Level generator
    /// </summary>
    [Required]
    public LevelGenerationSystem levelGenerationSystem = null;

    /// <summary>
    /// Player system
    /// </summary>
    [Required]
    public PlayerSystem playerSystem = null;

    public GameObject fadeScreen = null;

    public bool useEnum = true;
    [EnumToggleButtons, ShowIf("useEnum")]
    public Difficulty difficulty = Difficulty.Medium;
    Difficulty lastDifficulty;
    [Range(0.01f, 20), HideIf("useEnum")]
    public float difficultyScaler = 1;
    float lastDifficultyScaler;

    public enum Difficulty
    {
        UNSPECIFIED,
        Beginner,
        Easy,
        Medium,
        Hard,
    }

    public float GetDifficultyScaler(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Beginner:
                return 0.25f;
            case Difficulty.Easy:
                return 0.5f;
            case Difficulty.Hard:
                return 2f;
            default:
                return 1f;
        }
    }

    private void Start()
    {
        SetDifficulty(difficulty);
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameIE());
    }

    private IEnumerator StartGameIE()
    {
        if (fadeScreen != null)
            fadeScreen.SetActive(true);

        yield return new WaitForSeconds(0.05f);
        playerSystem.DisableMovements();
        // Generate levels
        GameObject startLevel = levelGenerationSystem.StartGeneration();
        yield return new WaitForSeconds(0.01f);

        // Start game on first level
        playerSystem.SetLevel(startLevel);

        if (fadeScreen != null)
            fadeScreen.SetActive(false);

        playerSystem.EnableMovements();

        if (levelGenerationSystem.generateBossArea)
            levelGenerationSystem.generatedBossArea.GetComponent<BossSystem>().OnLevelComplete += GameManager_OnLevelComplete;
    }

    private void GameManager_OnLevelComplete(object sender, System.EventArgs e)
    {
        StartGame();
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        lastDifficulty = difficulty;
        lastDifficultyScaler = difficultyScaler = GetDifficultyScaler(difficulty);
        // Update ennemies
        EnnemySystem.SetDifficulty(difficultyScaler);
    }

    public void SetDifficulty(float scaler)
    {
        lastDifficultyScaler = scaler;
        lastDifficulty = difficulty = Difficulty.UNSPECIFIED;
        // Update ennemies
        EnnemySystem.SetDifficulty(difficultyScaler);
    }

    private void Update()
    {
        if (lastDifficulty != difficulty && useEnum)
        {
            SetDifficulty(difficulty);
        }
        else if (lastDifficultyScaler != difficultyScaler && !useEnum)
        {
            SetDifficulty(difficultyScaler);
        }
    }
}
