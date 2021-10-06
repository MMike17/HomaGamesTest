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
	Action<float> SetBonusPercent;
	int currentLevel;

	public void Init(Action<float, float> setMinMaxDifficulty, Action<float> setBonusPercent)
	{
		SetMinMaxDifficulty = setMinMaxDifficulty;
		SetBonusPercent = setBonusPercent;

		currentLevel = 0;

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

			Color environmentColor = Color.Lerp(currentSettings.environmentColor, nextSettings.environmentColor, timer / blendDuration);

			mainCamera.backgroundColor = environmentColor;
			RenderSettings.fogColor = environmentColor;

			RenderSettings.subtractiveShadowColor = Color.Lerp(currentSettings.shadowColor, nextSettings.shadowColor, timer / blendDuration);

			yield return null;
		}

		mainCamera.backgroundColor = nextSettings.environmentColor;
		RenderSettings.fogColor = nextSettings.environmentColor;
		RenderSettings.subtractiveShadowColor = nextSettings.shadowColor;
	}

	public void BlendToNewLevel()
	{
		if(!CheckInitialized())
			return;

		int newLevel = UnityEngine.Random.Range(0, levelSettings.Length - 1);

		StartCoroutine(StartBlending(newLevel));
	}

	[Serializable]
	public class LevelSettings
	{
		[Range(0, 1)]
		public float minDifficulty, maxDifficulty, bonusPercent;
		public Color environmentColor, shadowColor;
	}
}