using UnityEngine;

public class ZoomScript : MonoBehaviour
{
    public float normalFOV = 60f;

    public float zoomFOV = 20f;

    public float zoomSpeed = 10f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            normalFOV = cam.fieldOfView;
        }
    }

    void Update()
    {
        if (cam == null) return;

        if (ControlFreak2.CF2Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
        }
    }
}
