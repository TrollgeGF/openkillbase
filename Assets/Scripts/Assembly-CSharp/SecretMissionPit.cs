using UnityEngine;

public class SecretMissionPit : MonoBehaviour
{
	public int missionNumber;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			GameProgressSaver.SetSecretMission(missionNumber);
		}
	}
}
