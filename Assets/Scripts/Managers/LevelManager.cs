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

			mainCamera.backgroundColor = environmentColor;
			RenderSettings.fogColor = environmentColor;

			RenderSettings.subtractiveShadowColor = Color.Lerp(currentSettings.shadowColor, nextSettings.shadowColor, percentile);

			currentBonusPercent = Mathf.Lerp(currentSettings.bonusPercent, nextSettings.bonusPercent, percentile);
			currentScorePerObstacle = Mathf.RoundToInt(Mathf.Lerp(currentSettings.scorePerObstacle, nextSettings.scorePerObstacle, percentile));

			yield return null;
		}

		mainCamera.backgroundColor = nextSettings.environmentColor;
		RenderSettings.fogColor = nextSettings.environmentColor;
		RenderSettings.subtractiveShadowColor = nextSettings.shadowColor;

		currentBonusPercent = nextSettings.bonusPercent;
		currentScorePerObstacle = nextSettings.scorePerObstacle;

		currentLevel = nextLevel;
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

	[Serializable]
	public class LevelSettings
	{
		[Range(0, 1)]
		public float minDifficulty, maxDifficulty, bonusPercent;
		public Color environmentColor, shadowColor;
		public int scorePerObstacle;
	}
}