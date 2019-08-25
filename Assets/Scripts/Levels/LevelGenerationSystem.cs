using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manage level generation
/// </summary>
public class LevelGenerationSystem : MonoBehaviour
{
    /// <summary>
    /// Parent of created level objects
    /// </summary>
    public GameObject levelsObject = null;

    /// <summary>
    /// Whether to generate a start area or not
    /// </summary>
    public bool generateStartArea = true;
    [ShowIf("generateStartArea"), Required]
    public GameObject startArea = null;

    /// <summary>
    /// Number of levels to generate (sub area and boss area excluded)
    /// </summary>
    [MinValue(0)]
    public int levelsToGenerate = 1;
    /// <summary>
    /// List of levels that can be used. Require at least one level
    /// </summary>
    [Required]
    public List<GameObject> levels = null;
    private List<SublevelObject> levels2 = null;

    public class SublevelObject : IComparer<SublevelObject>
    {
        public GameObject sublevel = null;
        public float spawnPercentage = 0;

        public SublevelObject() { }
        public SublevelObject(GameObject sublevel, float spawnPercentage)
        {
            this.sublevel = sublevel;
            this.spawnPercentage = spawnPercentage;
        }

        public int Compare(SublevelObject x, SublevelObject y)
        {
            if (x.spawnPercentage > y.spawnPercentage)
                return 1;
            if (x.spawnPercentage == y.spawnPercentage)
                return 0;
            return -1;
        }
    }

    /// <summary>
    /// Whether to enerate a boss area or not
    /// </summary>
    public bool generateBossArea = true;
    [ShowIf("generateBossArea"), Required]
    public GameObject bossArea = null;
    [ShowIf("generateBossArea"), Required]
    public GameObject bossHealthBar = null;
    [ShowIf("generateBossArea"), Required]
    public PlayerSystem playerSystem = null;

    public LevelMapGenerator levelMapGenerator = null;

    /// <summary>
    /// List of generated levels
    /// </summary>
    [HideInInspector]
    public List<GameObject> generatedLevels = new List<GameObject>();

    /// <summary>
    /// Generated start area, if any
    /// </summary>
    [HideInInspector]
    public GameObject generatedStartArea = null;
    /// <summary>
    /// Generated boss area, if any
    /// </summary>
    [HideInInspector]
    public GameObject generatedBossArea = null;

    private Vector3 lastPos = Vector3.zero;

    private LevelMapGenerator.GenerationParameters generationParameters = null;

    private int levelIndexCounter = 0;

    public static int levelCount = -1;

    /// <summary>
    /// Start the generation of levels
    /// </summary>
    public GameObject StartGeneration()
    {
        if (levelCount != -1)
            levelsToGenerate = levelCount;

        generationParameters = new LevelMapGenerator.GenerationParameters();
        generationParameters.levelsToGenerate = levelsToGenerate;

        // Start area
        lastPos = GenerateStartArea(lastPos);
        // Levels
        lastPos = GenerateLevels(lastPos);

        GenerateBossArea(lastPos);

        // Create teleporter links
        GenerateLinks();

        // Call map generation
        if (levelMapGenerator != null)
            levelMapGenerator.GenerateMap(generationParameters);

        // Return start location
        if (generateStartArea)
            return generatedStartArea;
        else if (levelsToGenerate != 0)
            return generatedLevels[0];
        else if (generateBossArea)
            return generatedBossArea;
        else
            throw new System.InvalidOperationException("No sublevel generated");
    }


    /// <summary>
    /// Generate start area at 0,0,0 location
    /// </summary>
    public Vector3 GenerateStartArea(Vector3 pos)
    {
        // If start area disabled, skip
        if (!generateStartArea)
            return pos;

        // If start area null, generate an error
        if (startArea == null)
            throw new System.ArgumentNullException();

        // Instantiate start area
        generatedStartArea = Instantiate(startArea, pos + startArea.transform.position, new Quaternion());
        // Set parent
        if (levelsObject != null)
            generatedStartArea.transform.parent = levelsObject.transform;
        // Set offset for the next level
        LevelSystem levelSystem = generatedStartArea.GetComponent<LevelSystem>();
        levelSystem.levelIndex = levelIndexCounter++;
        pos.z += levelSystem.levelSize.transform.localScale.z;

        generationParameters.startArea = generatedStartArea;


        return pos;
    }

    public void PopulateLevels()
    {
        levels2 = new List<SublevelObject>();
        float maxChance = 0;
        foreach (GameObject item in levels)
        {
            maxChance += item.GetComponent<LevelSystem>().spawnChance;
        }

        float chanceCounter = 0;
        // Redetermine all chance percentage
        foreach (GameObject item in levels)
        {
            chanceCounter += item.GetComponent<LevelSystem>().spawnChance / maxChance;
            SublevelObject sublevelObject = new SublevelObject(item, chanceCounter);
            if (sublevelObject.spawnPercentage != 0)
                levels2.Add(sublevelObject);
        }

        // Sort list
        levels2.Sort(new SublevelObject());
    }

    public GameObject GetRandomLevel()
    {
        float rndVal = Random.value;

        foreach (SublevelObject item in levels2)
        {
            if (rndVal <= item.spawnPercentage)
                return item.sublevel;
        }

        return null;
    }

    /// <summary>
    /// Generate levels
    /// </summary>
    public Vector3 GenerateLevels(Vector3 pos)
    {
        if (levelsToGenerate == 0)
            return pos;

        // Get number of lines and columns to create a grid of levels (XZ grid)
        int lines = Mathf.CeilToInt(Mathf.Sqrt(levelsToGenerate));
        int columns = Mathf.CeilToInt((float)levelsToGenerate / lines);
        // Line and column counter
        int iLines = 0, iColumns = 0;
        // Keep track of the greatest z size of the levels on the same line
        float maxZ = 0;

        // Clear precedent level list if any
        generatedLevels.Clear();
        PopulateLevels();

        // Create the required number of levels
        for (int i = 0; i < levelsToGenerate; i++)
        {
            // Get a random level from the list
            GameObject rndLevel = GetRandomLevel();
            // Instantiate this level
            GameObject createdLevel = Instantiate(rndLevel, pos + rndLevel.transform.position, new Quaternion());
            // Set parent
            if (levelsObject != null)
                createdLevel.transform.parent = levelsObject.transform;
            // Add level to list
            generatedLevels.Add(createdLevel);

            // Get levelSystem object
            LevelSystem levelSystem = createdLevel.GetComponent<LevelSystem>();
            levelSystem.levelIndex = levelIndexCounter++;
            // Get greatest z size
            levelSystem.levelSize.UpdateSizes();
            if (levelSystem.levelSize.transform.localScale.z > maxZ)
                maxZ = levelSystem.levelSize.transform.localScale.z;

            // Get position for the next level
            iColumns++;
            // If column overflow
            if (iColumns >= columns)
            {
                // Switch to next line
                iLines++;
                iColumns = 0;
                pos.x = 0;
                pos.z += maxZ;
                maxZ = 0;
            }
            else
            {
                pos.x += levelSystem.levelSize.transform.localScale.x;
            }
        }
        pos.x = 0;
        pos.z += maxZ;

        generationParameters.lines = lines;
        generationParameters.columns = columns;
        generationParameters.sublevels = generatedLevels;

        return pos;
    }

    /// <summary>
    /// Generate boss area
    /// </summary>
    /// <param name="pos"></param>
    public void GenerateBossArea(Vector3 pos)
    {
        // If boss area disabled, skip
        if (!generateBossArea)
            return;

        // If boss area null, generate an error
        if (bossArea == null)
            throw new System.ArgumentNullException();

        // Instantiate start area
        generatedBossArea = Instantiate(bossArea, pos + bossArea.transform.position, new Quaternion());
        // Set parent
        if (levelsObject != null)
            generatedBossArea.transform.parent = levelsObject.transform;

        LevelSystem levelSystem = generatedBossArea.GetComponent<LevelSystem>();
        levelSystem.levelIndex = levelIndexCounter++;

        BossLevelSystem bs = generatedBossArea.GetComponent<BossLevelSystem>();
        bs.bossHealthBar = bossHealthBar;
        bs.playerSystem = playerSystem;

        generationParameters.bossArea = generatedBossArea;
    }

    /// <summary>
    /// Generate bonds between teleporters
    /// </summary>
    public void GenerateLinks()
    {
        /*** Start area link ***/
        if (generateStartArea)
        {
            if (levelsToGenerate > 0)
            {
                // link start area tp to first level
                LevelSystem ls = generatedStartArea.GetComponent<LevelSystem>();
                TeleporterSystem ts = ls.teleportersExit.transform.GetChild(0).GetComponent<TeleporterSystem>();
                ts.destination = generatedLevels[0];
            }
            else
            {
                // link start area tp to boss area
                LevelSystem ls = generatedStartArea.GetComponent<LevelSystem>();
                TeleporterSystem ts = ls.teleportersExit.transform.GetChild(0).GetComponent<TeleporterSystem>();
                ts.destination = generatedBossArea;
            }
        }

        /*** Levels links ***/

        for (int i = 0; i < generatedLevels.Count; i++)
        {
            // Get available levels to be linked
            List<GameObject> availableLevels = new List<GameObject>(generatedLevels);

            // Get levelSystem and remove this level from available list
            GameObject levelObject = availableLevels[i];
            GameObject nextLevel;

            // Get next level
            if ((i + 1) < availableLevels.Count)
                nextLevel = availableLevels[i + 1];
            else if (generateBossArea)
                nextLevel = generatedBossArea;
            else
                break;

            LevelSystem levelSystem = levelObject.GetComponent<LevelSystem>();
            availableLevels.RemoveAt(i);

            // Get teleporters count
            int tpCount = levelSystem.teleportersExit.transform.childCount;

            if (tpCount == 0)
                continue;

            TeleporterSystem teleporterSystem;
            // Set one of the tp to the next level, randomly selected
            if (tpCount == 1)
            {
                teleporterSystem = levelSystem.teleportersExit.transform.GetChild(0).GetComponent<TeleporterSystem>();
                teleporterSystem.destination = nextLevel;
                continue;
            }

            int rndTp = Random.Range(0, levelSystem.teleportersExit.transform.childCount);

            // and the others to a random available level
            availableLevels.Remove(nextLevel);

            for (int j = 0; j < tpCount; j++)
            {
                // Get teleporter at index j
                teleporterSystem = levelSystem.teleportersExit.transform.GetChild(j).GetComponent<TeleporterSystem>();

                if (j == rndTp)
                {
                    teleporterSystem.destination = nextLevel;
                    // Remove the random level from the list
                    availableLevels.Remove(nextLevel);
                }
                else
                {
                    // If no level is available, throw an exception
                    if (availableLevels.Count == 0)
                    {
                        throw new MissingComponentException("Not enough levels");
                    }

                    // Take a random level
                    int rndIndex = Random.Range(0, availableLevels.Count);
                    GameObject rndLevel = availableLevels[rndIndex];

                    // Remove the random level from the list
                    availableLevels.Remove(rndLevel);

                    // Set target destination
                    teleporterSystem.destination = rndLevel;
                }
            }
        }
    }
}
