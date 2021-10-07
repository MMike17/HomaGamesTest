using System;
using TMPro;
using UnityEngine;

/// <summary>Manages the main menu</summary>
public class MainMenuManager : BaseBehaviour
{
	[Header("Scene references")]
	public GameObject canvas;
	public TextMeshProUGUI highscoreDisplay;

	Action StartGame;
	bool gameStarted;

	public void Init(Action startGame, float highscore)
	{
		StartGame = startGame + (() => canvas.SetActive(false));
		highscoreDisplay.text = "Highscore : " + highscore;

		gameStarted = false;

		InitInternal();
	}

	void Update()
	{
		// start on tap
		if(!gameStarted)
		{
			if(Input.touchCount > 0)
				StartGame();

			gameStarted = true;
		}
	}
}