// SoundPanel.cs
using UnityEngine;
using UnityEngine.SceneManagement;
public class SoundPanel : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    public void OnCancelButton()
    {
        UIManager.Instance.HideSoundPanel();

        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}