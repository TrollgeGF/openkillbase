using UnityEngine;

public class FinalDoorOpener : MonoBehaviour
{
	public bool startTimer;

	public bool startMusic;

	private void Awake()
	{
		FinalDoor componentInParent = GetComponentInParent<FinalDoor>();
		if (componentInParent != null)
		{
			componentInParent.Open();
		}
		Invoke("GoTime", 1f);
	}

	private void GoTime()
	{
		if (startTimer)
		{
			GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().StartTimer();
		}
		if (startMusic)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().StartMusic();
		}
	}
}
