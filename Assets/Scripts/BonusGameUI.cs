using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BonusManager;

/// <summary>Class used to play the bonus game</summary>
[RequireComponent(typeof(Animator))]
public class BonusGameUI : BaseBehaviour
{
	[Header("Settings")]
	public float minDistanceThreshold;
	public Color[] bonusColors;

	[Header("Scene references")]
	public Transform leftStartPoint;
	public Transform rightStartPoint;
	public Image leftImage, rightImage;
	public Button winButton;

	Animator anim;
	Action SetBonusLost;
	float speed, waitingTimer, opportunitySize;
	int lastBonusColor;
	bool isPlaying, isWaiting;

	void OnDrawGizmos()
	{
		if(winButton != null)
			Debug.DrawLine(winButton.transform.position + winButton.transform.right * minDistanceThreshold / 2, winButton.transform.position - winButton.transform.right * minDistanceThreshold / 2, Color.red);
	}

	public void Init(Action SetBonusWon, Action setBonusLost)
	{
		SetBonusLost = setBonusLost;

		anim = GetComponent<Animator>();
		isPlaying = false;

		winButton.onClick.AddListener(() =>
		{
			if(!isPlaying)
				return;

			isPlaying = false;

			if(Vector3.Distance(leftImage.transform.parent.position, winButton.transform.position) <= minDistanceThreshold)
			{
				isPlaying = false;

				SetBonusWon();

				leftImage.transform.parent.position = winButton.transform.position;
				rightImage.transform.parent.position = winButton.transform.position;

				anim.Play("Win");
			}
			else
			{
				isPlaying = false;

				SetBonusLost();
				anim.Play("Lose");
			}
		});

		anim.Play("Hide");

		InitInternal();
	}

	void Update()
	{
		if(isPlaying)
		{
			leftImage.transform.parent.position = Vector3.MoveTowards(leftImage.transform.parent.position, winButton.transform.position, speed * Time.deltaTime);
			rightImage.transform.parent.position = Vector3.MoveTowards(rightImage.transform.parent.position, winButton.transform.position, speed * Time.deltaTime);

			if(!isWaiting)
			{
				if(Vector3.Distance(leftImage.transform.parent.position, winButton.transform.position) <= minDistanceThreshold)
				{
					isWaiting = true;
					waitingTimer = 0;
				}
			}
			else
			{
				waitingTimer += Time.deltaTime;

				// player is too late
				if(waitingTimer >= opportunitySize)
				{
					isPlaying = false;

					SetBonusLost();
					anim.Play("Lose");
				}
			}
		}
	}

	public void StartBonus(Bonus bonus, float duration, float opportunityWindowSize)
	{
		if(!CheckInitialized())
			return;

		opportunitySize = opportunityWindowSize;
		isWaiting = false;

		duration = duration / 2 - opportunitySize;

		isPlaying = true;
		speed = Vector3.Distance(leftStartPoint.position, winButton.transform.position) / duration;

		// picks bonus color
		List<int> colorIndexes = new List<int>();

		for (int i = 0; i < bonusColors.Length; i++)
			colorIndexes.Add(i);

		colorIndexes.Remove(lastBonusColor);

		lastBonusColor = UnityEngine.Random.Range(0, colorIndexes.Count);
		Color selectedColor = bonusColors[lastBonusColor];

		// configures UI
		leftImage.sprite = bonus.leftPart;
		leftImage.color = selectedColor;
		leftImage.transform.parent.position = leftStartPoint.position;

		rightImage.sprite = bonus.rightPart;
		rightImage.color = selectedColor;
		rightImage.transform.parent.position = rightStartPoint.position;

		anim.Play("Show");
	}
}