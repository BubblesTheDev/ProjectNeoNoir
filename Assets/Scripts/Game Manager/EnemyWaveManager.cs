using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class EnemyWaveManager : MonoBehaviour
{
    public List<waveStructure> waves;
    [Space, Header("Wave stats")]
    [SerializeField] private float waveIncMulti;
    public int currentWave { private set; get; }
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float timeToForceSwitchWaves;
    [SerializeField] private float minTimeBetweenSpawns, maxTimeBetweenSpawns;
    [SerializeField] private List<GameObject> enemiesLeft;
    [SerializeField] private int enemiesLeftThreshold;

    [Space, Header("UI Stuffs")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI roundCounter;


    private void Awake()
    {
        if (waves.Count == 0) Debug.LogWarning("There is currenlty no waves created, the wave spawner will not work");
        else startWave();
    }

    IEnumerator startWave()
    {

        roundCounter.enabled = true;
        for (int i = 1; i < timeBetweenWaves+1; i++)
        {
            roundCounter.text = ("Round: " + currentWave.ToString() + " \n <b>" + i + "</b>");
            yield return new WaitForSeconds(1f);
        }

        roundCounter.text = ("<b>Round Start!</b>");
        yield return new WaitForSeconds(0.6f);
        roundCounter.enabled = false;

        foreach (enemyPerWave Wave in waves[currentWave].enemiesInTheWave)
        {
            GameObject tempObj = Instantiate(Wave.enemyType, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity, GameObject.Find("EnemyHolder").transform);
            enemiesLeft.Add(tempObj);

            yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
        }
    }



    private void LateUpdate()
    {
        if (enemiesLeft.Count < enemiesLeftThreshold)
        {
            if (enemiesRemainingText.enabled == false) enemiesRemainingText.enabled = true;
            enemiesRemainingText.text = "Enemies Remaining: " + enemiesLeft.Count.ToString();
        }
        else enemiesRemainingText.enabled = false;

        if (enemiesLeft.Count == 0 && waves.Count >= currentWave) StartCoroutine(startWave());
    }
}

[System.Serializable]
public struct waveStructure
{
    public List<enemyPerWave> enemiesInTheWave;
}

[System.Serializable]
public struct enemyPerWave
{
    public GameObject enemyType;
    public int numOfEnemies;
}
