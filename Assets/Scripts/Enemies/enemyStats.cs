using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class enemyStats : MonoBehaviour
{
    public float currentHP;
    public float maxHp;
    public float moveSpeed;
    public float ragDollTime;

    public roomEnemySpawner spawner;

    private void Update()
    {
        if (currentHP <= 0) die();
    }

    void die()
    {
        if (GetComponent<basicRangedAI>()) Destroy(GetComponent<basicRangedAI>());
        else if (GetComponent<meleeBruiserAI>()) Destroy(GetComponent<meleeBruiserAI>());
        spawner.enemiesRemaining.Remove(gameObject);

        StartCoroutine(ragDollEnemy(15f));
        Destroy(gameObject, 0f);
    }

    public void takeDamage(float damageToTake)
    {
        currentHP -= damageToTake;
        onDamageTaken?.Invoke();
    }

    public IEnumerator ragDollEnemy(float timeToRagdoll)
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;

        yield return new WaitForSeconds(timeToRagdoll);

        GetComponent<Animator>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    public UnityEvent onDamageTaken;
}
