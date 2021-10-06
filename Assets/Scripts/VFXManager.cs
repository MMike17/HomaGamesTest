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
	ChromaticAberration aberration;
	LensDistortion distortion;
	Bloom bloom;
	float timer;

	public void Init()
	{
		profile = volume.profile;

		bool hasError = false;

		if(!profile.TryGetSettings<ChromaticAberration>(out aberration))
			hasError = true;

		if(!profile.TryGetSettings<LensDistortion>(out distortion))
			hasError = true;

		if(!profile.TryGetSettings<Bloom>(out bloom))
			hasError = true;

		if(hasError)
			Debug.LogError("Couldn't get Post Processing setting");

		InitInternal();
	}

	public void BonusAnim()
	{
		if(!CheckInitialized())
			return;

		StartCoroutine(StartAnimation(bonusSettings));
	}

	public void NewHighAnim()
	{
		if(!CheckInitialized())
			return;

		StartCoroutine(StartAnimation(newHighSettings));
	}

	public void PassedObstacleAnim()
	{
		if(!CheckInitialized())
			return;

		StartCoroutine(StartAnimation(passedObstacleSettings));
	}

	IEnumerator StartAnimation(PostProcessAnimSettings settings)
	{
		timer = 0;

		while (timer <= settings.duration)
		{
			timer += Time.deltaTime;

			aberration.intensity.value = settings.GetChromaticAberation(timer);
			distortion.intensity.value = settings.GetDistortion(timer);
			bloom.intensity.value = settings.GetBloom(timer);

			yield return null;
		}

		aberration.intensity.value = settings.GetChromaticAberation(settings.duration);
		distortion.intensity.value = settings.GetDistortion(settings.duration);
		bloom.intensity.value = settings.GetBloom(settings.duration);
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
		[Space]
		public float bloomAmount;
		public AnimationCurve bloomCurve;

		public float GetChromaticAberation(float timer)
		{
			return chromaticAberrationCurve.Evaluate(timer / duration) * chromaticAberrationAmount;
		}

		public float GetDistortion(float timer)
		{
			return distortionCurve.Evaluate(timer / duration) * distortionAmount;
		}

		public float GetBloom(float timer)
		{
			return bloomCurve.Evaluate(timer / duration) * bloomAmount;
		}
	}
}