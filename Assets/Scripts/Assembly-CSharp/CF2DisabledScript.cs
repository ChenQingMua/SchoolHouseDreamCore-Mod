using UnityEngine;

public class CF2DisabledScript : MonoBehaviour
{
    public GameObject targetObject;

    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }
        }
        else
        {
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }
        }
    }
}
