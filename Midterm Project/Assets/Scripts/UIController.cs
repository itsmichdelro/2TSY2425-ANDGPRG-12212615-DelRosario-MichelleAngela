
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void ToggleMusic()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        if (AudioManager.Instance != null && musicSlider != null)
            AudioManager.Instance.MusicVolume(musicSlider.value);
    }

    public void SFXVolume()
    {
        if (AudioManager.Instance != null && sfxSlider != null)
            AudioManager.Instance.SFXVolume(sfxSlider.value);
    }
}