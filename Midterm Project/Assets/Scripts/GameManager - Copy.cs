// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Screens")]
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private LevelCompleteScreen levelCompleteScreen;

    private int enemiesKilled = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeScreens();
    }

    private void InitializeScreens()
    {
        if (gameOverScreen == null)
        {
            Debug.LogError("GameOverScreen not assigned in GameManager!");
            return;
        }

        if (levelCompleteScreen == null)
        {
            Debug.LogError("LevelCompleteScreen not assigned in GameManager!");
            return;
        }

        gameOverScreen.gameObject.SetActive(false);
        levelCompleteScreen.gameObject.SetActive(false);
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
            gameOverScreen.gameObject.SetActive(true);
            gameOverScreen.Setup(enemiesKilled);
        }
        else
        {
            Debug.LogError("Game Over Screen reference is missing!");
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
            levelCompleteScreen.gameObject.SetActive(true);
            levelCompleteScreen.Setup(enemiesKilled);
        }
        else
        {
            Debug.LogError("Level Complete Screen reference is missing!");
        }
    }
}

