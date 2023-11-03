using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class enemyStats : MonoBehaviour
{
    public float currentHP;
    public float maxHp;

    public float moveSpeed;

    private void Update()
    {

        if (currentHP <= 0) die();
    }

    void die()
    {
        Destroy(gameObject);
    }

    public void takeDamage(float damageToTake)
    {
        currentHP -= damageToTake;
        onDamageTaken?.Invoke();

        
    }

    public UnityEvent onDamageTaken;
}
