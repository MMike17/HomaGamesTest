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
		StartGame = () =>
		{
			startGame();

			canvas.SetActive(false);
			gameStarted = true;
		};

		highscoreDisplay.text = "Highscore : " + highscore;

		gameStarted = false;

		InitInternal();
	}

	void Update()
	{
		// start on tap
		if(!gameStarted)
		{
			if(Application.isMobilePlatform)
			{
				if(Input.touchCount > 0)
					StartGame();
			}
			else if(Input.GetMouseButtonDown(0))
				StartGame();
		}
	}
}