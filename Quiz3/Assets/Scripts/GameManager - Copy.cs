using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("UI Screens")]
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private LevelCompleteScreen levelCompleteScreen;

    private int enemiesKilled = 0;

    void Awake()
    {
        Instance = this;
        InitializeScreens();
    }

    private void InitializeScreens()
    {
        // Initialize Game Over Screen
        if (gameOverScreen != null)
        {
            gameOverScreen.gameObject.SetActive(false);
        }

        // Initialize Level Complete Screen
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.gameObject.SetActive(false);
        }
    }

    public void IncrementScore()
    {
        enemiesKilled++;
        AudioManager.Instance?.PlaySFX("EnemyDeath");
        Debug.Log($"Enemies killed: {enemiesKilled}");
    }

    public int GetScore()
    {
        return enemiesKilled;
    }

    public void ShowGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.Setup(enemiesKilled);
        }
        else
        {
            Debug.LogError("Game Over Screen reference is missing!");
        }
    }

    public void ShowLevelComplete()
    {
        if (levelCompleteScreen != null)
        {
            AudioManager.Instance?.PlaySFX("LevelComplete");
            levelCompleteScreen.Setup(enemiesKilled);
        }
        else
        {
            Debug.LogError("Level Complete Screen reference is missing!");
        }
    }
}