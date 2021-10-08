using UnityEngine;

/// <summary>Class representing a spawned obstacle</summary>
public class Obstacle
{
	Transform obstacle;
	float limit;
	bool isDone, isEmpty;

	public Obstacle(Transform obstacle, float limit, bool isEmpty)
	{
		this.obstacle = obstacle;
		this.limit = limit;
		this.isEmpty = isEmpty;

		isDone = false;
	}

	public bool CheckGiveScore()
	{
		if(isEmpty || isDone)
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