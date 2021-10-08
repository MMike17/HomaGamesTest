using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages the generation of the track (obstacle and bonuses)</summary>
public class TrackGenerationManager : BaseBehaviour
{
	[Header("Settings")]
	public int randomizationMemory;
	public float minSpeed, maxSpeed, minFrequency, maxFrequency;
	public int obstaclesPerLevel;

	[Header("Scene references")]
	public GameObject[] obstacles;
	public GameObject emptyObstaclePrefab;
	public Transform obstacleSpawnPoint, obstacleDestroyPoint;

	float currentSpeed => Mathf.Lerp(minSpeed, maxSpeed, currentDifficulty);
	float currentFrequency => Mathf.Lerp(minFrequency, maxFrequency, currentDifficulty);
	Transform lastSpawnedObstacle => spawnedObstacle[spawnedObstacle.Count - 1].GetTransform();

	List<Obstacle> spawnedObstacle;
	List<int> lastObstacles;
	Func<float> GetBonusPercentile;
	Action<float> GenerateBonus;
	Action GiveScore, ChangeLevel;
	float currentDifficulty, sizeCount, shipControllerZPos;
	int levelObstaclesCount;
	bool gamePaused;

	public void Init(float shipControllerZPos, Action giveScore, Action changeLevel, Action<float> generateBonus, Func<float> getBonusPercentile)
	{
		this.shipControllerZPos = shipControllerZPos;

		GiveScore = giveScore;
		ChangeLevel = changeLevel;
		GenerateBonus = generateBonus;
		GetBonusPercentile = getBonusPercentile;

		spawnedObstacle = new List<Obstacle>();
		lastObstacles = new List<int>();

		gamePaused = true;
		levelObstaclesCount = 0;

		InitInternal();
	}

	void Update()
	{
		if(!initialized || gamePaused)
			return;

		// detect when we need to spawn obstacles
		if(spawnedObstacle.Count > 0)
		{
			float lapsedTime = Vector3.Distance(obstacleSpawnPoint.position, lastSpawnedObstacle.transform.position) / currentSpeed;

			if(lapsedTime >= currentFrequency)
				GenerateNextObstacle();
		}
		else
			GenerateNextObstacle();

		// move obstacles
		List<Obstacle> toDestroy = new List<Obstacle>();

		spawnedObstacle.ForEach(item =>
		{
			// destroys obstacles when they're too far back
			if(item.GetTransform().position.z <= obstacleDestroyPoint.position.z)
				toDestroy.Add(item);

			// give score
			if(item.CheckGiveScore())
				GiveScore();

			item.GetTransform().Translate(0, 0, -currentSpeed * Time.deltaTime);
		});

		// clean list
		toDestroy.ForEach(item =>
		{
			spawnedObstacle.Remove(item);
			Destroy(item.GetTransform().gameObject);
		});
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
		if(lastObstacles.Count > randomizationMemory)
			lastObstacles.RemoveAt(0);

		return newObstacle;
	}

	void GenerateNextObstacle()
	{
		// change level
		if(levelObstaclesCount >= obstaclesPerLevel)
		{
			ChangeLevel();
			levelObstaclesCount = 0;

			Debug.Log("Changed level");
		}

		// should generate bonus
		if(UnityEngine.Random.value >= GetBonusPercentile())
		{
			GenerateBonus(currentFrequency);

			// generate obstacle because it can't wait until the bonus is done
			GameObject obstacleObject = Instantiate(emptyObstaclePrefab, obstacleSpawnPoint.position, Quaternion.identity);
			Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos, true);

			spawnedObstacle.Add(newObstacle);
			Debug.Log("Generated bonus");
		}
		else // should generate obstacle
		{
			int newObstacleIndex = PickNewObstacle();

			GameObject obstacleObject = Instantiate(obstacles[newObstacleIndex], obstacleSpawnPoint.position, Quaternion.identity);
			Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos, false);

			spawnedObstacle.Add(newObstacle);
			Debug.Log("Generated obstacle");
		}

		levelObstaclesCount++;
	}

	public void SetDifficulty(float difficulty)
	{
		if(!CheckInitialized())
			return;

		currentDifficulty = difficulty;
	}

	public void StartGame()
	{
		if(!CheckInitialized())
			return;

		// destroy all obstacles

		gamePaused = false;
	}

	public void PauseGame()
	{
		if(!CheckInitialized())
			return;

		gamePaused = true;
	}
}