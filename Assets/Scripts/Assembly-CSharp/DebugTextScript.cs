using UnityEngine;
using TMPro;

public class DebugTextScript : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public Transform player;

    private float deltaTime = 0.0f;
    private bool isVisible = false;

    void Start()
    {
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.E))
        {
            isVisible = !isVisible;
            if (fpsText != null)
            {
                fpsText.gameObject.SetActive(isVisible);
            }
        }

        if (!isVisible || fpsText == null) return;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        Vector3 playerPos = player != null ? player.position : Vector3.zero;

        float totalSeconds = Time.time;
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        string timeString = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        fpsText.text = $"{fps:0.0} FPS\n" +
                       $"X:{playerPos.x:0.0} Y:{playerPos.y:0.0} Z:{playerPos.z:0.0}\n" +
                       $"Time: {timeString}";
    }
}
