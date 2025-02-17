using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject soundPanel;
    public GameObject pausePanel;
    public GameObject mainMenuPanel;
    public GameObject gameOverScreen;
    public GameObject levelCompleteScreen;  // Added level complete screen

    [Header("Persistent UI")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [SerializeField] private TextMeshProUGUI levelText;

    private int enemiesKilled = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePanels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePanels()
    {
        if (soundPanel != null) soundPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        // Initialize level text
        UpdateLevelText(1);
    }

    public void IncrementScore()
    {
        if (!isGameOver)
        {
            enemiesKilled++;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("EnemyDeath");
            }
            Debug.Log($"Enemies killed: {enemiesKilled}");
        }
    }

    public int GetScore()
    {
        return enemiesKilled;
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Showing Game Over Screen");
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            GameOverScreen gameOverComponent = gameOverScreen.GetComponent<GameOverScreen>();
            if (gameOverComponent != null)
            {
                gameOverComponent.Setup(enemiesKilled);
            }
        }
        else
        {
            Debug.LogError("GameOverScreen is not assigned in UIManager!");
        }
    }
    public void ShowLevelComplete()
    {
        if (isGameOver) return;

        if (levelCompleteScreen != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("LevelComplete");
            }
            levelCompleteScreen.SetActive(true);
            LevelCompleteScreen levelCompleteComponent = levelCompleteScreen.GetComponent<LevelCompleteScreen>();
            if (levelCompleteComponent != null)
            {
                levelCompleteComponent.Setup(enemiesKilled);
            }
        }
        else
        {
            Debug.LogError("LevelCompleteScreen is not assigned in UIManager!");
        }
    }

    public HealthBar GetHealthBar()
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not assigned in UIManager!");
            // Try to find it in the scene if not assigned
            healthBar = FindObjectOfType<HealthBar>();
        }
        return healthBar;
    }
    
    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }

    public void ResetAllUI()
    {
        // Reset game state
        isGameOver = false;
        enemiesKilled = 0;

        // Reset health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(100);
            healthBar.SetHealth(100);
        }

        // Reset enemy count
        UpdateEnemyCount(0);

        // Reset wave info
        UpdateWaveInfo(1, 3, 10f);

        
        

        // Hide screens
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(false);
        }
    }

    public void UpdateEnemyCount(int count)
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies Left: {count}";
        }
    }

    public void UpdateWaveInfo(int currentWave, int totalWaves, float timeRemaining)
    {
        if (waveInfoText != null)
        {
            waveInfoText.text = $"Wave {currentWave}/{totalWaves}\n" +
                               $"Time: {timeRemaining:F1}s";
        }
    }

    public void ShowSoundPanel()
    {
        if (soundPanel != null)
        {
            soundPanel.SetActive(true);
        }
    }

    public void HideSoundPanel()
    {
        if (soundPanel != null)
        {
            soundPanel.SetActive(false);
        }
    }

    private bool IsAnyPanelActive()
    {
        return (soundPanel != null && soundPanel.activeSelf) ||
               (pausePanel != null && pausePanel.activeSelf) ||
               (mainMenuPanel != null && mainMenuPanel.activeSelf);
    }

    public TextMeshProUGUI GetEnemyCountText()
    {
        return enemyCountText;
    }

    public TextMeshProUGUI GetWaveInfoText()
    {
        return waveInfoText;
    }

    public void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;  // Only pause the game for pause panel
        }
    }

    public void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            if (!IsAnyPanelActive())
            {
                Time.timeScale = 1f;
            }
        }
    }
}