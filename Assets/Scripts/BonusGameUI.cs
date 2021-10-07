using System;
using UnityEngine;
using UnityEngine.UI;
using static BonusManager;

/// <summary>Class used to play the bonus game</summary>
[RequireComponent(typeof(Animator))]
public class BonusGameUI : BaseBehaviour
{
	[Header("Settings")]
	public float distanceActivation;

	[Header("Scene references")]
	public Transform leftStartPoint;
	public Transform rightStartPoint;
	public Image leftImage, rightImage;
	public Button winButton;

	Animator anim;
	float speed;
	bool isPlaying;

	void OnDrawGizmos()
	{
		if(winButton != null)
			Debug.DrawLine(winButton.transform.position + winButton.transform.right * distanceActivation / 2, winButton.transform.position - winButton.transform.right * distanceActivation / 2, Color.red);
	}

	public void Init(Action SetBonusWon, Action SetBonusLost)
	{
		anim = GetComponent<Animator>();
		isPlaying = false;

		winButton.onClick.AddListener(() =>
		{
			if(!isPlaying)
				return;

			if(Vector3.Distance(leftImage.transform.parent.position, winButton.transform.position) <= distanceActivation)
			{
				SetBonusWon();

				leftImage.transform.parent.position = winButton.transform.position;
				rightImage.transform.parent.position = winButton.transform.position;

				anim.Play("Win");
			}
			else
			{
				SetBonusLost();
				anim.Play("Lose");
			}
		});

		InitInternal();
	}

	void Update()
	{
		if(isPlaying)
		{
			leftImage.transform.parent.position = Vector3.MoveTowards(leftImage.transform.parent.position, winButton.transform.position, speed * Time.deltaTime);
			rightImage.transform.parent.position = Vector3.MoveTowards(rightImage.transform.parent.position, winButton.transform.position, speed * Time.deltaTime);
		}
	}

	public void StartBonus(Bonus bonus, float delay)
	{
		if(!CheckInitialized())
			return;

		isPlaying = true;
		speed = Vector3.Distance(leftStartPoint.position, winButton.transform.position) / delay;

		leftImage.sprite = bonus.leftPart;
		leftImage.transform.parent.position = leftStartPoint.position;

		rightImage.sprite = bonus.rightPart;
		rightImage.transform.parent.position = rightStartPoint.position;
	}
}