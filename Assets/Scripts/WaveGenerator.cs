using UnityEngine;
using System.Collections;
using TMPro;

public class WaveGenerator : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public ParticleSystem spawnEffect;
    public GameObject EvolutionUI;
    public TextMeshProUGUI WaveInfoText;
    [Header("Wave Settings")]
    public int baseEnemiesPerWave = 5;
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;

    private int currentWave = 1;
    private bool isSpawning = false;
    [HideInInspector]
    public static float potentialEnemies = 0;

    private void Start()
    {
        ShowEvolutionToStartNextWave();
    }
    private void ShowEvolutionToStartNextWave()
    {
        EvolutionUI.SetActive(true);
        WaveInfoText.text = "Wave " + currentWave;
    }
    public void StartNextWave()
    {
        if (!isSpawning)
            StartCoroutine(SpawnWave());
    }
    private void Update()
    {        
        //After 1st Wave ends, seemless continuation
            if (potentialEnemies == 0 && finishedWave)
            {
                currentWave++;
                ShowEvolutionToStartNextWave();
                finishedWave = false;

        }


    }
    bool finishedWave;
    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        int enemiesToSpawn = baseEnemiesPerWave + (currentWave-1);
        potentialEnemies = enemiesToSpawn;
        int spawned = 0;

        bool first = true;
        Debug.Log($"--- Starting Wave {currentWave} ({enemiesToSpawn} enemies) ---");

        while (spawned < enemiesToSpawn)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Play particle effect at spawn
            if (spawnEffect != null)
            {
                ParticleSystem fx = Instantiate(spawnEffect, spawnPoint.position, Quaternion.identity);
                Destroy(fx.gameObject, 2f);
            }
            if (first)
            {
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Spawned an Enemy");
                spawned++;
                first = false;
            }
            if (!first)
            {
                // Wait random delay before spawning enemy
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

                // Spawn enemy
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("Spawned an Enemy");

                spawned++;
            }

            isSpawning = false;
            Debug.Log($"--- Wave {currentWave} complete ---");
            finishedWave = true;
            
        }
    }
}
