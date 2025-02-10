using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;  

public class GameOverScreen : MonoBehaviour
{
  
    [SerializeField] private TextMeshProUGUI pointsTextTMP;  

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        AudioManager.Instance?.PlaySFX("GameOver");
        if (pointsTextTMP != null)
        {
            pointsTextTMP.text = score.ToString() + " ENEMIES KILLED";
        }
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