using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public Material normalMaterial;
    public Material pressedMaterial;

    public AudioClip pressSound;
    public float soundVolume = 1.0f;

    public float interactDistance = 6f;

    public UnityEvent onClick;

    private Renderer buttonRenderer;
    private AudioSource audioSource;
    private bool isAnimating = false;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer == null) buttonRenderer = GetComponentInChildren<Renderer>();

        if (normalMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = normalMaterial;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void Update()
    {
        if (ControlFreak2.CF2Input.GetMouseButtonDown(0) && !isAnimating)
        {
            if (IsLookingAtButton())
            {
                PressButton();
            }
        }
    }

    private bool IsLookingAtButton()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return false;

        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }
        return false;
    }

    private void PressButton()
    {
        isAnimating = true;

        if (pressedMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = pressedMaterial;
        }

        if (pressSound != null)
        {
            audioSource.PlayOneShot(pressSound, soundVolume);
        }

        onClick?.Invoke();

        Invoke(nameof(ResetButton), 0.2f);
    }

    private void ResetButton()
    {
        if (normalMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = normalMaterial;
        }

        isAnimating = false;
    }
}
