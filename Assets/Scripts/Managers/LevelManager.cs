using System;
using System.Collections;
using UnityEngine;

/// <summary>Manages the level effects</summary>
public class LevelManager : BaseBehaviour
{
	[Header("Settings")]
	public float blendDuration;
	[Space]
	public LevelSettings[] levelSettings;

	[Header("Scene references")]
	public Camera mainCamera;

	Action<float, float> SetMinMaxDifficulty;
	float currentBonusPercent;
	int currentLevel, currentScorePerObstacle;

	public void Init(Action<float, float> setMinMaxDifficulty)
	{
		SetMinMaxDifficulty = setMinMaxDifficulty;

		currentLevel = 0;
		currentBonusPercent = levelSettings[currentLevel].bonusPercent;

		ApplySettings(
			levelSettings[currentLevel].environmentColor,
			levelSettings[currentLevel].shadowColor,
			levelSettings[currentLevel].bonusPercent,
			levelSettings[currentLevel].scorePerObstacle
		);

		InitInternal();
	}

	IEnumerator StartBlending(int nextLevel)
	{
		float timer = 0;
		LevelSettings currentSettings = levelSettings[currentLevel];
		LevelSettings nextSettings = levelSettings[nextLevel];

		while (timer <= blendDuration)
		{
			timer += Time.deltaTime;

			float percentile = timer / blendDuration;

			Color environmentColor = Color.Lerp(currentSettings.environmentColor, nextSettings.environmentColor, percentile);

			ApplySettings(
				environmentColor,
				Color.Lerp(currentSettings.shadowColor, nextSettings.shadowColor, percentile),
				Mathf.Lerp(currentSettings.bonusPercent, nextSettings.bonusPercent, percentile),
				Mathf.RoundToInt(Mathf.Lerp(currentSettings.scorePerObstacle, nextSettings.scorePerObstacle, percentile))
			);

			yield return null;
		}

		ApplySettings(
			nextSettings.environmentColor,
			nextSettings.shadowColor,
			nextSettings.bonusPercent,
			nextSettings.scorePerObstacle
		);

		currentLevel = nextLevel;
	}

	void ApplySettings(Color environmentColor, Color shadowColor, float bonusPercent, int scorePerObstacle)
	{
		mainCamera.backgroundColor = environmentColor;
		RenderSettings.fogColor = environmentColor;
		RenderSettings.subtractiveShadowColor = shadowColor;

		currentBonusPercent = bonusPercent;
		currentScorePerObstacle = scorePerObstacle;
	}

	public void BlendToNewLevel()
	{
		if(!CheckInitialized())
			return;

		int newLevel = UnityEngine.Random.Range(0, levelSettings.Length - 1);

		StartCoroutine(StartBlending(newLevel));
	}

	public float GetBonusPercent()
	{
		if(!CheckInitialized())
			return 0;

		return currentBonusPercent;
	}

	public int GetScorePerObstacle()
	{
		if(!CheckInitialized())
			return 0;

		return currentScorePerObstacle;
	}

	[Serializable]
	public class LevelSettings
	{
		[Range(0, 1)]
		public float minDifficulty, maxDifficulty, bonusPercent;
		public Color environmentColor, shadowColor;
		public int scorePerObstacle;
	}
}