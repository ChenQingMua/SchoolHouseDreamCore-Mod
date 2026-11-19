using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public float loadDelay = 1f;

    public bool useFadeToBlack = true;
    public float fadeDuration = 1.5f;

    private bool isLoading = false;

    public void LoadTargetScene()
    {
        if (isLoading) return;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("³¡¾°Ãû³ÆÎ´ÌîÐ´£¡");
            return;
        }

        isLoading = true;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        yield return new WaitForSeconds(loadDelay);

        if (useFadeToBlack)
        {
            yield return StartCoroutine(FadeToBlackRoutine());
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeToBlackRoutine()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        GameObject imageObj = new GameObject("BlackScreenImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        Image img = imageObj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        img.color = new Color(0, 0, 0, 1);
    }
}
