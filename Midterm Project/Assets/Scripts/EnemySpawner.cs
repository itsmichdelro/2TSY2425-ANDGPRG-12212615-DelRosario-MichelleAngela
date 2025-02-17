using TMPro;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class LevelConfig
    {
        public int totalWaves = 3;
        public float waveDuration;
        public int baseEnemiesPerWave;
        public int enemyIncreasePerWave;
        public float spawnInterval;
        public int maxEnemiesAtOnce;
    }

    [Header("Level Configurations")]
    [SerializeField] private LevelConfig[] levelConfigs;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float topY = 6f;

    [Header("UI Settings")]
    private TextMeshProUGUI enemyCountText;
    private TextMeshProUGUI waveInfoText;

    private int currentLevel = 1;
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
        InitializeLevelConfigs();

        if (UIManager.Instance != null)
        {
            enemyCountText = UIManager.Instance.GetEnemyCountText();
            waveInfoText = UIManager.Instance.GetWaveInfoText();
        }
    }

    private void InitializeLevelConfigs()
    {
        levelConfigs = new LevelConfig[3];

        // Level 1 Configuration
        levelConfigs[0] = new LevelConfig
        {
            totalWaves = 3,
            waveDuration = 15f,
            baseEnemiesPerWave = 15,
            enemyIncreasePerWave = 5,
            spawnInterval = 0.5f,
            maxEnemiesAtOnce = 20
        };

        // Level 2 Configuration
        levelConfigs[1] = new LevelConfig
        {
            totalWaves = 3,
            waveDuration = 20f,
            baseEnemiesPerWave = 15,
            enemyIncreasePerWave = 10,
            spawnInterval = 0.5f,
            maxEnemiesAtOnce = 30
        };

        // Level 3 Configuration
        levelConfigs[2] = new LevelConfig
        {
            totalWaves = 3,
            waveDuration = 30f,
            baseEnemiesPerWave = 15,
            enemyIncreasePerWave = 15,
            spawnInterval = 0.5f,
            maxEnemiesAtOnce = 50
        };
    }

    public void ResetSpawner()
    {
        currentWave = 0;
        enemiesSpawnedThisWave = 0;
        activeEnemyCount = 0;
        isWaveActive = false;
        levelCompleted = false;

        // Reset UI
        UpdateEnemyCountUI(0);
        if (waveInfoText != null)
        {
            waveInfoText.text = $"Wave 1/{levelConfigs[currentLevel - 1].totalWaves}";
        }
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

        ResetSpawner();
        StartNextWave();
    }

    private void ShowLevelComplete()
    {
        if (levelCompleted) return;

        Debug.Log($"Level {currentLevel} completed!");
        levelCompleted = true;
        isWaveActive = false;

        UpdateEnemyCountUI(0);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLevelComplete();
        }
    }

    private void UpdateEnemyCountUI(int count)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnemyCount(count);
        }
    }

    void Update()
    {
        if (levelCompleted) return;

        LevelConfig currentConfig = levelConfigs[currentLevel - 1];
        int remainingToSpawn = totalEnemiesThisWave - enemiesSpawnedThisWave;
        int totalRemaining = activeEnemyCount + remainingToSpawn;

        UpdateEnemyCountUI(totalRemaining);

        // Check for wave completion conditions
        if (currentWave > currentConfig.totalWaves && activeEnemyCount == 0)
        {
            ShowLevelComplete();
            return;
        }

        if (!isWaveActive || currentWave > currentConfig.totalWaves) return;

        float timeRemaining = Mathf.Max(0, (waveStartTime + currentConfig.waveDuration) - Time.time);

        // Update wave info UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWaveInfo(currentWave, currentConfig.totalWaves, timeRemaining);
        }

        // Check spawn conditions
        bool canSpawnMore = enemiesSpawnedThisWave < totalEnemiesThisWave;
        bool belowMaxEnemies = activeEnemyCount < currentConfig.maxEnemiesAtOnce;
        bool timeToSpawn = Time.time >= nextSpawnTime;

        if (canSpawnMore && belowMaxEnemies && timeToSpawn)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + currentConfig.spawnInterval;
        }

        // Check if wave time is up
        if (timeRemaining <= 0)
        {
            if (UIManager.Instance != null && activeEnemyCount > 0)
            {
                UIManager.Instance.ShowGameOver();
                isWaveActive = false;
                return;
            }
            StartNextWave();
        }
    }

    void StartNextWave()
    {
        currentWave++;
        LevelConfig currentConfig = levelConfigs[currentLevel - 1];

        if (currentWave <= currentConfig.totalWaves)
        {
            waveStartTime = Time.time;
            enemiesSpawnedThisWave = 0;
            totalEnemiesThisWave = currentConfig.baseEnemiesPerWave +
                                 (currentWave - 1) * currentConfig.enemyIncreasePerWave;
            nextSpawnTime = Time.time;
            isWaveActive = true;
            Debug.Log($"Starting Wave {currentWave} of Level {currentLevel}! " +
                     $"Total enemies this wave: {totalEnemiesThisWave}");
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

        Vector2 spawnPosition = new Vector2(
            Random.Range(minX, maxX),
            topY
        );

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.tag = "Enemy";
        enemiesSpawnedThisWave++;
        activeEnemyCount++;

        Debug.Log($"Spawned enemy {enemiesSpawnedThisWave} of {totalEnemiesThisWave}. " +
                 $"Active enemies: {activeEnemyCount}");
    }

    public void EnemyKilled()
    {
        activeEnemyCount--;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.IncrementScore();
        }

        Debug.Log($"Enemy killed. Active enemies remaining: {activeEnemyCount}");

        LevelConfig currentConfig = levelConfigs[currentLevel - 1];

        // Check completion conditions
        if (currentWave > currentConfig.totalWaves && activeEnemyCount == 0)
        {
            ShowLevelComplete();
        }
        else if (activeEnemyCount == 0 &&
                 enemiesSpawnedThisWave >= totalEnemiesThisWave &&
                 currentWave <= currentConfig.totalWaves)
        {
            StartNextWave();
        }
    }

    public void SetLevel(int level)
    {
        if (level >= 1 && level <= levelConfigs.Length)
        {
            currentLevel = level;
            ResetSpawner();
            StartNextWave();
        }
        else
        {
            Debug.LogError($"Invalid level number: {level}");
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}