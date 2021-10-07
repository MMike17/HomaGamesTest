using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the screen which pops up at the end of a game</summary>
public class EndScreenManager : BaseBehaviour
{
	[Header("Scene references")]
	public TextMeshProUGUI scoreDisplay;
	public GameObject sharePanel;
	public Button shareButton, replayButton;
	[Space]
	public Button closeShareButton;

	public void Init(Action RestartGame)
	{
		shareButton.onClick.AddListener(() => sharePanel.SetActive(true));
		replayButton.onClick.AddListener(() => RestartGame());

		closeShareButton.onClick.AddListener(() => sharePanel.SetActive(false));

		InitInternal();
	}

	public void DisplayData(List<float> scores, float highscore)
	{
		// TODO : display informations

		sharePanel.SetActive(false);

		scoreDisplay.text = scores[scores.Count - 1].ToString();
	}
}