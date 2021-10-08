using System;
using System.Collections.Generic;

/// <summary>Class used to save the player score history</summary>
[Serializable]
public class PlayerScore
{
	public List<float> scoresHistory;
	public float highscore;

	public PlayerScore()
	{
		scoresHistory = new List<float>();
		highscore = 0;
	}

	void TrimHistory(int limitSize)
	{
		if(scoresHistory.Count > limitSize)
			return;

		int loops = scoresHistory.Count - limitSize;

		// limit history size
		for (int i = 0; i < loops; i++)
			scoresHistory.RemoveAt(0);
	}

	public void AddScore(float score, int limitSize)
	{
		scoresHistory.Add(score);

		if(score > highscore)
			highscore = score;

		TrimHistory(limitSize);
	}
}