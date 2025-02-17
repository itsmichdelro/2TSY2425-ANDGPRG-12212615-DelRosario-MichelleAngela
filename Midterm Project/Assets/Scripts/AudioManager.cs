// AudioManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip bulletSound;
    public AudioClip playerDeath;
    public AudioClip enemyHit;
    public AudioClip levelComplete;
    public AudioClip gameOver;

    private bool isMusicOn = true;
    private bool isSFXOn = true;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
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

    void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        LoadVolumeSettings();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        musicSource.mute = !isMusicOn;
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
    }

    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        sfxSource.mute = !isSFXOn;
        PlayerPrefs.SetInt("SFXEnabled", isSFXOn ? 1 : 0);
    }

    public void MusicVolume(float volume)
    {
        musicVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        sfxSource.volume = volume;
    }

    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume");
            MusicVolume(musicVol);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume");
            SFXVolume(sfxVol);
        }

        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        musicSource.mute = !isMusicOn;
        sfxSource.mute = !isSFXOn;
    }

    public void PlaySFX(string soundName)
    {
        if (!isSFXOn) return;

        AudioClip clipToPlay = null;
        switch (soundName.ToLower())
        {
            case "BulletSounds":
                clipToPlay = bulletSound;
                break;
            case "playerdeath":
                clipToPlay = playerDeath;
                break;
            case "enemyhit":
                clipToPlay = enemyHit;
                break;
            case "levelcomplete":
                clipToPlay = levelComplete;
                break;
            case "gameover":
                clipToPlay = gameOver;
                break;
        }

        if (clipToPlay != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clipToPlay);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!isSFXOn) return;
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}

