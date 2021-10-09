using System;
using UnityEngine;

/// <summary>Class representing a spawned obstacle</summary>
public class Obstacle
{
	public Func<bool> CheckMethod { get; private set; }
	public bool isEmpty => _isEmpty;

	Transform obstacle;
	float playerZ, checkDistance;
	bool isDone, _isEmpty;

	/// <summary>Creates new real obstacle</summary>
	public Obstacle(Transform obstacle, float playerZ)
	{
		this.obstacle = obstacle;
		this.playerZ = playerZ;

		CheckMethod = () =>
		{
			if(_isEmpty || isDone)
				return false;

			if(obstacle.position.z <= playerZ)
			{
				isDone = true;
				return true;
			}

			return false;
		};

		isDone = false;
		_isEmpty = false;
	}

	/// <summary>Creates new empty obstacle (for bonus)</summary>
	public Obstacle(Transform obstacle, float playerZ, float checkDistance)
	{
		this.obstacle = obstacle;
		this.playerZ = playerZ;
		this.checkDistance = checkDistance;

		CheckMethod = () =>
		{
			if(!_isEmpty || isDone)
				return false;

			if(obstacle.position.z <= playerZ + checkDistance)
			{
				isDone = true;
				return true;
			}

			return false;
		};

		isDone = false;
		_isEmpty = true;
	}

	public Transform GetTransform()
	{
		return obstacle;
	}
}