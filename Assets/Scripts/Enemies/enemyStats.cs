using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyStats : MonoBehaviour
{
    public float currentHP;
    public float maxHp;

    public float moveSpeed;

    public float fireDuration, oilDuration, stunDuration;
    public float fireTickDmg, oilPercentSlow;
    public statusEffects effectsOnEnemy;
    private float fireTickTime;
    private void Update()
    {
        if(fireTickTime < fireDuration && effectsOnEnemy == statusEffects.fire)
        {
            fireTickTime += Time.deltaTime;
            currentHP -= fireTickDmg;
        }
    }

    public void takeDamage(float damageToTake, statusEffects effectsToGive)
    {
        currentHP -= damageToTake;


        if (effectsOnEnemy == statusEffects.oil && effectsToGive == statusEffects.normal)
        {
            effectsOnEnemy = statusEffects.fire;
            fireTickTime = 0;
        }
        else if (effectsOnEnemy == statusEffects.fire && effectsToGive == statusEffects.oil) fireTickTime = 0;
        else if (effectsToGive == statusEffects.electric) StartCoroutine(stunEnemy());
    }

    IEnumerator stunEnemy()
    {
        //turn off AI
        //Play stun animation

        yield return new WaitForSeconds(stunDuration);

    }
}
