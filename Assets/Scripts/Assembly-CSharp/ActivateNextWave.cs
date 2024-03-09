using UnityEngine;
using UnityEngine.Audio;

public class ActivateNextWave : MonoBehaviour
{
	public bool lastWave;

	private bool activated;

	public int deadEnemies;

	public int enemyCount;

	public GameObject[] nextEnemies;

	private int currentEnemy;

	public Door[] doors;

	private int currentDoor;

	public GameObject[] toActivate;

	private bool objectsActivated;

	public bool needNextRoomInfo;

	public Door doorForward;

	private float slowDown = 1f;

	public AudioMixer allSounds;

	public bool forEnemies;

	public KillChallenge killChallenge;

	private void FixedUpdate()
	{
		if (activated || deadEnemies != enemyCount)
		{
			return;
		}
		activated = true;
		if (!lastWave)
		{
			Invoke("SpawnEnemy", 1f);
			return;
		}
		Invoke("EndWaves", 1f);
		if (!forEnemies)
		{
			GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().SlowDown(0.15f);
		}
	}

	private void SpawnEnemy()
	{
		nextEnemies[currentEnemy].SetActive(true);
		currentEnemy++;
		if (currentEnemy < nextEnemies.Length)
		{
			Invoke("SpawnEnemy", 0.1f);
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void EndWaves()
	{
		if (toActivate.Length > 0 && !objectsActivated)
		{
			GameObject[] array = toActivate;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			objectsActivated = true;
			EndWaves();
			return;
		}
		if (currentDoor < doors.Length)
		{
			doors[currentDoor].Unlock();
			if (doors[currentDoor] == doorForward)
			{
				doors[currentDoor].Open();
			}
			currentDoor++;
			Invoke("EndWaves", 0.1f);
			return;
		}
		if (needNextRoomInfo)
		{
			GetComponentInParent<NextRoomInfo>().nextRoom.SetActive(true);
		}
		if (!forEnemies)
		{
			MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
			componentInChildren.ArenaMusicEnd();
			slowDown = 1f;
			Time.timeScale = 1f;
			allSounds.SetFloat("allPitch", slowDown);
		}
		if (killChallenge != null)
		{
			killChallenge.Done();
		}
		Object.Destroy(this);
	}
}
