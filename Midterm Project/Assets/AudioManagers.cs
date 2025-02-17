using UnityEngine;
using UnityEngine.UIElements;
public class AudioManagers : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip Background;
    public AudioClip BulletSounds;
    public AudioClip PlayerDeath;
    public AudioClip EnemyHit;

    public static object Instance { get; internal set; }

    private void Start()
    {
        musicSource.clip = Background;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}