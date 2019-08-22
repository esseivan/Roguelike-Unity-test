using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class HealthBarEnnemy : MonoBehaviour
{
    public GameObject healthbarEnnemy = null;
    private HealthBarEnnemyManager createdHealthbar = null;
    private HealthSystem healthSystem = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj=  Instantiate(healthbarEnnemy, this.gameObject.transform);
        createdHealthbar = obj.GetComponent<HealthBarEnnemyManager>();
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float percent = (float)healthSystem.health / healthSystem.maxHealth;
        createdHealthbar.SetPercentage(percent);
    }
}
