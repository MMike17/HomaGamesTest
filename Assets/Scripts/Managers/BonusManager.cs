using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Manages bonus spawning</summary>
public class BonusManager : BaseBehaviour
{
	[Header("Settings")]
	public int randomizationMemory;
	public Bonus[] bonuses;

	[Header("Scene references")]
	public Transform bonusHolder;
	public Transform leftSpawnPoint, rightSpawnPoint;
	public BonusGameUI bonusGameUI;

	List<int> lastBonuses;

	public void Init(Action<bool> SetBonusState)
	{
		lastBonuses = new List<int>();

		bonusGameUI.Init(
			() => SetBonusState(true),
			() => SetBonusState(false)
		);

		InitInternal();
	}

	int GetNextBonusIndex()
	{
		List<int> bonusIndexes = new List<int>();

		for (int i = 0; i < bonuses.Length; i++)
			bonusIndexes.Add(i);

		// only keep new indexes
		lastBonuses.ForEach(item => bonusIndexes.Remove(item));

		int newBonus = UnityEngine.Random.Range(0, bonusIndexes.Count - 1);
		bonusIndexes.Add(newBonus);

		// keep memory size constant
		if(lastBonuses.Count > randomizationMemory)
			lastBonuses.RemoveAt(0);

		return newBonus;
	}

	public void SpawnBonus(float delay)
	{
		if(!CheckInitialized())
			return;

		int nextBonusIndex = GetNextBonusIndex();
		bonusGameUI.StartBonus(bonuses[nextBonusIndex], delay);
	}

	[Serializable]
	public class Bonus
	{
		public Sprite leftPart, rightPart;
	}
}