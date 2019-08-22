using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyGenerationSystem : MonoBehaviour
{
    /// <summary>
    /// Ennemy to spawn
    /// </summary>
    [Required]
    public GameObject ennemy;

    /// <summary>
    /// Event called when all ennemies died
    /// </summary>
    public event EventHandler OnAllEnnemiesDied = null;

    /// <summary>
    /// Event called when one ennemy dies
    /// </summary>
    public event EventHandler OnEnnemyDied = null;

    /// <summary>
    /// List of living ennemies
    /// </summary>
    public List<GameObject> Ennemies { get; private set; } = null;

    /// <summary>
    /// Spawn all ennemies
    /// </summary>
    [Button]
    public void SpawnEnnemies()
    {
        Ennemies = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform spawnLocation = transform.GetChild(i);

            GameObject clone = Instantiate(ennemy, spawnLocation.transform);
            clone.GetComponent<EnnemySystem>().OnDied += EnnemyGenerationSystem_OnDied;
            Ennemies.Add(clone);
        }
    }

    /// <summary>
    /// Ennemy died event
    /// </summary>
    private void EnnemyGenerationSystem_OnDied(object sender, EventArgs e)
    {
        OnEnnemyDied?.Invoke(sender, e);

        Ennemies.Remove((GameObject)sender);

        if (Ennemies.Count == 0)
            OnAllEnnemiesDied?.Invoke(this.gameObject, EventArgs.Empty);
    }

    /// <summary>
    /// Kill all ennemies
    /// </summary>
    public void KillEnnemies()
    {
        while (Ennemies.Count != 0)
        {
            Ennemies[0].GetComponent<EnnemySystem>().Kill();
            Debug.Log(Ennemies.Count);
        }
    }
}
