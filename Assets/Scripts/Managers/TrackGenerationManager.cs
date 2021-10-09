using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages the generation of the track (obstacle and bonuses)</summary>
public class TrackGenerationManager : BaseBehaviour
{
	[Header("Settings")]
	public int randomizationMemory;
	public float speed, minFrequency, maxFrequency, minOpportunityWindow, maxOpportunityWindow;
	public int obstaclesPerLevel;

	[Header("Scene references")]
	public GameObject[] sideObstacles;
	public GameObject[] centerObstacles;
	public GameObject emptyObstaclePrefab;
	public Transform obstacleSpawnPoint, obstacleDestroyPoint;

	float currentFrequency => Mathf.Lerp(maxFrequency, minFrequency, currentDifficulty);
	float currentOpportunityWindow => Mathf.Lerp(maxOpportunityWindow, minOpportunityWindow, currentDifficulty);
	Transform lastSpawnedObstacle => spawnedObstacle[spawnedObstacle.Count - 1].GetTransform();

	List<Obstacle> spawnedObstacle;
	List<int> lastSideObstacles, lastCenterObstacles;
	Func<float> GetBonusPercentile;
	Action<float, float> GenerateBonus;
	Action GiveScore, ChangeLevel;
	float currentDifficulty, sizeCount, shipControllerZPos;
	int levelObstaclesCount;
	bool gamePaused;

	public void Init(float shipControllerZPos, Action giveScore, Action changeLevel, Action<float, float> generateBonus, Func<float> getBonusPercentile)
	{
		this.shipControllerZPos = shipControllerZPos;

		GiveScore = giveScore;
		ChangeLevel = changeLevel;
		GenerateBonus = generateBonus;
		GetBonusPercentile = getBonusPercentile;

		spawnedObstacle = new List<Obstacle>();
		lastSideObstacles = new List<int>();
		lastCenterObstacles = new List<int>();

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

			// is bonus
			if(item.isEmpty)
			{
				if(item.CheckMethod())
					GenerateBonus(currentFrequency, currentOpportunityWindow);
			}
			else // is obstacle
			{
				if(item.CheckMethod())
					GiveScore();
			}

			item.GetTransform().Translate(0, 0, -speed * Time.deltaTime);
		});

		// clean list
		toDestroy.ForEach(item =>
		{
			spawnedObstacle.Remove(item);
			Destroy(item.GetTransform().gameObject);
		});
	}

	GameObject SpawnNewObstacle()
	{
		bool isCenter = UnityEngine.Random.value <= currentDifficulty;

		List<int> obstacleIndexes = new List<int>();
		GameObject[] selectedObstacles = isCenter ? centerObstacles : sideObstacles;

		for (int i = 0; i < selectedObstacles.Length; i++)
			obstacleIndexes.Add(i);

		// only keep new indexes
		List<int> selectedIndexesHistory = isCenter ? lastCenterObstacles : lastSideObstacles;
		selectedIndexesHistory.ForEach(item => obstacleIndexes.Remove(item));

		int newObstacle = obstacleIndexes[UnityEngine.Random.Range(0, obstacleIndexes.Count)];
		selectedIndexesHistory.Add(newObstacle);

		// keep memory size consistant
		if(selectedIndexesHistory.Count > randomizationMemory)
			selectedIndexesHistory.RemoveAt(0);

		if(isCenter)
			lastCenterObstacles = selectedIndexesHistory;
		else
			lastSideObstacles = selectedIndexesHistory;

		return Instantiate(selectedObstacles[newObstacle], obstacleSpawnPoint.position, Quaternion.identity);
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
		}

		// generates obstacle
		if(PickNextObstacleOrBonus())
		{
			GameObject obstacleObject = SpawnNewObstacle();
			Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos);

			spawnedObstacle.Add(newObstacle);
		}
		else
		{
			// generate empty obstacle because it can't wait until the bonus is done
			GameObject obstacleObject = Instantiate(emptyObstaclePrefab, obstacleSpawnPoint.position, Quaternion.identity);
			Obstacle newObstacle = new Obstacle(obstacleObject.transform, shipControllerZPos, currentFrequency * speed / 2);

			spawnedObstacle.Add(newObstacle);
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
		levelObstaclesCount = 0;

		gamePaused = false;
	}

	public void PauseGame()
	{
		if(!CheckInitialized())
			return;

		gamePaused = true;
	}
}