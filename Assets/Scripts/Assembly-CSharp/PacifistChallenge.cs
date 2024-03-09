using UnityEngine;

public class PacifistChallenge : MonoBehaviour
{
	private void OnEnable()
	{
		ChallengeManager component = GetComponent<ChallengeManager>();
		StyleCalculator componentInChildren = GameObject.FindWithTag("StyleHUD").GetComponentInChildren<StyleCalculator>();
		if (!componentInChildren.enemiesShot)
		{
			component.ChallengeDone();
		}
	}
}
