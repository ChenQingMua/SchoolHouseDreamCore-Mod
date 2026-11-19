using System.Collections;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public GameObject occluder;

    public float quitDelay = 5f;

    private Coroutine quitRoutine;

    public void Show()
    {
        if (occluder != null)
        {
            occluder.SetActive(true);
        }

        if (quitRoutine == null)
        {
            quitRoutine = StartCoroutine(QuitAfterDelay());
        }
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, quitDelay));

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
