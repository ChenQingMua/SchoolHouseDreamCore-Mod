using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public float interactDistance = 6f;

    public GameObject targetUI;

    public LayerMask detectLayers = ~0;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (targetUI != null) targetUI.SetActive(false);
    }

    void Update()
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        bool shouldShowUI = false;

        if (Physics.Raycast(ray, out hit, interactDistance, detectLayers))
        {
            if (hit.collider.isTrigger)
            {
                shouldShowUI = true;
            }

            else if (hit.collider.CompareTag("Button") || hit.collider.GetComponentInParent<ButtonScript>() != null)
            {
                shouldShowUI = true;
            }
        }

        if (targetUI != null && targetUI.activeSelf != shouldShowUI)
        {
            targetUI.SetActive(shouldShowUI);
        }
    }
}
