using System;
using System.Collections.Generic;

/// <summary>Class used to save the player score history</summary>
[Serializable]
public class PlayerScore
{
	public List<float> scoresHistory { get; private set; }

	public PlayerScore()
	{
		scoresHistory = new List<float>();
	}

	public void TrimHistory(int limitSize)
	{
		if(scoresHistory.Count > limitSize)
			return;

		int loops = scoresHistory.Count - limitSize;

		// limit history size
		for (int i = 0; i < loops; i++)
			scoresHistory.RemoveAt(0);
	}
}