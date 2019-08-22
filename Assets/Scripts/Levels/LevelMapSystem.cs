using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapSystem : MonoBehaviour
{
    public RawImage borderImage = null;
    public Text textObject = null;

    public void SetVisible(bool state)
    {
        borderImage.enabled = state;
        // Keep text enabled
        textObject.enabled = true;
    }

    public void SetSeen(bool mode)
    {
        borderImage.color = mode ? Color.green : Color.white;
    }

    public void SetText(LevelSystem.SublevelType sublevelType)
    {
        string text = "?";
        Color color = Color.white;

        switch (sublevelType)
        {
            case LevelSystem.SublevelType.StartArea:
                text = "S";
                break;
            case LevelSystem.SublevelType.Ennemy:
                text = "X";
                color = Color.red;
                break;
            case LevelSystem.SublevelType.Loot:
                text = "L";
                color = Color.cyan;
                break;
            case LevelSystem.SublevelType.BossArea:
                text = "B";
                color = Color.red;
                break;
            default:
                break;
        }
        textObject.text = text;
        textObject.color = color;
    }
}
