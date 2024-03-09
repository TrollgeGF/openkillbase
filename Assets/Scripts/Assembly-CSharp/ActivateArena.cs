using UnityEngine;

public class ActivateArena : MonoBehaviour
{
	public bool onlyWave;

	private bool activated;

	public Door[] doors;

	public GameObject[] enemies;

	private int currentEnemy;

	public bool forEnemy;

	private void OnTriggerEnter(Collider other)
	{
		if ((forEnemy || !(other.gameObject.tag == "Player") || activated) && (!forEnemy || !(other.gameObject.tag == "Enemy") || activated))
		{
			return;
		}
		activated = true;
		if (!onlyWave && !forEnemy)
		{
			MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
			componentInChildren.ArenaMusicStart();
		}
		if (doors.Length > 0)
		{
			Door[] array = doors;
			foreach (Door door in array)
			{
				door.Lock();
			}
			if (enemies.Length > 0)
			{
				Invoke("SpawnEnemy", 1f);
			}
		}
		else if (enemies.Length > 0)
		{
			SpawnEnemy();
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void SpawnEnemy()
	{
		enemies[currentEnemy].SetActive(true);
		currentEnemy++;
		if (currentEnemy < enemies.Length)
		{
			Invoke("SpawnEnemy", 0.1f);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
