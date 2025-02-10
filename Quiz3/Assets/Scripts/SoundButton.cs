using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundButton : MonoBehaviour
{
    public void GoToSoundPanel()
    {
        SceneManager.LoadScene("Sound Panel");
    }
}