using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detection of a player or ennemy fall
/// </summary>
[RequireComponent(typeof(Collider))]
public class EntryDetection : MonoBehaviour
{
    /// <summary>
    /// Event called when something is detected enterring this area
    /// </summary>
    public event EventHandler OnEnter = null;
    /// <summary>
    /// Event called when something is detected exitting this area
    /// </summary>
    public event EventHandler OnExit = null;

    private void OnTriggerEnter(Collider collider)
    {
        OnEnter?.Invoke(collider, EventArgs.Empty);
    }

    private void OnTriggerExit(Collider collider)
    {
        OnExit?.Invoke(collider, EventArgs.Empty);
    }
}
