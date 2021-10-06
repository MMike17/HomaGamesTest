using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>Script to test the chromatic aberration in editor above the limit value (max : 1)</summary>
[RequireComponent(typeof(PostProcessVolume))]
public class ChromaticAberationTest : MonoBehaviour
{
	[Range(0, 5f)]
	public float chromaticAmount;

	PostProcessProfile profile;

	void OnDrawGizmos()
	{
		if(profile == null)
			profile = GetComponent<PostProcessVolume>().profile;

		ChromaticAberration aberration;

		if(profile.TryGetSettings<ChromaticAberration>(out aberration))
			aberration.intensity.value = chromaticAmount;
	}
}