using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage a teleporter
/// </summary>
public class TeleporterSystem : MonoBehaviour
{
    /// <summary>
    /// Whether the teleporter is enabled
    /// </summary>
    public bool IsTeleporterEnabled { get; private set; } = false;

    public bool IsPlayerHere { get; set; } = false;

    /// <summary>
    /// The particles system
    /// </summary>
    public ParticleSystem particles = null;

    /// <summary>
    /// The destination
    /// </summary>
    public GameObject destination = null;
    [DisableIf("destination")]
    public bool isBossTeleporter = false;

    public event EventHandler OnLevelComplete = null;

    private Collider currentCollider = null;

    public void SetLevelComplete(bool isLevelComplete, int levelIndex)
    {
        if (particles != null && destination != null)
        {
            var main = particles.main;
            if (!isLevelComplete)
                main.startColor = new ParticleSystem.MinMaxGradient(Color.white);
            else
            {
                // Check if destination is greater or not
                if (destination.GetComponent<LevelSystem>().levelIndex > levelIndex)
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.green);
                else
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.red);
            }
        }
    }

    /// <summary>
    /// Enable the teleporter
    /// </summary>
    public void EnableTeleporter()
    {
        if (IsTeleporterEnabled)
            return;

        // Enable particles
        if (particles != null)
            GetComponent<ParticleSystem>().Play();

        IsTeleporterEnabled = true;

        // Check if collider is already on the teleporter
        if(currentCollider != null)
        {
            OnPlayerEnter(currentCollider);
        }
    }

    /// <summary>
    /// Disable the teleporter
    /// </summary>
    public void DisableTeleporter()
    {
        if (!IsTeleporterEnabled)
            return;

        // Disable particles
        if (particles != null)
            GetComponent<ParticleSystem>().Stop();

        IsTeleporterEnabled = false;
    }

    // Called on collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            currentCollider = collider;
            if (!IsTeleporterEnabled)
                return;

            OnPlayerEnter(collider);
        }
    }

    private void OnPlayerEnter(Collider collider)
    {
        if (isBossTeleporter)
        {
            OnLevelComplete?.Invoke(this, EventArgs.Empty);
        }
        if (destination != null)
        {
            // Teleport player
            collider.GetComponent<PlayerSystem>().SetLevel(destination);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider == currentCollider)
            currentCollider = null;
    }

    // Draw a gizmos line to the destination (yellow if the player is in this level)
    private void OnDrawGizmos()
    {
        if (destination == null)
            return;

        if (IsPlayerHere)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, destination.GetComponent<LevelSystem>().teleporterEntry.transform.position);
    }
}
