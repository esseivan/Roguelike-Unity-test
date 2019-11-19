using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderSystem : MonoBehaviour
{
    [ShowInInspector]
    public float Percentage { get; private set; } = 1;

    public bool StickRight = true;

    public float defaultPos = 0;
    public float deltaPos = 0.5f;

    public float defaultSize = 1;
    public float deltaSize = 1;

    public float GetPercentage()
    {
        return Percentage;
    }

    public void SetPercentage(float percentage)
    {
        if (this.Percentage == percentage)
            return;

        this.Percentage = percentage;

        UpdateControl();
    }

    [Button]
    public void UpdateControl()
    {
        if (Percentage < 0)
            Percentage = 0;
        else if (Percentage > 1)
            Percentage = 1;

        Vector3 scale = transform.localScale;
        Vector3 pos = transform.localPosition;

        scale.x = defaultSize - deltaSize * (1 - Percentage);
        pos.x = defaultPos - deltaPos * (1 - Percentage);
        if (StickRight)
            pos.x = -pos.x;

        transform.localScale = scale;
        transform.localPosition = pos;
    }

    [Button]
    public void SetDefaults()
    {
        defaultPos = transform.localPosition.x;
        defaultSize = transform.localScale.x;
    }
    [Button]
    public void SetDeltas()
    {
        deltaPos = Mathf.Abs(transform.localPosition.x - defaultPos);
        deltaSize = Mathf.Abs(transform.localScale.x - defaultSize);
    }
}
