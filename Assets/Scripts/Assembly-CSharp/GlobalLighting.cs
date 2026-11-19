using UnityEngine;
using System.Collections;
using TMPro;

public class GlobalLighting : MonoBehaviour
{
    public float minLightOnTime = 300f;
    public float maxLightOnTime = 1200f;
    public float minLightOffTime = 60f;
    public float maxLightOffTime = 300f;

    public float flashlightRange = 20f;
    public float flashlightIntensity = 3f;
    public float flashlightAngle = 45f;
    public Color flashlightColor = Color.white;

    public TextMeshProUGUI flashlightHintText;

    private Color originalAmbientColor;
    private Color originalFogColor;
    private bool originalFogState;

    private Light flashlight;
    private bool isFlashlightOn = false;
    private bool isLightsOff = false;
    private bool hasShownHint = false;

    void Start()
    {
        originalAmbientColor = RenderSettings.ambientLight;
        originalFogColor = RenderSettings.fogColor;
        originalFogState = RenderSettings.fog;

        SetupFlashlight();

        if (flashlightHintText != null)
        {
            flashlightHintText.gameObject.SetActive(false);
        }

        StartCoroutine(LightRoutine());
    }

    void Update()
    {
        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.Q))
        {
            isFlashlightOn = !isFlashlightOn;
            if (flashlight != null)
            {
                flashlight.enabled = isFlashlightOn;
            }

            if (isFlashlightOn && flashlightHintText != null)
            {
                flashlightHintText.gameObject.SetActive(false);
                hasShownHint = true;
            }
        }
    }

    private void SetupFlashlight()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("������û�� Main Camera���ֵ�Ͳ�޷����أ�");
            return;
        }

        GameObject flashlightObj = new GameObject("PlayerFlashlight");
        flashlightObj.transform.SetParent(mainCam.transform);
        flashlightObj.transform.localPosition = Vector3.zero;
        flashlightObj.transform.localRotation = Quaternion.identity;

        flashlight = flashlightObj.AddComponent<Light>();
        flashlight.type = LightType.Spot;
        flashlight.range = flashlightRange;
        flashlight.intensity = flashlightIntensity;
        flashlight.spotAngle = flashlightAngle;
        flashlight.color = flashlightColor;
        flashlight.shadows = LightShadows.Soft;
        flashlight.enabled = false;
    }

    private IEnumerator LightRoutine()
    {
        while (true)
        {
            float onTime = Random.Range(minLightOnTime, maxLightOnTime);
            yield return new WaitForSeconds(onTime);

            TurnOffLights();

            float offTime = Random.Range(minLightOffTime, maxLightOffTime);
            yield return new WaitForSeconds(offTime);

            TurnOnLights();
        }
    }

    private void TurnOffLights()
    {
        isLightsOff = true;

        Color dimColor = new Color(10f / 255f, 10f / 255f, 10f / 255f);

        RenderSettings.ambientLight = dimColor;
        RenderSettings.fog = true;
        RenderSettings.fogColor = dimColor;

        if (!hasShownHint && !isFlashlightOn && flashlightHintText != null)
        {
            flashlightHintText.gameObject.SetActive(true);
        }

        Debug.Log("[�ƹ�ϵͳ] �صƣ����뼫��״̬��");
    }

    private void TurnOnLights()
    {
        isLightsOff = false;

        RenderSettings.ambientLight = originalAmbientColor;
        RenderSettings.fogColor = Color.white;
        RenderSettings.fog = originalFogState;

        if (flashlightHintText != null)
        {
            flashlightHintText.gameObject.SetActive(false);
        }

        Debug.Log("[�ƹ�ϵͳ] ���ƣ��ָ�������");
    }
}
