using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RectTransform;

/// <summary>Displays scores as a curve</summary>
public class ScoresGraph : BaseBehaviour
{
	[Header("Settings")]
	public float animationDuration;
	public float animDelay;

	[Header("Scene references")]
	public RectTransform pointPrefab;
	public RectTransform linePrefab, pointsHolder, linesHolder, highscoreLine;
	[Space]
	public RectTransform lowLimit;
	public RectTransform highLimit, leftLimit, rightLimit;

	RectTransform[] points, lines;
	float minScore, maxScore;

	public void Init(int maxScoresLength)
	{
		points = new RectTransform[maxScoresLength];
		lines = new RectTransform[maxScoresLength - 1];

		// spawn points
		for (int i = 0; i < maxScoresLength; i++)
			points[i] = Instantiate(pointPrefab, pointsHolder);

		// spawn lines
		for (int i = 0; i < maxScoresLength - 1; i++)
			lines[i] = Instantiate(linePrefab, linesHolder);
	}

	float ToPercent(float value, bool firstScore)
	{
		if(firstScore)
			return 0.5f;

		return (value - minScore) / (maxScore - minScore);
	}

	IEnumerator AnimateLine(int lastLineIndex)
	{
		// wait for player to understand what is happening
		yield return new WaitForSecondsRealtime(animDelay);

		// setup line animation 
		float timer = 0;

		RectTransform animatedLine = lines[lastLineIndex];
		RectTransform startPoint = points[lastLineIndex];
		RectTransform endPoint = points[lastLineIndex + 1];

		float angle = Vector3.SignedAngle(Vector3.right, endPoint.position - startPoint.position, Vector3.forward);
		float fullSize = Vector3.Distance(endPoint.position, startPoint.position);

		// set initial state
		animatedLine.SetSizeWithCurrentAnchors(Axis.Horizontal, 0);
		animatedLine.rotation = Quaternion.Euler(0, 0, angle);
		animatedLine.gameObject.SetActive(true);

		// animate line
		while (timer <= animationDuration)
		{
			timer += Time.deltaTime;
			float percentile = timer / animationDuration;

			animatedLine.position = Vector3.Lerp(startPoint.position, endPoint.position, percentile / 2);
			animatedLine.SetSizeWithCurrentAnchors(Axis.Horizontal, Mathf.Lerp(0, fullSize, percentile));

			yield return null;
		}

		animatedLine.position = Vector3.Lerp(startPoint.position, endPoint.position, 0.5f);
		animatedLine.SetSizeWithCurrentAnchors(Axis.Horizontal, fullSize);
	}

	public void DisplayData(List<float> scores, float lastHighscore)
	{
		// hide all points and lines
		foreach (RectTransform point in points)
			point.gameObject.SetActive(false);

		foreach (RectTransform line in lines)
			line.gameObject.SetActive(false);

		// compute min, max and range
		minScore = lastHighscore;
		maxScore = lastHighscore;

		scores.ForEach(item =>
		{
			if(item < minScore)
				minScore = item;

			if(item > maxScore)
				maxScore = item;
		});

		// position highscore line
		float highscorePercent = ToPercent(lastHighscore, scores.Count <= 1);
		highscoreLine.position = Vector3.Lerp(lowLimit.position, highLimit.position, highscorePercent);

		// position points
		float horizontalRange = rightLimit.position.x - leftLimit.position.x;
		float step = horizontalRange / scores.Count - 1;

		for (int i = 0; i < scores.Count; i++)
		{
			float heightPercentile = ToPercent(scores[i], scores.Count <= 1);
			points[i].position = new Vector3(leftLimit.position.x + step * i, Mathf.Lerp(lowLimit.position.y, highLimit.position.y, heightPercentile), 0);
			points[i].gameObject.SetActive(true);
		}

		// position lines
		for (int i = 0; i < scores.Count - 2; i++)
		{
			lines[i].transform.position = Vector3.Lerp(points[i].position, points[i + 1].position, 0.5f);
			lines[i].SetSizeWithCurrentAnchors(Axis.Horizontal, Vector3.Distance(points[i].position, points[i + 1].position));

			float angle = Vector3.SignedAngle(Vector3.right, points[i + 1].position - points[i].position, Vector3.forward);
			lines[i].transform.rotation = Quaternion.Euler(0, 0, angle);

			lines[i].gameObject.SetActive(true);
		}

		if(scores.Count > 1)
			StartCoroutine(AnimateLine(scores.Count - 2));
	}
}