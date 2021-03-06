using TMPro;
using UnityEngine;

/// <summary>Manages the player score</summary>
public class ScoreManager : BaseBehaviour
{
	[Header("Settings")]
	public int scoreDecimals;
	[Range(0, 1)]
	public float mulitplierPerBonus;

	[Header("Scene references")]
	public TextMeshProUGUI scoreDisplay;
	public TextMeshProUGUI multiplierDisplay;

	float currentScoreMultiplier;
	float currentPlayerScore;

	public void Init()
	{
		ResetScore();

		InitInternal();
	}

	string ClampNumberToDecimals(float number)
	{
		string result = number.ToString();

		int decimalCount = result.IndexOf(',') != -1 ? result.Length - result.IndexOf(',') - 1 : 0;

		// truncate decimals
		if(decimalCount >= scoreDecimals)
			result = result.Substring(0, result.IndexOf(',') + 1) + result.Substring(result.IndexOf(',') + 1, scoreDecimals);

		return result;
	}

	void ResetScore()
	{
		currentPlayerScore = 0;
		currentScoreMultiplier = 1;

		scoreDisplay.text = "0";
		multiplierDisplay.gameObject.SetActive(false);
	}

	public void StartGame()
	{
		if(!CheckInitialized())
			return;

		ResetScore();
	}

	public void AddScore(int score)
	{
		if(!CheckInitialized())
			return;

		currentPlayerScore += score * currentScoreMultiplier;

		scoreDisplay.text = ClampNumberToDecimals(currentPlayerScore);
	}

	public void AddMultiplier()
	{
		if(!CheckInitialized())
			return;

		currentScoreMultiplier += mulitplierPerBonus;

		multiplierDisplay.gameObject.SetActive(true);
		multiplierDisplay.text = "x" + ClampNumberToDecimals(currentScoreMultiplier);
	}

	public void CancelMultiplier()
	{
		if(!CheckInitialized())
			return;

		currentScoreMultiplier = 1;

		multiplierDisplay.gameObject.SetActive(false);
		multiplierDisplay.text = "x" + ClampNumberToDecimals(currentScoreMultiplier);
	}

	public float GetCurrentScore()
	{
		if(!CheckInitialized())
			return 0;

		return currentPlayerScore;
	}
}