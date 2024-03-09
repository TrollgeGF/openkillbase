using UnityEngine;

public class SlideLengthChallenge : MonoBehaviour
{
	public float slideLength;

	private void OnEnable()
	{
		NewMovement component = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
		if (component.longestSlide >= slideLength)
		{
			GetComponent<ChallengeManager>().ChallengeDone();
		}
	}
}
