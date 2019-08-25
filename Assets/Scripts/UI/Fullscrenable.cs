using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Fullscrenable : MonoBehaviour
{
    private RectSettings saved;
    private RectSettings fullscreen = new RectSettings()
    {
        anchorMin = new Vector2(0, 0),
        anchorMax = new Vector2(1, 1),
        offsetMax = new Vector2(0, 0),
        offsetMin = new Vector2(0, 0),
    };

    private bool isFullScreen = false;
    private RectTransform rect = null;

    public Mode fullScreenMode = Mode.Resize;

    public enum Mode
    {
        Resize,
        Extend
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetFullscreen()
    {
        if (isFullScreen)
            return;

        isFullScreen = true;
        saved = new RectSettings(rect);
        if (fullScreenMode == Mode.Resize)
        {
            int maxWidth = Screen.width;
            int maxHeight = Screen.height;

            float factorWidth = maxWidth / rect.sizeDelta.x;
            float factorHeight = maxHeight / rect.sizeDelta.y;

            if (factorWidth <= factorHeight)
                fullscreen.scale = saved.scale * factorWidth;
            else
                fullscreen.scale = saved.scale * factorHeight;


            fullscreen.ApplyScale(rect);
        }
        fullscreen.ApplySettings(rect);
    }

    public void UnsetFullscreen()
    {
        if (!isFullScreen)
            return;

        isFullScreen = false;
        saved.ApplySettings(rect);
        saved.ApplyScale(rect);
    }

    private void ApplySettings(RectSettings settings)
    {
        settings.ApplySettings(rect);
    }

    private class RectSettings
    {
        public Vector2 anchorMin, anchorMax;
        public Vector2 offsetMin, offsetMax;
        public Vector2 scale;

        public RectSettings() { }

        public RectSettings(RectTransform rectTransform)
        {
            anchorMin = rectTransform.anchorMin;
            anchorMax = rectTransform.anchorMax;
            offsetMin = rectTransform.offsetMin;
            offsetMax = rectTransform.offsetMax;
            scale = rectTransform.localScale;
        }

        public void ApplySettings(RectTransform rectTransform)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }

        public void ApplyScale(RectTransform rectTransform)
        {
            rectTransform.localScale = scale;
        }
    }
}
