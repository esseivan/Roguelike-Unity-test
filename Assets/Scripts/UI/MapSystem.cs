using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    [Required]
    public RectTransform sublevelGridmap = null;
    [Required]
    public RectTransform sublevelGridmapBorder = null;

    public bool IsMapOpen { get; private set; } = false;

    public void ToggleMap()
    {
        if (IsMapOpen)
            CloseMap();
        else
            OpenMap();
    }

    public void OpenMap()
    {
        if (IsMapOpen)
            return;

        IsMapOpen = true;

        // Set full screen
        sublevelGridmap.GetComponent<Fullscrenable>().SetFullscreen();
        sublevelGridmapBorder.GetComponent<Fullscrenable>().SetFullscreen();
    }

    public void CloseMap()
    {
        if (!IsMapOpen)
            return;

        IsMapOpen = false;

        // Unset full screen
        sublevelGridmap.GetComponent<Fullscrenable>().UnsetFullscreen();
        sublevelGridmapBorder.GetComponent<Fullscrenable>().UnsetFullscreen();
    }
}
