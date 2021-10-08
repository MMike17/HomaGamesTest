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
	public float distanceActivation;
	public float minDistanceThreshold;
	public Color[] bonusColors;

	[Header("Scene references")]
	public Transform leftStartPoint;
	public Transform rightStartPoint;
	public Image leftImage, rightImage;
	public Button winButton;

	Animator anim;
	Action SetBonusLost;
	float speed;
	int lastBonusColor;
	bool isPlaying;

	void OnDrawGizmos()
	{
		if(winButton != null)
			Debug.DrawLine(winButton.transform.position + winButton.transform.right * distanceActivation / 2, winButton.transform.position - winButton.transform.right * distanceActivation / 2, Color.red);
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

			if(Vector3.Distance(leftImage.transform.parent.position, winButton.transform.position) <= distanceActivation)
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

			// player is too late
			if(Vector3.Distance(leftImage.transform.parent.position, winButton.transform.position) <= minDistanceThreshold)
			{
				isPlaying = false;

				SetBonusLost();
				anim.Play("Lose");
			}
		}
	}

	public void StartBonus(Bonus bonus, float delay)
	{
		if(!CheckInitialized())
			return;

		// This is so that bonus animations still happen
		delay /= 2;
		// -= 0.5f;

		isPlaying = true;
		speed = Vector3.Distance(leftStartPoint.position, winButton.transform.position) / delay;

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