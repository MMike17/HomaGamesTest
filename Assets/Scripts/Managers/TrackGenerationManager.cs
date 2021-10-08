using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages the generation of the track (obstacle and bonuses)</summary>
public class TrackGenerationManager : BaseBehaviour
{
	[Header("Settings")]
	public int randomizationMemory;
	public float speed, minFrequency, maxFrequency;
	public int obstaclesPerLevel;

	[Header("Scene references")]
	public GameObject[] obstacles;
	public GameObject emptyObstaclePrefab;
	public Transform startTempObstacleSpawnPoint, obstacleSpawnPoint, obstacleDestroyPoint;

	float currentFrequency => Mathf.Lerp(maxFrequency, minFrequency, currentDifficulty);
	Transform lastSpawnedObstacle => spawnedObstacle[spawnedObstacle.Count - 1].GetTransform();

	List<Obstacle> spawnedObstacle;
	List<int> lastObstacles;
	Func<float> GetBonusPercentile;
	Action<float> GenerateBonus;
	Action GiveScore, ChangeLevel;
	float currentDifficulty, sizeCount, shipControllerZPos;
	int levelObstaclesCount;
	bool gamePaused, isNextAnObstacle, inIntroduction;

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
		inIntroduction = true;
		levelObstaclesCount = 0;

		isNextAnObstacle = PickNextObstacleOrBonus();

		InitInternal();
	}

	void Update()
	{
		if(!initialized || gamePaused)
			return;

		// detect when we need to spawn obstacles
		if(spawnedObstacle.Count > 0)
		{
			float lapsedTime = Vector3.Distance(obstacleSpawnPoint.position, lastSpawnedObstacle.transform.position) / speed;

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

			item.GetTransform().Translate(0, 0, -speed * Time.deltaTime);
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

	bool PickNextObstacleOrBonus()
	{
		return UnityEngine.Random.value >= GetBonusPercentile();
	}

	void GenerateNextObstacle()
	{
		// change level
		if(levelObstaclesCount >= obstaclesPerLevel)
		{
			ChangeLevel();
			levelObstaclesCount = 0;
			inIntroduction = false;

			Debug.Log("Changed level");
		}

		// generates obstacle
		if(isNextAnObstacle)
		{
			int newObstacleIndex = PickNewObstacle();

			GameObject obstacleObject = Instantiate(obstacles[newObstacleIndex], obstacleSpawnPoint.position, Quaternion.identity);
			Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos, false);

			spawnedObstacle.Add(newObstacle);
			Debug.Log("Generated obstacle");
		}

		isNextAnObstacle = PickNextObstacleOrBonus();

		// starts next bonus generation
		if(!isNextAnObstacle)
		{
			DelayedActionsManager.SceduleAction(() =>
				{
					GenerateBonus(currentFrequency);

					// generate obstacle because it can't wait until the bonus is done
					GameObject obstacleObject = Instantiate(emptyObstaclePrefab, obstacleSpawnPoint.position, Quaternion.identity);
					Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos, true);

					spawnedObstacle.Add(newObstacle);
					Debug.Log("Generated bonus");
				},
				currentFrequency / 2);
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
		foreach (Obstacle obstacle in spawnedObstacle)
			Destroy(obstacle.GetTransform().gameObject);

		spawnedObstacle.Clear();

		// reset game settings
		gamePaused = true;
		inIntroduction = true;
		levelObstaclesCount = 0;
		isNextAnObstacle = PickNextObstacleOrBonus();

		gamePaused = false;
	}

	public void PauseGame()
	{
		if(!CheckInitialized())
			return;

		gamePaused = true;
	}
}