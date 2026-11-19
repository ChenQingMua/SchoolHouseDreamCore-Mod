using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject doorIn;
    public GameObject doorOut;
    public GameObject barrier;

    public Material closedMaterial;
    public Material openMaterial;

    public AudioClip openSound;
    public AudioClip closeSound;
    public float soundVolume = 1.0f;

    public float autoCloseTime = 3.0f;
    public float interactDistance = 6.0f;

    private bool isOpen = false;
    private float closeTimer = 0f;

    private Renderer[] doorInRenderers;
    private Renderer[] doorOutRenderers;
    private Collider[] barrierColliders;
    private AudioSource doorAudio;

    void Start()
    {
        if (doorIn == null) doorIn = FindChildByName(transform, "_DoorIn");
        if (doorOut == null) doorOut = FindChildByName(transform, "_DoorOut");
        if (barrier == null) barrier = FindChildByName(transform, "_Barrier");

        if (doorIn != null) doorInRenderers = doorIn.GetComponentsInChildren<Renderer>();
        if (doorOut != null) doorOutRenderers = doorOut.GetComponentsInChildren<Renderer>();

        if (barrier != null)
        {
            Collider mainBarrierCol = barrier.GetComponent<Collider>();

            if (mainBarrierCol != null)
            {
                GameObject barrier2 = Instantiate(barrier, barrier.transform.parent);
                barrier2.name = "_Barrier_Back";

                barrier2.transform.localPosition = barrier.transform.localPosition;
                barrier2.transform.localRotation = barrier.transform.localRotation * Quaternion.Euler(0, 180, 0);
                barrier2.transform.localScale = barrier.transform.localScale;

                barrierColliders = new Collider[2];
                barrierColliders[0] = mainBarrierCol;
                barrierColliders[1] = barrier2.GetComponent<Collider>();
            }
            else
            {
                barrierColliders = new Collider[1] { mainBarrierCol };
            }
        }

        if (doorOut != null) doorAudio = doorOut.GetComponent<AudioSource>();
        if (doorAudio == null) doorAudio = GetComponentInChildren<AudioSource>();

        ApplyState(false, false);
    }

    void Update()
    {
        if (ControlFreak2.CF2Input.GetMouseButtonDown(0))
        {
            if (IsLookingAtDoor())
            {
                if (!isOpen)
                {
                    ApplyState(true, true);
                }
                else
                {
                    closeTimer = autoCloseTime;
                }
            }
        }

        if (isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
            {
                ApplyState(false, true);
            }
        }
    }

    private bool IsLookingAtDoor()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return false;

        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            return hit.collider.transform.IsChildOf(transform) || hit.collider.gameObject == transform.gameObject;
        }
        return false;
    }

    private void ApplyState(bool open, bool playAudio)
    {
        isOpen = open;

        Material targetMat = open ? openMaterial : closedMaterial;
        if (targetMat != null)
        {
            if (doorInRenderers != null) foreach (var r in doorInRenderers) r.material = targetMat;
            if (doorOutRenderers != null) foreach (var r in doorOutRenderers) r.material = targetMat;
        }

        if (barrierColliders != null)
        {
            foreach (var col in barrierColliders)
            {
                if (col != null) col.enabled = !open;
            }
        }

        if (open)
        {
            closeTimer = autoCloseTime;
            if (playAudio) PlaySound(openSound);
        }
        else
        {
            if (playAudio) PlaySound(closeSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (doorAudio != null)
        {
            doorAudio.Stop();
            doorAudio.PlayOneShot(clip, soundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }

    private GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;
            GameObject result = FindChildByName(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
