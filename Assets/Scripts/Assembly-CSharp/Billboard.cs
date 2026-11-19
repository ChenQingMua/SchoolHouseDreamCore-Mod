using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Camera m_Camera;

	private void Start()
	{
		this.m_Camera = Camera.main;
	}

	private void LateUpdate()
	{
		if (this.m_Camera == null)
		{
			this.m_Camera = Camera.main;
			if (this.m_Camera == null)
			{
				return;
			}
		}

		Vector3 forward = this.m_Camera.transform.rotation * Vector3.forward;
		forward.y = 0f;

		if (forward.sqrMagnitude < 0.0001f)
		{
			return;
		}

		base.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
	}
}
