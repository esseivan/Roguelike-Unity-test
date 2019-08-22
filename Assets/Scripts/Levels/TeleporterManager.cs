using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleporter manager
/// </summary>
public class TeleporterManager : MonoBehaviour
{
    /// <summary>
    /// Whether the child teleporter are enabled
    /// </summary>
    public bool TeleporterEnabled { get; private set; }

    /// <summary>
    /// The list of child teleporters
    /// </summary>
    private List<GameObject> teleporterList = null;

    private void Start()
    {
        teleporterList = new List<GameObject>();
        // Populate the teleporter list
        int tpCount = transform.childCount;
        for (int i = 0; i < tpCount; i++)
        {
            GameObject tp = transform.GetChild(i).gameObject;
            teleporterList.Add(tp);
        }
    }

    public void SetLevelComplete(bool isLevelComplete, int levelIndex)
    {
        foreach (GameObject teleporter in teleporterList)
        {
            // Disable teleporter
            TeleporterSystem ts = teleporter.GetComponent<TeleporterSystem>();
            ts.SetLevelComplete(isLevelComplete, levelIndex);
        }
    }

    public void SetPlayerHere(bool isPlayerHere)
    {
        foreach (GameObject teleporter in teleporterList)
        {
            // Disable teleporter
            TeleporterSystem ts = teleporter.GetComponent<TeleporterSystem>();
            ts.IsPlayerHere = isPlayerHere;
        }
    }

    /// <summary>
    /// Disable child teleporters
    /// </summary>
    public void DisableTeleporters()
    {
        if (!TeleporterEnabled)
            return;

        if (teleporterList == null)
            return;

        TeleporterEnabled = false;
        foreach (GameObject teleporter in teleporterList)
        {
            // Disable teleporter
            TeleporterSystem ts = teleporter.GetComponent<TeleporterSystem>();
            ts.DisableTeleporter();
        }
    }

    /// <summary>
    /// Enable child teleporters
    /// </summary>
    public void EnableTeleporters()
    {
        if (TeleporterEnabled)
            return;

        if (teleporterList == null)
            return;

        TeleporterEnabled = true;
        foreach (GameObject teleporter in teleporterList)
        {
            // Enable teleporter
            TeleporterSystem ts = teleporter.GetComponent<TeleporterSystem>();
            ts.EnableTeleporter();
        }
    }
}
