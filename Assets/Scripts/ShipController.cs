using UnityEngine;

/// <summary>Class moving the ship depending on player inputs</summary>
public class ShipController : BaseBehaviour
{
	[Header("Speed")]
	public float forwardSpeed;
	public float minX, maxX, lateralSpeed;
	public AnimationCurve smoothingCurve;

	float horizontalInput, halfRange;
	bool gamePaused;

	void OnDrawGizmos()
	{
		Debug.DrawLine(Vector3.up * 1.5f + Vector3.right * minX - Vector3.forward * 2, Vector3.up * 1.5f + Vector3.right * minX + Vector3.forward * 10, Color.red);

		Debug.DrawLine(Vector3.up * 1.5f + Vector3.right * maxX - Vector3.forward * 2, Vector3.up * 1.5f + Vector3.right * maxX + Vector3.forward * 10, Color.red);
	}

	public void Init()
	{
		gamePaused = true;
		horizontalInput = 0;

		halfRange = Mathf.Abs(minX - maxX) / 2;

		InitInternal();
	}

	void Update()
	{
		if(!initialized || gamePaused)
			return;

		ManageInput();
		Movement();
	}

	void ManageInput()
	{
		if(Application.isMobilePlatform)
		{
			if(Input.touchCount == 0)
				return;

			horizontalInput = Input.GetTouch(0).position.x >= Screen.width ? 1 : -1;
		}
		else
			horizontalInput = Input.GetAxis("Horizontal");
	}

	void Movement()
	{
		float targetX = 0;

		if(horizontalInput > 0)
			targetX = maxX;
		else if(horizontalInput < 0)
			targetX = minX;

		Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
		float speedPonderation = Mathf.Clamp01(Mathf.Abs(transform.position.x - targetX) / halfRange);
		float smoothedSpeed = smoothingCurve.Evaluate(speedPonderation) * lateralSpeed;

		transform.position = Vector3.MoveTowards(transform.position, targetPos, smoothedSpeed * Time.deltaTime);
	}

	public void StartShip()
	{
		gamePaused = false;
	}

	public void FreezeShip()
	{
		gamePaused = true;
	}
}