using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pausePanel;

    public string menuSceneName = "Menu";

    public bool showCursorWhenPaused = true;
    public bool muteAudioWhenPaused = false;

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    public static event System.Action<bool> OnPauseChanged;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!isPaused);
    }

    public void SetPaused(bool paused)
    {
        if (isPaused == paused) return;
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(paused);
        if (showCursorWhenPaused)
        {
            ControlFreak2.CFCursor.visible = paused;
            ControlFreak2.CFCursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        }
        if (muteAudioWhenPaused) AudioListener.pause = paused;
        OnPauseChanged?.Invoke(paused);
    }

    private void PrepareSceneTransition()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        ControlFreak2.CFCursor.visible = true;
        ControlFreak2.CFCursor.lockState = CursorLockMode.None;
        isPaused = false;
    }

    public void ResumeGame() => SetPaused(false);

    public void RestartGame()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        PrepareSceneTransition();
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (pausePanel != null) pausePanel.SetActive(false);
            AudioListener.pause = false;
        }
    }
}
