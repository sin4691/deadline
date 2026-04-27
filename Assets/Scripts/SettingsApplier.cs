using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void Start()
    {
        Debug.Log("Timer: " + PlayerPrefs.GetString("TimerDarkUISwitch"));
        Debug.Log("Fullscreen: " + PlayerPrefs.GetString("FullscreenDarkUISwitch"));
        Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV", 70f);
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 100f) / 100f;
        if (playerMovement != null)
            playerMovement.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 75f) / 100f;

        // ≈∏¿Ã∏”
        if (Timer.Instance != null)
            Timer.Instance.SetTimerVisible(PlayerPrefs.GetString("TimerDarkUISwitch", "false") == "true");
    }
}