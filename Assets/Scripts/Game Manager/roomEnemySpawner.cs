using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class roomEnemySpawner : MonoBehaviour
{
    [SerializeField] private int currentWaveIndex;
    [SerializeField] private List<wave> waves = new List<wave>();
    [SerializeField] bool spawnerActive = false, playerInRoom = false;
    [SerializeField] private float timeBetweenEnemySpawns;
    [SerializeField] private List<GameObject> enemiesRemaining = new List<GameObject>();


    private void Update()
    {
        if(playerInRoom && !spawnerActive && waves.Count <= currentWaveIndex-1)
        {
            if (waves[currentWaveIndex].noEnemiesRemaining && enemiesRemaining.Count <= 0 || !waves[currentWaveIndex].noEnemiesRemaining)
            {
                StartCoroutine(spawnWave());
            }
        }
    }

    private IEnumerator spawnWave()
    {
        spawnerActive = true;
        yield return new WaitForSeconds(waves[currentWaveIndex].delayBeforeWave);
        for (int i = 0; i < waves[currentWaveIndex].enemies.Count; i++)
        {
            enemiesRemaining.Add(Instantiate(waves[currentWaveIndex].enemies[i].enemyToSpawn, 
                waves[currentWaveIndex].enemies[i].spawnPoint.transform.position, 
                waves[currentWaveIndex].enemies[i].spawnPoint.transform.rotation, 
                GameObject.Find("EnemyHolder").transform));
            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }
        spawnerActive = false;
        currentWaveIndex++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) if(!playerInRoom) playerInRoom = true;

    }
}
[System.Serializable]
public struct wave
{
    public bool noEnemiesRemaining;
    public float delayBeforeWave;
    public List<enemyAndPos> enemies;
}

[System.Serializable]
public struct enemyAndPos
{
    public GameObject enemyToSpawn;
    public GameObject spawnPoint;
}
