using UnityEngine;

public class OneKillChallenge : MonoBehaviour
{
	public int kills;

	private void OnEnable()
	{
		ChallengeManager component = GetComponent<ChallengeManager>();
		StatsManager component2 = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		if (component2.kills == kills)
		{
			component.ChallengeDone();
		}
	}
}
