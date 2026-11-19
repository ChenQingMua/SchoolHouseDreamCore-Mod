using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Toggle vsyncToggle;
    public Toggle cameraEffectsToggle;
    public Slider sensitivitySlider;

    private const string VSYNC_KEY = "V_Sync";
    private const string CAMERA_FX_KEY = "Camera_Effects";
    private const string SENSITIVITY_KEY = "Sensitivity";

    private bool isLoading = false;

    void Start()
    {
        LoadSettings();

        vsyncToggle.onValueChanged.AddListener(OnVsyncChanged);
        cameraEffectsToggle.onValueChanged.AddListener(OnCameraEffectsChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void LoadSettings()
    {
        isLoading = true;

        bool isVsyncOn = PlayerPrefs.GetInt(VSYNC_KEY, 1) == 1;
        vsyncToggle.isOn = isVsyncOn;
        QualitySettings.vSyncCount = isVsyncOn ? 1 : 0;

        bool isCameraFxOn = PlayerPrefs.GetInt(CAMERA_FX_KEY, 1) == 1;
        cameraEffectsToggle.isOn = isCameraFxOn;

        SetCameraEffect(isCameraFxOn);

        float sensitivityValue = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 20f);
        sensitivitySlider.value = sensitivityValue;

        PlayerScript player = FindObjectOfType<PlayerScript>();
        if (player != null) player.mouseSensitivity = sensitivityValue;

        isLoading = false;
    }

    public void OnVsyncChanged(bool isOn)
    {
        if (isLoading) return;

        QualitySettings.vSyncCount = isOn ? 1 : 0;
        PlayerPrefs.SetInt(VSYNC_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnCameraEffectsChanged(bool isOn)
    {
        if (isLoading) return;

        SetCameraEffect(isOn);

        PlayerPrefs.SetInt(CAMERA_FX_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSensitivityChanged(float value)
    {
        if (isLoading) return;

        PlayerScript player = FindObjectOfType<PlayerScript>();
        if (player != null) player.mouseSensitivity = value;

        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
        PlayerPrefs.Save();
    }

    private void SetCameraEffect(bool isOn)
    {
        DreamcoreFX cameraFx = FindObjectOfType<DreamcoreFX>();
        if (cameraFx != null)
        {
            cameraFx.SetEffectState(isOn);
        }
    }
}
