using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages the generation of the track (obstacle and bonuses)</summary>
public class TrackGenerationManager : BaseBehaviour
{
	[Header("Settings")]
	int randomizationMemory;

	[Header("Scene references")]
	public GameObject[] obstacles;

	List<int> lastObstacles;
	float currentDifficulty;

	public void Init()
	{
		InitInternal();
	}

	void Update()
	{
		if(!initialized)
			return;
	}

	void SpawnObstacle()
	{
		List<int> obstacleIndexes = new List<int>();

		for (int i = 0; i < obstacles.Length; i++)
			obstacleIndexes.Add(i);

		// only keep new indexes
		lastObstacles.ForEach(item => obstacleIndexes.Remove(item));

		int newObstacle = Random.Range(0, obstacleIndexes.Count - 1);
		obstacleIndexes.Add(newObstacle);

		// keep memory size consistant
		if(obstacleIndexes.Count > randomizationMemory)
			obstacleIndexes.RemoveAt(0);

		// TODO : Spawn obstacle
	}

	public void SetDifficulty(float difficulty)
	{
		if(!CheckInitialized())
			return;

		currentDifficulty = difficulty;
	}
}