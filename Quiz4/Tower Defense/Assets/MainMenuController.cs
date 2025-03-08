using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsMenuPanel;

    [Header("Audio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource backgroundMusic;

    // Keep track of previous time scale when pausing
    private float timeScaleBeforePause;

    private void Start()
    {
        // Make sure settings panel is initially hidden
        if (settingsMenuPanel != null)
        {
            settingsMenuPanel.SetActive(false);
        }

        // Set up the music slider
        if (backgroundMusic != null && musicSlider != null)
        {
            // Initialize slider to current volume
            musicSlider.value = backgroundMusic.volume;

            // Add listener to slider value change
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    // Called when Sound button is clicked
    public void OpenSettingsMenu()
    {
        if (settingsMenuPanel != null)
        {
            // Store current time scale
            timeScaleBeforePause = Time.timeScale;

            // Pause the game
            Time.timeScale = 0f;

            // Show settings menu
            settingsMenuPanel.SetActive(true);
        }
    }

    // Called when X button in settings is clicked
    public void CloseSettingsMenu()
    {
        if (settingsMenuPanel != null)
        {
            // Hide settings menu
            settingsMenuPanel.SetActive(false);

            // Resume the game
            Time.timeScale = timeScaleBeforePause;
        }
    }

    // Called when music slider value changes
    public void SetMusicVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;

            // Optional: Save the volume setting for future sessions
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }

    // Called when Restart button is clicked
    public void RestartGame()
    {
        // Unpause game
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called when Main Menu button is clicked
    public void GoToMainMenu()
    {
        // Unpause game
        Time.timeScale = 1f;

        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}