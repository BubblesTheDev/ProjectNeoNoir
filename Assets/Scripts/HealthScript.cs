using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Maximum health value of unit")]
    private int maxHealth;

    [SerializeField]
    [Tooltip("Current health value of unit. Should not exceed max health")]
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onTakeDamage(int dmg)
    {
        currentHealth -= dmg;
    }

    void onHeal(int health) 
    {
        currentHealth += health;
    }
}
