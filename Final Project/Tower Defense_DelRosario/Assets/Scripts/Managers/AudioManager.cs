using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sounds[] sounds;
    
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioMixer audioMixer;

    [Header("Audio Sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    private void Awake()
    {
        Instance = this;
        foreach (Sounds s in sounds) //to see in inspector
        { 
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.outputAudioMixerGroup = s.mixerGroup;
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }

    private void Start()
    {
        PlaySound("BGM");
    }

    public void PlaySound(string name) //FindObjectOfType<AudioManager>().Play("name");

    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("No sound of name " + name); return;
        }

        s.audioSource.Play();
    }

    public void ChangeMasterVolume()
    {
        ChangeVolume("MasterVol", masterSlider.value);
    }

    public void ChangeBGMVolume()
    {
        ChangeVolume("BGMVol", bgmSlider.value);
    }

    public void ChangeSFXVolume()
    {
        ChangeVolume("SFXVol", sfxSlider.value);
    }

    public void ChangeVolume(string name, float value)
    {
        if (value <= 0.0f)
        {
            audioMixer.SetFloat(name, -80.0f);
        }
        else
        {
            float dbVolume = Mathf.Log10(value) * 20;
            audioMixer.SetFloat(name, dbVolume);
        }
    }
}
