using UnityEngine;
using TMPro;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    public GameObject blackScreen;
    public TextMeshProUGUI introText;

    public Transform playerTransform;
    public Camera mainCamera;
    public MonoBehaviour cameraScript;
    public MonoBehaviour playerScript;

    public float loadWaitTime = 1f;
    public float startCameraHeight = -0.5f;
    public float standUpDuration = 3f;
    public float textShowDuration = 5f;

    private float defaultCamHeight;
    private Quaternion defaultCamRotation;

    void Start()
    {
        if (blackScreen != null) blackScreen.SetActive(true);

        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            SetAlpha(introText, 0f);
        }

        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(loadWaitTime);

        if (cameraScript != null) cameraScript.enabled = false;
        if (playerScript != null) playerScript.enabled = false;

        Vector3 originalOffset = mainCamera.transform.position - playerTransform.position;
        defaultCamHeight = originalOffset.y;

        defaultCamRotation = mainCamera.transform.localRotation;

        Vector3 camPos = playerTransform.position + new Vector3(originalOffset.x, startCameraHeight, originalOffset.z);
        mainCamera.transform.position = camPos;

        Quaternion startRot = Quaternion.Euler(40f, defaultCamRotation.eulerAngles.y, defaultCamRotation.eulerAngles.z);
        mainCamera.transform.localRotation = startRot;

        float fadeDuration = 2f;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (blackScreen != null) SetAlpha(blackScreen, 1f - (t / fadeDuration));
            yield return null;
        }
        if (blackScreen != null) blackScreen.SetActive(false);

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        t = 0f;
        Vector3 startPos = mainCamera.transform.position;
        while (t < standUpDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / standUpDuration);

            Vector3 targetPos = playerTransform.position + new Vector3(originalOffset.x, defaultCamHeight, originalOffset.z);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, progress);

            mainCamera.transform.localRotation = Quaternion.Slerp(startRot, defaultCamRotation, progress);

            yield return null;
        }

        if (cc != null) cc.enabled = true;

        if (cameraScript != null) cameraScript.enabled = true;
        if (playerScript != null) playerScript.enabled = true;

        if (introText != null)
        {
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                SetAlpha(introText, t);
                yield return null;
            }

            yield return new WaitForSeconds(textShowDuration);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                SetAlpha(introText, 1f - t);
                yield return null;
            }
            introText.gameObject.SetActive(false);
        }
    }

    private void SetAlpha(object target, float alpha)
    {
        if (target == null) return;

        UnityEngine.UI.Graphic graphic = null;
        TMPro.TextMeshProUGUI tmpText = null;

        if (target is GameObject go)
        {
            graphic = go.GetComponent<UnityEngine.UI.Graphic>();
            tmpText = go.GetComponent<TMPro.TextMeshProUGUI>();
        }
        else if (target is Component comp)
        {
            graphic = comp.GetComponent<UnityEngine.UI.Graphic>();
            tmpText = comp.GetComponent<TMPro.TextMeshProUGUI>();
        }

        if (graphic != null)
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
        else if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
    }
}
