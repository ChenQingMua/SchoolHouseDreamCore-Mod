using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DreamcoreFX : MonoBehaviour
{
    public Shader dreamShader;
    public Material dreamMaterial;

    private bool isEffectEnabled = true;

    void Start()
    {
        if (dreamShader != null && dreamShader.isSupported)
        {
            dreamMaterial = new Material(dreamShader);
        }

        int savedCameraFx = PlayerPrefs.GetInt("Camera_Effects", 1);
        isEffectEnabled = (savedCameraFx == 1);
    }

    public void SetEffectState(bool state)
    {
        isEffectEnabled = state;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isEffectEnabled && dreamMaterial != null)
        {
            Graphics.Blit(source, destination, dreamMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
