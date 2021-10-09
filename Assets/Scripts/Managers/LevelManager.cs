using System;
using System.Collections;
using System.Collections.Generic;
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
	public Light mainLight;

	Action<float, float> SetMinMaxDifficulty;
	Action<int> SetObstacleCount;
	float currentBonusPercent;
	int currentLevel, currentScorePerObstacle;

	public void Init(Action<float, float> setMinMaxDifficulty, Action<int> setObstacleCount)
	{
		SetMinMaxDifficulty = setMinMaxDifficulty;
		SetObstacleCount = setObstacleCount;

		currentLevel = 0;
		currentBonusPercent = levelSettings[currentLevel].bonusPercent;

		ApplySettings(
			levelSettings[currentLevel].environmentColor,
			levelSettings[currentLevel].lightColor,
			levelSettings[currentLevel].bonusPercent,
			levelSettings[currentLevel].scorePerObstacle,
			levelSettings[currentLevel].minDifficulty,
			levelSettings[currentLevel].maxDifficulty,
			levelSettings[currentLevel].obstaclesPerLevel
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
				Color.Lerp(currentSettings.lightColor, nextSettings.lightColor, percentile),
				Mathf.Lerp(currentSettings.bonusPercent, nextSettings.bonusPercent, percentile),
				Mathf.RoundToInt(Mathf.Lerp(currentSettings.scorePerObstacle, nextSettings.scorePerObstacle, percentile)),
				Mathf.Lerp(currentSettings.minDifficulty, nextSettings.minDifficulty, percentile),
				Mathf.Lerp(currentSettings.maxDifficulty, nextSettings.maxDifficulty, percentile),
				nextSettings.obstaclesPerLevel
			);

			yield return null;
		}

		ApplySettings(
			nextSettings.environmentColor,
			nextSettings.lightColor,
			nextSettings.bonusPercent,
			nextSettings.scorePerObstacle,
			nextSettings.minDifficulty,
			nextSettings.maxDifficulty,
			nextSettings.obstaclesPerLevel
		);

		currentLevel = nextLevel;
	}

	void ApplySettings(Color environmentColor, Color lightColor, float bonusPercent, int scorePerObstacle, float minDifficulty, float maxDifficulty, int obstacleCount)
	{
		mainCamera.backgroundColor = environmentColor;
		RenderSettings.fogColor = environmentColor;
		mainLight.color = lightColor;

		currentBonusPercent = bonusPercent;
		currentScorePerObstacle = scorePerObstacle;

		SetMinMaxDifficulty(minDifficulty, maxDifficulty);
		SetObstacleCount(obstacleCount);
	}

	public void BlendToNewLevel()
	{
		if(!CheckInitialized())
			return;

		List<int> levelIndexes = new List<int>();

		for (int i = 0; i < levelSettings.Length; i++)
			levelIndexes.Add(i);

		levelIndexes.Remove(currentLevel);
		int newLevel = levelIndexes[UnityEngine.Random.Range(0, levelIndexes.Count)];

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
		public Color environmentColor, lightColor;
		public int scorePerObstacle, obstaclesPerLevel;
	}
}