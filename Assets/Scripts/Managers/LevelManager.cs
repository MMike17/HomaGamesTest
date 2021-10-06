using System;
using UnityEngine;

/// <summary>Manages the level effects</summary>
public class LevelManager : BaseBehaviour
{
	[Header("Settings")]
	public LevelSettings[] levelSettings;

	[Serializable]
	public class LevelSettings
	{
		public float minDifficulty, maxDifficulty;
		[Range(0, 1)]
		public float bonusPercent;
		public Color environmentColor, shadowColor;
	}
}