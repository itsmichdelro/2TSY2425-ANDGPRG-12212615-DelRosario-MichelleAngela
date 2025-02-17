using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Button nextLevelButton;  // Reference to the Next Level button
    [SerializeField] private TextMeshProUGUI levelCompleteText;  // Optional: to show current level

    private void Start()
    {
        // Hide the Next Level button if we're on the final level
        if (nextLevelButton != null && WaveEnemySpawner.Instance != null)
        {
            nextLevelButton.gameObject.SetActive(WaveEnemySpawner.Instance.GetCurrentLevel() < 3);
        }
    }

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        int currentLevel = WaveEnemySpawner.Instance.GetCurrentLevel();

        // Update UI text
        pointsText.text = score.ToString() + " ENEMIES DEFEATED";
        if (levelCompleteText != null)
        {
            levelCompleteText.text = $"LEVEL {currentLevel} COMPLETE!";
        }

        // Show/hide next level button based on current level
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(currentLevel < 3);
        }

        AudioManager.Instance.PlaySFX("LevelComplete");
    }

    public void NextLevelButton()
    {
        if (WaveEnemySpawner.Instance != null)
        {
            int nextLevel = WaveEnemySpawner.Instance.GetCurrentLevel() + 1;
            if (nextLevel <= 3)
            {
                // Reset UI and game state
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ResetAllUI();
                    // Add this line to update the level text
                    UIManager.Instance.UpdateLevelText(nextLevel);
                }
                // Set next level configuration
                WaveEnemySpawner.Instance.SetLevel(nextLevel);
                // Hide level complete screen
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public void RestartButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}