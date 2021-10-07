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
	public GameObject canvas, sharePanel;
	public Button shareButton, replayButton;
	[Space]
	public ScoresGraph scoresGraph;
	[Space]
	public Button closeShareButton;

	public void Init(int maxScoresLength, Action RestartGame)
	{
		shareButton.onClick.AddListener(() => sharePanel.SetActive(true));
		replayButton.onClick.AddListener(() => RestartGame());

		scoresGraph.Init(maxScoresLength);

		closeShareButton.onClick.AddListener(() => sharePanel.SetActive(false));

		canvas.SetActive(false);

		InitInternal();
	}

	public void DisplayData(List<float> scores, float lastHighScore)
	{
		canvas.SetActive(true);
		sharePanel.SetActive(false);

		scoreDisplay.text = scores[scores.Count - 1].ToString();

		scoresGraph.DisplayData(scores, lastHighScore);
	}
}