using UnityEngine;

/// <summary>Class representing a spawned obstacle</summary>
public class Obstacle
{
	Transform obstacle;
	float limit;
	bool isDone;

	public Obstacle(Transform obstacle, float limit)
	{
		this.obstacle = obstacle;
		this.limit = limit;

		isDone = false;
	}

	public bool CheckGiveScore()
	{
		if(isDone)
			return false;

		if(obstacle.position.z <= limit)
		{
			isDone = true;
			return true;
		}

		return false;
	}

	public Transform GetTransform()
	{
		return obstacle;
	}
}