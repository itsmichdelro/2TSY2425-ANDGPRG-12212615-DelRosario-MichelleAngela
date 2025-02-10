// UIManager.cs
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject soundPanel;
    public GameObject pausePanel;
    public GameObject mainMenuPanel;

    private void Awake()
    {
        Instance = this;
        InitializePanels();
    }

    private void InitializePanels()
    {
        // Hide sound and pause panels
        if (soundPanel != null) soundPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Make sure main menu is active at start
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowSoundPanel()
    {
        if (soundPanel != null)
        {
            soundPanel.SetActive(true);
            // Remove TimeScale manipulation for sound panel
            // This allows volume settings to work properly
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

    // Add methods specifically for pause panel if needed
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