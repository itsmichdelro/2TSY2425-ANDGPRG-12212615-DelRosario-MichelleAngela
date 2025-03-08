using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyGroup
{
    public string enemyName = "Goblin"; 
    public GameObject enemyPrefab;
    public int count = 5;
    public float delayBetweenSpawns = 1.0f;
}

[System.Serializable]
public class CustomWave
{
    public string waveName = "Wave 1";
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    public bool hasBoss = false;
    public GameObject bossPrefab;
    public float healthMultiplier = 1.0f;
    public float goldMultiplier = 1.0f;
    public float timeBetweenGroups = 2.0f;
}

public class SpawnerController : MonoBehaviour
{
    public static SpawnerController Instance;

    [Header("Wave Configuration")]
    [SerializeField] List<CustomWave> waves = new List<CustomWave>();
    [SerializeField] int currentWave = 0;
    [SerializeField] bool autoStartWaves = true;

    [Header("Default Enemy Prefabs")]
    [SerializeField] GameObject defaultGroundEnemyPrefab;
    [SerializeField] GameObject defaultFlyingEnemyPrefab;
    [SerializeField] GameObject defaultBossPrefab;

    [Header("Spawn Settings")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float defaultTimeBetweenSpawns = 1.5f;

    [Header("Tracking")]
    [SerializeField] List<Enemy> enemyList = new List<Enemy>();

    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
       
        if (waves.Count == 0)
        {
            GenerateDefaultWaves(10);
        }

        if (autoStartWaves)
            StartCoroutine(StartNextWave());
    }

    private void GenerateDefaultWaves(int waveCount)
    {
        waves.Clear();

        for (int i = 0; i < waveCount; i++)
        {
            CustomWave wave = new CustomWave();
            wave.waveName = "Wave " + (i + 1);
            wave.healthMultiplier = 1.0f + (i * 0.15f);
            wave.goldMultiplier = 1.0f + (i * 0.1f);

            // Add ground enemies to all waves
            EnemyGroup groundGroup = new EnemyGroup();
            groundGroup.enemyName = "Goblin";
            groundGroup.enemyPrefab = defaultGroundEnemyPrefab;
            groundGroup.count = 5 + i;
            groundGroup.delayBetweenSpawns = Mathf.Max(0.5f, 1.5f - (i * 0.05f));
            wave.enemyGroups.Add(groundGroup);

            // Add flying enemies starting from wave 3
            if (i >= 2)
            {
                EnemyGroup flyingGroup = new EnemyGroup();
                flyingGroup.enemyName = "Raven";
                flyingGroup.enemyPrefab = defaultFlyingEnemyPrefab;
                flyingGroup.count = i - 1;
                flyingGroup.delayBetweenSpawns = Mathf.Max(0.7f, 1.8f - (i * 0.05f));
                wave.enemyGroups.Add(flyingGroup);
            }

            // Add boss every 5th wave and final wave
            wave.hasBoss = (i + 1) % 5 == 0 || i == waveCount - 1;
            wave.bossPrefab = defaultBossPrefab;

            waves.Add(wave);
        }
    }

    public IEnumerator StartNextWave()
    {
        // Wait before starting wave
        yield return new WaitForSeconds(3f);

        if (currentWave < waves.Count)
        {
            // Update UI to show current wave
            GameManager.Instance.UpdateWaveUI(currentWave + 1);

            CustomWave wave = waves[currentWave];
            isSpawning = true;

            Debug.Log("Starting " + wave.waveName);

            // Spawn each group of enemies
            foreach (EnemyGroup group in wave.enemyGroups)
            {
                if (group.enemyPrefab != null)
                {
                    for (int i = 0; i < group.count; i++)
                    {
                        SpawnEnemy(group.enemyPrefab, GetRandomSpawnPoint(),
                            wave.healthMultiplier, wave.goldMultiplier);

                        yield return new WaitForSeconds(group.delayBetweenSpawns);
                    }
                }

                // Pause between different enemy groups
                yield return new WaitForSeconds(wave.timeBetweenGroups);
            }

            // Spawn boss if this wave has one
            if (wave.hasBoss && wave.bossPrefab != null)
            {
                SpawnEnemy(wave.bossPrefab, GetRandomSpawnPoint(),
                    wave.healthMultiplier * 2.0f, wave.goldMultiplier * 3.0f);
            }

            isSpawning = false;
            currentWave++;
        }
        else
        {
            // All waves completed
            GameManager.Instance.GameWon();
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0)
            return transform;

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    private void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint, float healthMultiplier, float goldMultiplier)
    {
        Vector3 spawnPosition = spawnPoint.position;

        // For ground enemies, ensure they spawn on NavMesh
        NavMeshHit hit;
        Enemy enemyComponent = enemyPrefab.GetComponent<Enemy>();
        bool isFlying = enemyComponent != null && enemyComponent.GetMonsterType() == MonsterType.Flying;

        if (!isFlying)
        {
            if (NavMesh.SamplePosition(spawnPosition, out hit, 10f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }
            else
            {
                Debug.LogWarning("Could not find NavMesh near spawn point. Using original position.");
            }
        }

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetupEnemy(healthMultiplier, goldMultiplier);
            enemy.SetTarget(GameManager.Instance.CrystalCore.transform);
            enemyList.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);

        // Check if wave is complete
        if (enemyList.Count == 0 && !isSpawning)
        {
            // Give wave completion bonus
            GameManager.Instance.AddGold(50 + currentWave * 10);

            // Start next wave
            StartCoroutine(StartNextWave());
        }
    }

    public List<Enemy> GetEnemies()
    {
        return enemyList;
    }
}