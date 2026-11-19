using UnityEngine;

public class VSyncScript : MonoBehaviour
{
    private const string VSYNC_KEY = "V_Sync";

    private void Start()
    {
        ApplyVSync();
    }

    private void ApplyVSync()
    {
        int savedVSync = PlayerPrefs.GetInt(VSYNC_KEY, 1);

        if (savedVSync == 1)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
