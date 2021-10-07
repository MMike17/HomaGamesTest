using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the screen which pops up at the end of a game</summary>
public class EndScreenManager : BaseBehaviour
{
	[Header("Scene references")]
	public Button shareButton;
	public Button replayButton;

	public void Init()
	{
		InitInternal();
	}

	public void DisplayData(List<float> scores, float highscore)
	{
		// TODO : display informations
	}
}