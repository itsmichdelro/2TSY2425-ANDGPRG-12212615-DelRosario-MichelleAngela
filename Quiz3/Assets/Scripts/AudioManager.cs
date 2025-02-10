// AudioManager.cs
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    // Keep track of current volumes
    private float currentMusicVolume = 1f;
    private float currentSFXVolume = 1f;
    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Theme");

        // Load saved volume settings if you have them
        LoadVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        // Load saved settings from PlayerPrefs
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        isSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        // Apply loaded settings
        musicSource.volume = isMusicMuted ? 0 : currentMusicVolume;
        sfxSource.volume = isSFXMuted ? 0 : currentSFXVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", currentSFXVolume);
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found: " + name);
            return;
        }

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found: " + name);
            return;
        }

        // Apply volume settings to the OneShot
        sfxSource.PlayOneShot(s.clip, isSFXMuted ? 0 : currentSFXVolume);
    }

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        musicSource.volume = isMusicMuted ? 0 : currentMusicVolume;
        SaveVolumeSettings();
    }

    public void ToggleSFX()
    {
        isSFXMuted = !isSFXMuted;
        sfxSource.volume = isSFXMuted ? 0 : currentSFXVolume;
        SaveVolumeSettings();
    }

    public void MusicVolume(float volume)
    {
        currentMusicVolume = volume;
        if (!isMusicMuted)
        {
            musicSource.volume = volume;
        }
        SaveVolumeSettings();
    }

    public void SFXVolume(float volume)
    {
        currentSFXVolume = volume;
        if (!isSFXMuted)
        {
            sfxSource.volume = volume;
        }
        SaveVolumeSettings();
    }

    internal float GetSFXVolume()
    {
        throw new NotImplementedException();
    }

    internal float GetMusicVolume()
    {
        throw new NotImplementedException();
    }
}