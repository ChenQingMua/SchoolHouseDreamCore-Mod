using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float mouseSensitivity = 2f;

    public CharacterController cc;
    public Canvas hud;

    private Quaternion playerRotation;
    private Vector3 moveDirection;
    private float playerSpeed;
    private float height;
    private bool sensitivityActive;

    private void Start()
    {
        this.height = base.transform.position.y;
        this.playerRotation = base.transform.rotation;

        this.mouseSensitivity = ((PlayerPrefs.GetFloat("Sensitivity", 10f) / 10));

        if (PlayerPrefs.GetInt("AnalogMove") == 1)
        {
            this.sensitivityActive = true;
        }
    }

    private void Update()
    {
        base.transform.position = new Vector3(base.transform.position.x, this.height, base.transform.position.z);

        this.MouseMove();

        this.PlayerMove();

        if (this.cc.velocity.magnitude > 0f)
        {
            ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
            ControlFreak2.CFCursor.visible = false;
        }
    }

    private void MouseMove()
    {
        this.playerRotation.eulerAngles = new Vector3(0f, this.playerRotation.eulerAngles.y, 0f);

        this.playerRotation.eulerAngles += Vector3.up * ControlFreak2.CF2Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.timeScale;

        base.transform.rotation = this.playerRotation;
    }

    private void PlayerMove()
    {
        Vector3 forwardMove = base.transform.forward * ControlFreak2.CF2Input.GetAxis("Forward");
        Vector3 rightMove = base.transform.right * ControlFreak2.CF2Input.GetAxis("Strafe");
        Vector3 input = forwardMove + rightMove;

        float mag = Mathf.Clamp01(input.magnitude);
        Vector3 dir = mag > 0.0001f ? input / mag : Vector3.zero;

        float speed = ControlFreak2.CF2Input.GetButton("Run") ? runSpeed : walkSpeed;

        if (!this.sensitivityActive)
            mag = mag > 0.0001f ? 1f : 0f;

        this.moveDirection = dir * speed * mag * Time.deltaTime;

        this.cc.Move(this.moveDirection);
    }
}
