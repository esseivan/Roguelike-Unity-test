using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level size
/// </summary>
public class LevelSize : MonoBehaviour
{
    /// <summary>
    /// X Size
    /// </summary>
    public float SizeX { get; private set; }
    /// <summary>
    /// Y Size
    /// </summary>
    public float SizeY { get; private set; }
    /// <summary>
    /// Z Size
    /// </summary>
    public float SizeZ { get; private set; }

    // Set values on start only
    private void Start()
    {
        UpdateSizes();
    }

    public void UpdateSizes()
    {
        SizeX = transform.localScale.x;
        SizeY = transform.localScale.y;
        SizeZ = transform.localScale.z;
    }

    // Draw gizmos box
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
