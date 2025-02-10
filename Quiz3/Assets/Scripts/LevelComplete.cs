// LevelCompleteScreen.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " ENEMIES DEFEATED";


        AudioManager.Instance.PlaySFX("LevelComplete");

    }
    public void RestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
