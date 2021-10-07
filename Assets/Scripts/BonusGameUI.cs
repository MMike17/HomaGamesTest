using System;
using UnityEngine;
using UnityEngine.UI;
using static BonusManager;

/// <summary>Class used to play the bonus game</summary>
[RequireComponent(typeof(Animator))]
public class BonusGameUI : BaseBehaviour
{
	// TODO : Make "Win" animation in editor
	// TODO : Make "Lose" animation in editor

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

			if(Vector3.Distance(leftImage.transform.position, winButton.transform.position) <= distanceActivation)
			{
				SetBonusWon();
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
			leftImage.transform.position = Vector3.MoveTowards(leftImage.transform.position, winButton.transform.position, speed * Time.deltaTime);
			rightImage.transform.position = Vector3.MoveTowards(rightImage.transform.position, winButton.transform.position, speed * Time.deltaTime);
		}
	}

	public void StartBonus(Bonus bonus, float delay)
	{
		if(!CheckInitialized())
			return;

		isPlaying = true;
		speed = Vector3.Distance(leftStartPoint.position, winButton.transform.position) / delay;

		leftImage.sprite = bonus.leftPart;
		leftImage.transform.position = leftStartPoint.position;

		rightImage.sprite = bonus.rightPart;
		rightImage.transform.position = rightStartPoint.position;
	}
}