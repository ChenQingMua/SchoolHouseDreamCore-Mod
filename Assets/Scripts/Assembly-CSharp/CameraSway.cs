using UnityEngine;

public class CameraSway : MonoBehaviour
{
    public float positionAmount = 0.1f;
    public float rotationAmount = 2f;
    public float speed = 1f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float seed;

    void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        seed = Random.Range(0f, 100f);
    }

    void Update()
    {
        float timeX = Time.time * speed + seed;
        float timeY = Time.time * speed + seed + 10f;

        float noiseX = Mathf.PerlinNoise(timeX, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, timeY) - 0.5f;

        Vector3 posOffset = new Vector3(noiseX * positionAmount, noiseY * positionAmount, 0f);
        transform.localPosition = startPosition + posOffset;

        float rotX = noiseX * rotationAmount;
        float rotY = noiseY * rotationAmount;
        float rotZ = Mathf.PerlinNoise(timeX + 20f, timeY + 20f) * rotationAmount - (rotationAmount * 0.5f);

        transform.localRotation = startRotation * Quaternion.Euler(rotX, rotY, rotZ);
    }
}
