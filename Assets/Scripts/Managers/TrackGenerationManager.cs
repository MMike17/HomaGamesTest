using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages the generation of the track (obstacle and bonuses)</summary>
public class TrackGenerationManager : BaseBehaviour
{
	[Header("Settings")]
	public int randomizationMemory;
	public float minSpeed, maxSpeed, minFrequency, maxFrequency;
	public int minSize, maxSize;

	[Header("Scene references")]
	public GameObject[] obstacles;

	float currentSpeed => Mathf.Lerp(minSpeed, maxSpeed, currentDifficulty);
	float currentFrequency => Mathf.Lerp(minFrequency, maxFrequency, currentDifficulty);

	List<int> lastObstacles;
	Func<float> GetBonusPercentile;
	float currentDifficulty, currentSize, sizeCount;

	public void Init(Func<float> getBonusPercentile)
	{
		GetBonusPercentile = getBonusPercentile;

		InitInternal();
	}

	void Update()
	{
		if(!initialized)
			return;
	}

	void ComputeCurrentSize()
	{
		float proportionalSize = Mathf.Lerp(minSize, maxSize, currentDifficulty);
		sizeCount += proportionalSize - minSize;

		// switch between two sizes depending on additive lerp frequence
		if(sizeCount >= maxSize)
		{
			currentSize = maxSize;
			sizeCount = sizeCount - maxSize + minSize;
		}
		else
			currentSize = minSize;
	}

	int PickNewObstacle()
	{
		List<int> obstacleIndexes = new List<int>();

		for (int i = 0; i < obstacles.Length; i++)
			obstacleIndexes.Add(i);

		// only keep new indexes
		lastObstacles.ForEach(item => obstacleIndexes.Remove(item));

		int newObstacle = UnityEngine.Random.Range(0, obstacleIndexes.Count - 1);
		obstacleIndexes.Add(newObstacle);

		// keep memory size consistant
		if(obstacleIndexes.Count > randomizationMemory)
			obstacleIndexes.RemoveAt(0);

		return newObstacle;
	}

	void GenerateNextObstacle()
	{
		// should generate bonus
		if(UnityEngine.Random.value >= GetBonusPercentile())
		{

		}
		else // should generate obstacle
		{
			ComputeCurrentSize();

			int newObstacleIndex = PickNewObstacle();

			// TODO : Spawn obstacle
		}
	}

	public void SetDifficulty(float difficulty)
	{
		if(!CheckInitialized())
			return;

		currentDifficulty = difficulty;
	}
}