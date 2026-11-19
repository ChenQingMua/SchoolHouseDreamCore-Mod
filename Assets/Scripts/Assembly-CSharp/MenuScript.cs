using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void JoinUs()
    {
        Application.OpenURL("https://qm.qq.com/q/9hRvkQCvYs");
    }
    public void Itch()
    {
        Application.OpenURL("https://chenqingmua.itch.io/shdc");
    }
    public void GitHub()
    {
        Application.OpenURL("https://github.com/ChenQingMua/SchoolHouseDreamCore-Mod");
    }
}
