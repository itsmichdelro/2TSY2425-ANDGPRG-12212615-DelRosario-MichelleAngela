using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject howToPlayPanel;
    

    void Start()
    {
        mainPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
 
    }

    public void StartButton()
    {
        if (Time.timeScale < 1.0f) Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void HowToPlayButton()
    {
        mainPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }



    public void BackButton()
    {
        if (howToPlayPanel.activeInHierarchy)
        {
            howToPlayPanel.SetActive(false);
        }
        else
        {

            mainPanel.SetActive(true);
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
