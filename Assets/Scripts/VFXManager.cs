using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>Manages the visual effects (post processing)</summary>
public class VFXManager : BaseBehaviour
{
	// TODO : Set effects in editor

	[Header("Settings")]
	public PostProcessAnimSettings bonusSettings;
	public PostProcessAnimSettings newHighSettings, passedObstacleSettings;

	[Header("Scene references")]
	public PostProcessVolume volume;

	PostProcessProfile profile;
	float timer;

	public void Init()
	{
		profile = volume.profile;

		InitInternal();
	}

	public void BonusAnim()
	{
		StartCoroutine(StartAnimation(bonusSettings));
	}

	public void NewHighAnim()
	{
		StartCoroutine(StartAnimation(newHighSettings));
	}

	public void PassedObstacleAnim()
	{
		StartCoroutine(StartAnimation(passedObstacleSettings));
	}

	IEnumerator StartAnimation(PostProcessAnimSettings settings)
	{
		timer = 0;

		while (timer <= settings.duration)
		{
			timer += Time.deltaTime;
			yield return null;
		}
	}

	[Serializable]
	public class PostProcessAnimSettings
	{
		public float duration;
		[Space]
		public float chromaticAberrationAmount;
		public AnimationCurve chromaticAberrationCurve;
		[Space]
		public float distortionAmount;
		public AnimationCurve distortionCurve;
	}
}