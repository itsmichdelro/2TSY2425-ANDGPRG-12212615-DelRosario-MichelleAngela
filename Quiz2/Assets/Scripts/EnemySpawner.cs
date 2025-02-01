using TMPro;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Wave Settings")]
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private int baseEnemiesPerWave = 50;
    [SerializeField] private int enemyIncreasePerWave = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int maxEnemiesAtOnce = 10;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float topZ = 10f;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI waveInfoText;

    private int currentWave = 0;
    private float waveStartTime;
    private float nextSpawnTime;
    private int enemiesSpawnedThisWave = 0;
    private int totalEnemiesThisWave;
    private int remainingEnemies;
    private GameObject playerReference;
    private bool isWaveActive = false;

    public static WaveEnemySpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned!");
            enabled = false;
            return;
        }
        playerReference = GameObject.FindGameObjectWithTag("Player");
        if (playerReference == null)
        {
            Debug.LogError("No Player found!");
            enabled = false;
            return;
        }
        isWaveActive = true;
        StartNextWave();
    }

    void Update()
    {
        if (!isWaveActive || currentWave > totalWaves) return;

        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Left: {remainingEnemies}";
        }

        float timeRemaining = Mathf.Max(0, (waveStartTime + waveDuration) - Time.time);
        if (waveInfoText != null)
        {
            waveInfoText.text = $"Wave {currentWave}/{totalWaves}\n" +
                                $"Time: {timeRemaining:F1}s";
        }

        bool canSpawnMore = enemiesSpawnedThisWave < totalEnemiesThisWave;
        bool belowMaxEnemies = currentEnemies < maxEnemiesAtOnce;
        bool timeToSpawn = Time.time >= nextSpawnTime;

        if (canSpawnMore && belowMaxEnemies && timeToSpawn)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        if (timeRemaining <= 0)
        {
            StartNextWave();
        }
    }

    void StartNextWave()
    {
        currentWave++;
        if (currentWave <= totalWaves)
        {
            waveStartTime = Time.time;
            enemiesSpawnedThisWave = 0;
            totalEnemiesThisWave = baseEnemiesPerWave + (currentWave - 1) * enemyIncreasePerWave;
            remainingEnemies = totalEnemiesThisWave;
            nextSpawnTime = Time.time;
            isWaveActive = true;
            Debug.Log($"Starting Wave {currentWave}! Enemies: {totalEnemiesThisWave}");
        }
        else
        {
            isWaveActive = false;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || playerReference == null) return;
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            0f,
            topZ
        );
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemiesSpawnedThisWave++;
    }

    public void EnemyKilled()
    {
        if (remainingEnemies > 0)
        {
            remainingEnemies--;
            Debug.Log($"Enemy killed. Remaining enemies: {remainingEnemies}"); // Debug line to track count
        }

        if (remainingEnemies <= 0)
        {
            if (currentWave >= totalWaves)
            {
                isWaveActive = false;
            }
            else
            {
                StartNextWave();
            }
        }
    }
}