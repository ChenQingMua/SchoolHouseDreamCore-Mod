using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public PlayerScript ps;

    public Vector3 offset;

    private int lookBehind;

    public float positionAmount = 0.05f;
    public float rotationAmount = 0.5f;
    public float swaySpeed = 1f;

    public float movePosMultiplier = 2f;
    public float moveRotMultiplier = 4f;
    public float smoothTransitionSpeed = 5f;

    private float seed;
    private CharacterController cc;
    private float currentSpeedMultiplier = 1f;

    private void Start()
    {
        this.offset = base.transform.position - this.player.transform.position;
        this.seed = Random.Range(0f, 100f);

        if (player != null)
        {
            cc = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (ControlFreak2.CF2Input.GetButton("Look Behind"))
        {
            this.lookBehind = 180;
        }
        else
        {
            this.lookBehind = 0;
        }

        bool isMoving = false;
        if (cc != null)
        {
            Vector3 horizontalVelocity = new Vector3(cc.velocity.x, 0, cc.velocity.z);
            isMoving = horizontalVelocity.magnitude > 0.1f;
        }

        float targetMultiplier = isMoving ? 1f : 0f;

        currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetMultiplier, Time.deltaTime * smoothTransitionSpeed);

        float currentPosMult = Mathf.Lerp(1f, movePosMultiplier, currentSpeedMultiplier);
        float currentRotMult = Mathf.Lerp(1f, moveRotMultiplier, currentSpeedMultiplier);

        currentPosAmount = positionAmount * currentPosMult;
        currentRotAmount = rotationAmount * currentRotMult;
    }

    private float currentPosAmount;
    private float currentRotAmount;

    private void LateUpdate()
    {
        Vector3 baseTargetPos = this.player.transform.position + this.offset;
        Quaternion targetRot = this.player.transform.rotation * Quaternion.Euler(0f, (float)this.lookBehind, 0f);

        float timeX = Time.time * swaySpeed + seed;
        float timeY = Time.time * swaySpeed + seed + 10f;

        float noiseX = Mathf.PerlinNoise(timeX, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, timeY) - 0.5f;

        Vector3 posOffset = new Vector3(noiseX * currentPosAmount, noiseY * currentPosAmount, 0f);

        float rotX = noiseX * currentRotAmount;
        float rotY = noiseY * currentRotAmount;
        float rotZ = Mathf.PerlinNoise(timeX + 20f, timeY + 20f) * currentRotAmount - (currentRotAmount * 0.5f);

        Vector3 finalPos = baseTargetPos + (targetRot * posOffset);
        Quaternion finalRot = targetRot * Quaternion.Euler(rotX, rotY, rotZ);

        base.transform.position = finalPos;
        base.transform.rotation = finalRot;
    }
}
