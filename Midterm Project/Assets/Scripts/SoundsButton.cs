// SoundButton.cs - Updated to include a direct hide method
using UnityEngine;

public class SoundsButton : MonoBehaviour
{
    public void GoToSoundPanel()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSoundPanel();
        }
    }

    public void HideSoundPanel()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideSoundPanel();
        }
    }
}