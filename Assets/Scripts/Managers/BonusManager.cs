using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages bonus spawning</summary>
public class BonusManager : BaseBehaviour
{
	[Header("Scene references")]
	public Transform leftSpawnPoint;
	public Transform rightSpawnPoint;

	List<Bonus> spawnedBonus;

	public void Init()
	{
		InitInternal();
	}

	public void SpawnBonus()
	{
		// TODO : Spawn bonus
	}

	[Serializable]
	public class Bonus
	{
		public Sprite leftPart, rightPart;
	}
}