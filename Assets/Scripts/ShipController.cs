using System;
using UnityEngine;

/// <summary>Class moving the ship depending on player inputs</summary>
[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class ShipController : BaseBehaviour
{
	[Header("Speed")]
	public float minX;
	public float maxX, lateralSpeed, maxRotation, rotationSpeed;
	public AnimationCurve smoothingCurve;

	Collider shipCollider;
	Animator anim;
	Action ShowEndScreen;
	float horizontalInput, halfRange, rotationAmount;
	bool gamePaused, blockInput;

	void OnDrawGizmos()
	{
		Debug.DrawLine(Vector3.up * 1.5f + Vector3.right * minX - Vector3.forward * 2, Vector3.up * 1.5f + Vector3.right * minX + Vector3.forward * 10, Color.red);

		Debug.DrawLine(Vector3.up * 1.5f + Vector3.right * maxX - Vector3.forward * 2, Vector3.up * 1.5f + Vector3.right * maxX + Vector3.forward * 10, Color.red);
	}

	public void Init(Action showEndScreen)
	{
		ShowEndScreen = showEndScreen;

		gamePaused = true;
		horizontalInput = 0;

		anim = GetComponent<Animator>();
		shipCollider = GetComponentInChildren<Collider>();
		halfRange = Mathf.Abs(minX - maxX) / 2;

		InitInternal();

		FreezeShip();
	}

	void Update()
	{
		if(!initialized || gamePaused)
			return;

		ManageInput();
		RotateShip();
		Movement();
	}

	void ManageInput()
	{
		if(Application.isMobilePlatform)
		{
			if(Input.touchCount == 0)
				horizontalInput = 0;
			else
				horizontalInput = Input.GetTouch(0).position.x >= Screen.width / 2 ? 1 : -1;
		}
		else
			horizontalInput = Input.GetAxis("Horizontal");

		if(blockInput)
			horizontalInput = 0;
	}

	void RotateShip()
	{
		rotationAmount = Mathf.MoveTowards(rotationAmount, -horizontalInput, rotationSpeed * Time.deltaTime);

		transform.rotation = Quaternion.Euler(0, 0, rotationAmount * maxRotation);
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

	void OnCollisionEnter(Collision collision)
	{
		if(gamePaused)
			return;

		if(collision.collider.CompareTag("Obstacle"))
		{
			anim.Play("Crash");

			ShowEndScreen();
			FreezeShip();
		}
		else
			Debug.LogError(debugTag + "This shouldn't happen");
	}

	public void StartShip()
	{
		if(!CheckInitialized())
			return;

		anim.Play("Idle");
		gamePaused = false;
	}

	public void FreezeShip()
	{
		if(!CheckInitialized())
			return;

		gamePaused = true;
	}

	public void BlockInput()
	{
		if(!CheckInitialized())
			return;

		blockInput = true;
	}

	public void UnlockInput()
	{
		if(!CheckInitialized())
			return;

		blockInput = false;
	}

	public void Restart()
	{
		if(!CheckInitialized())
			return;

		anim.Play("Idle");
	}

	public void GetBonus()
	{
		if(!CheckInitialized())
			return;

		anim.Play("Bonus");
	}

	public void PassObstacle()
	{
		if(!CheckInitialized())
			return;

		anim.Play("PassObstacle");
	}
}