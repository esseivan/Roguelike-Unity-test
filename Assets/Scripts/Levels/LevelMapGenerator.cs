using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapGenerator : MonoBehaviour
{
    /// <summary>
    /// Pivot has to be at 0 and 0.5 (mid left)
    /// </summary>
    [Required]
    public GameObject gridmapObject = null;

    [Required]
    public GameObject gridItemObject = null;

    public float width = 400;
    public float height = 400;
    private float itemWidth = 0;
    private float itemHeight = 0;

    private readonly List<GridLevelObject> grid = new List<GridLevelObject>();

    private void Start()
    {
        itemWidth = gridItemObject.GetComponent<RectTransform>().sizeDelta.x;
        itemHeight = gridItemObject.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void GenerateMap(GenerationParameters parameters)
    {
        if (parameters == null)
        {
            throw new System.ArgumentNullException();
        }

        // Generate grid
        grid.Clear();
        gridItemObject.SetActive(true);
        /*** Sublevels ***/
        // Rescale object if required
        if (parameters.columns > (width - 2 * itemWidth) / itemWidth)
        {
            // Get scale factor
            float percentage = width / ((parameters.columns + 2) * itemWidth);
            // Scale width accordingly
            Vector3 scale = gridItemObject.transform.localScale;
            scale.x *= percentage;
            itemWidth *= percentage;
            gridItemObject.transform.localScale = scale;
        }
        if (parameters.lines > height / itemHeight)
        {
            // Get scale factor
            float percentage = height / (parameters.lines * itemHeight);
            // Scale width accordingly
            Vector3 scale = gridItemObject.transform.localScale;
            scale.y *= percentage;
            itemHeight *= percentage;
            gridItemObject.transform.localScale = scale;
        }

        /*** Start area ***/
        if (parameters.startArea != null)
        {
            GameObject createdObject = Instantiate(gridItemObject, gridmapObject.transform);
            grid.Add(new GridLevelObject()
            {
                sublevel = parameters.startArea,
                isStartArea = true,
                itemObject = createdObject.GetComponent<LevelMapSystem>(),
            });
            createdObject.transform.localPosition = new Vector2(itemWidth / 2, 0);
        }

        Vector2 offset = new Vector2(itemWidth + itemWidth / 2, (itemHeight / 2f) * (parameters.lines - 1));
        Vector2 pos = Vector2.zero;
        Vector2 bossPos = new Vector2(width - itemWidth / 2, 0);

        for (int i = 0; i < parameters.lines; i++)
        {
            for (int j = 0; j < parameters.columns; j++)
            {
                int k = parameters.columns * i + j;
                // If max reached, exit
                if(k >= parameters.levelsToGenerate)
                {
                    i = parameters.lines;
                    j = parameters.columns;
                    continue;
                }

                GameObject createdObject = Instantiate(gridItemObject, gridmapObject.transform);

                grid.Add(new GridLevelObject()
                {
                    sublevel = parameters.sublevels[k],
                    lineIndex = i,
                    columnIndex = j,
                    itemObject = createdObject.GetComponent<LevelMapSystem>(),
                });
                createdObject.transform.localPosition = offset + pos;

                pos.x += itemWidth;
            }
            if (i == 0)
            {
                bossPos = new Vector2(itemWidth * 1.5f + pos.x, 0);
            }
            pos.x = 0;
            pos.y -= itemHeight;
        }

        /*** Boss area ***/
        if (parameters.bossArea != null)
        {
            GameObject createdObject = Instantiate(gridItemObject, gridmapObject.transform);
            grid.Add(new GridLevelObject()
            {
                sublevel = parameters.bossArea,
                isBossArea = true,
                itemObject = createdObject.GetComponent<LevelMapSystem>(),
            });
            createdObject.transform.localPosition = bossPos;
        }
        gridItemObject.SetActive(false);

        // Add OnPlayerEnter events
        foreach (GridLevelObject item in grid)
        {
            item.itemObject.SetVisible(false);
            item.itemObject.SetSeen(false);
            item.itemObject.SetText(item.sublevel.GetComponent<LevelSystem>().sublevelType);
            LevelSystem levelSystem = item.sublevel.GetComponent<LevelSystem>();
            levelSystem.OnPlayerEnter += LevelMapGenerator_OnPlayerEnter;
            levelSystem.OnPlayerExit += LevelSystem_OnPlayerExit;
        }
    }

    private void LevelSystem_OnPlayerExit(object sender, System.EventArgs e)
    {
        GridLevelObject item = grid.Where((x) => x.sublevel.GetComponent<LevelSystem>() == (LevelSystem)sender).FirstOrDefault();
        item.itemObject.SetSeen(false);
    }

    private void LevelMapGenerator_OnPlayerEnter(object sender, System.EventArgs e)
    {
        GridLevelObject item = grid.Where((x) => x.sublevel.GetComponent<LevelSystem>() == (LevelSystem)sender).FirstOrDefault();
        item.itemObject.SetSeen(true);
        item.itemObject.SetVisible(true);
    }

    public class GridLevelObject
    {
        public LevelMapSystem itemObject = null;
        public GameObject sublevel = null;
        public int columnIndex = 0;
        public int lineIndex = 0;
        public bool isStartArea = false;
        public bool isBossArea = false;
    }

    public class GenerationParameters
    {
        public GameObject startArea = null;
        public List<GameObject> sublevels = null;
        public GameObject bossArea = null;

        public int columns = 0;
        public int lines = 0;
        public int levelsToGenerate = 0;
    }

}
