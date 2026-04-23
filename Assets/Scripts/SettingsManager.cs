using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    public Volume globalVolume;
    public PlayerMovement playerMovement;
    public UnityEngine.UI.Slider brightnessSlider;
    public UnityEngine.UI.Slider fovSlider;
    public UnityEngine.UI.Slider sensitivitySlider;
    public UnityEngine.UI.Slider masterVolumeSlider;
    void Start()
    {
        if (globalVolume != null)
        {
            foreach (var component in globalVolume.sharedProfile.components)
                Debug.Log(component.GetType().Name);
        }
        // OnValueChanged 임시 제거
        brightnessSlider.onValueChanged.RemoveAllListeners();
        fovSlider.onValueChanged.RemoveAllListeners();
        sensitivitySlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        // 저장된 값으로 슬라이더 위치 설정
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0f);
        fovSlider.value = PlayerPrefs.GetFloat("FOV", 70f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 75f);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100f);


        // 값 적용
        SetBrightness(brightnessSlider.value);
        SetFOV(fovSlider.value);
        SetSensitivity(sensitivitySlider.value);
        SetMasterVolume(masterVolumeSlider.value);

        SetFullscreen(PlayerPrefs.GetInt("Fullscreen", 1) == 1);

        // OnValueChanged 다시 연결 
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        fovSlider.onValueChanged.AddListener(SetFOV);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
    }

    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
        if (globalVolume == null)   return;
        ColorAdjustments ca;
        if (globalVolume.sharedProfile.TryGet(out ca))
            ca.postExposure.value = value;
    }

    public void SetFOV(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
        if (Camera.main != null)
            Camera.main.fieldOfView = value;
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        if (playerMovement != null)
            playerMovement.mouseSensitivity = value / 100f;
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value / 100f;
    }
}