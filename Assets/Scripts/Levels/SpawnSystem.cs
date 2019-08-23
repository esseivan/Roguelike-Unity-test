using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn location system
/// </summary>
public class SpawnSystem : MonoBehaviour
{
    /// <summary>
    /// Gizmos display type
    /// </summary>
    public DisplayType gizmoDisplayType = DisplayType.Sphere;
    /// <summary>
    /// Gizmos display radius
    /// </summary>
    public float radius = 0.5f;

    /// <summary>
    /// Gizmos display type
    /// </summary>
    public enum DisplayType
    {
        NONE,
        Cube,
        Sphere
    }

    // Draw a gizmos to show the location
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        switch (gizmoDisplayType)
        {
            case DisplayType.Cube:
                Gizmos.DrawWireCube(transform.position, Vector3.one * 2 * radius);
                break;
            case DisplayType.Sphere:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;
            default:
                break;
        }
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2 * radius);
    }
}
