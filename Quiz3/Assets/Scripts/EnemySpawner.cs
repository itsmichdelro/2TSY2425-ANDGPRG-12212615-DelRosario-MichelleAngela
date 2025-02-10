using TMPro;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Wave Settings")]
    [SerializeField] private int totalWaves = 3;
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] private int baseEnemiesPerWave = 15;
    [SerializeField] private int enemyIncreasePerWave = 5;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int maxEnemiesAtOnce = 15;

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
    private int activeEnemyCount = 0;
    private GameObject playerReference;
    private bool isWaveActive = false;
    private bool levelCompleted = false;

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

        // Initialize first wave
        StartNextWave();
    }

    private void ShowLevelComplete()
    {
        if (levelCompleted) return;

        Debug.Log("Showing level complete screen");
        levelCompleted = true;
        isWaveActive = false;

        UpdateEnemyCountUI(0);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowLevelComplete();
        }
    }

    private void UpdateEnemyCountUI(int count)
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Left: {count}";
        }
    }

    void Update()
    {
        if (levelCompleted) return;

        int remainingToSpawn = totalEnemiesThisWave - enemiesSpawnedThisWave;
        int totalRemaining = activeEnemyCount + remainingToSpawn;

        UpdateEnemyCountUI(totalRemaining);

        // Check for wave completion conditions
        if (currentWave > totalWaves && activeEnemyCount == 0)
        {
            ShowLevelComplete();
            return;
        }

        if (!isWaveActive || currentWave > totalWaves) return;

        float timeRemaining = Mathf.Max(0, (waveStartTime + waveDuration) - Time.time);

        // Update wave info UI
        if (waveInfoText != null)
        {
            waveInfoText.text = $"Wave {currentWave}/{totalWaves}\n" +
                               $"Time: {timeRemaining:F1}s";
        }

        // Check spawn conditions
        bool canSpawnMore = enemiesSpawnedThisWave < totalEnemiesThisWave;
        bool belowMaxEnemies = activeEnemyCount < maxEnemiesAtOnce;
        bool timeToSpawn = Time.time >= nextSpawnTime;

        if (canSpawnMore && belowMaxEnemies && timeToSpawn)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Check if wave time is up
        if (timeRemaining <= 0)
        {
            if (GameManager.Instance != null && activeEnemyCount > 0)
            {
                GameManager.Instance.ShowGameOver();
                isWaveActive = false;
                return;
            }
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
            nextSpawnTime = Time.time;
            isWaveActive = true;
            Debug.Log($"Starting Wave {currentWave}! Total enemies this wave: {totalEnemiesThisWave}");
        }
        else
        {
            isWaveActive = false;
            Debug.Log("Final wave completed, waiting for remaining enemies");
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
        enemy.tag = "Enemy";
        enemiesSpawnedThisWave++;
        activeEnemyCount++;

        Debug.Log($"Spawned enemy {enemiesSpawnedThisWave} of {totalEnemiesThisWave}. Active enemies: {activeEnemyCount}");
    }

    public void EnemyKilled()
    {
        activeEnemyCount--;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncrementScore();
        }

        Debug.Log($"Enemy killed. Active enemies remaining: {activeEnemyCount}");

        // Check completion conditions
        if (currentWave > totalWaves && activeEnemyCount == 0)
        {
            ShowLevelComplete();
        }
        else if (activeEnemyCount == 0 && enemiesSpawnedThisWave >= totalEnemiesThisWave && currentWave <= totalWaves)
        {
            StartNextWave();
        }
    }
}