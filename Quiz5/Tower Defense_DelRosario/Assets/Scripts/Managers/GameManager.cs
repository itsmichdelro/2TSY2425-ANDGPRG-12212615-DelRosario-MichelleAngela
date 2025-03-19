using UnityEngine;

public enum WaveType
{
    Normal, Boss
}

public class GameManager : MonoBehaviour // handles start, middle, end of game
{
    public static GameManager Instance; // for instancing, creates its own instance on runtime
    [SerializeField] GameObject crystalCore;

    [Header("Wave")]
    private WaveType waveType;
    private int waveNumber = 1;
    public bool isWaveStart = false;

    public float waveInterval = 6f;
    private float countdown = 6f;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnerController.enemiesAlive > 0) return;

        if (countdown <= 0f)
        {
            SetWaveType();
            StartWave();
            countdown = waveInterval;
            return;
        }

        // Ends the current wave
        if (SpawnerController.enemiesAlive == 0)
        {
            if (isWaveStart == true)
            {
                isWaveStart = false;
                EndWave();
                return;
            }
        }

        countdown -= Time.deltaTime;

        if (waveNumber > 5 && PlayerManager.Instance.GetCurrentHealth > 0)
        {
            UIHandler.Instance.ShowEndGamePanel("winPanel");
        }
        else return;
    }

    private void SetWaveType()
    {
        if (waveNumber % 5 == 0) // only if divisible by 5
        {
            waveType = WaveType.Boss;
        }
        else
        {
            waveType = WaveType.Normal;
        }
    }

    private void StartWave()
    {
        isWaveStart = true;

        if (waveType == WaveType.Normal)
        {
            if (waveNumber < 5)
            {
                SpawnerController.Instance.SpawnEasyWave(Random.Range(1,2));
            }
            else SpawnerController.Instance.SpawnMediumWave(Random.Range(1, 2));
        }
        if (waveType == WaveType.Boss)
        {
            SpawnerController.Instance.SpawnBossWave();
        }
    }

    private void EndWave()
    {
        Debug.Log("wave " + waveNumber.ToString() + " end. wave interval start");

        waveNumber++;
        countdown = waveInterval;

        // Clear enemy list at wave end
        if (SpawnerController.Instance.GetEnemyList().Count > 0) 
        { 
            SpawnerController.Instance.enemyList.Clear(); 
        }
        else return;
    }

    #region Getters, Setters
    public GameObject CrystalCore { get { return crystalCore; } }
    public WaveType GetWaveType { get { return waveType; } }
    public int GetWaveNumber { get { return waveNumber; } }
    #endregion
}
