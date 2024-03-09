using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
	private StatsManager sman;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	public Door[] toUnlock;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (other.gameObject.GetComponent<NewMovement>().hp <= 0)
			{
				return;
			}
			other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			if (sman == null)
			{
				sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			}
			if (sman.currentCheckPoint != null)
			{
				other.transform.position = sman.currentCheckPoint.transform.position;
				sman.currentCheckPoint.toActivate.SetActive(true);
				Door[] doorsToUnlock = sman.currentCheckPoint.doorsToUnlock;
				foreach (Door door in doorsToUnlock)
				{
					door.Unlock();
				}
			}
			else
			{
				other.transform.position = sman.spawnPos;
				GameObject[] array = toActivate;
				foreach (GameObject gameObject in array)
				{
					gameObject.SetActive(true);
				}
				GameObject[] array2 = toDisactivate;
				foreach (GameObject gameObject2 in array2)
				{
					gameObject2.SetActive(false);
				}
				Door[] array3 = toUnlock;
				foreach (Door door2 in array3)
				{
					door2.Unlock();
				}
			}
			GetComponent<HudMessage>().PlayMessage();
		}
		else if (other.gameObject.layer == 10 || other.gameObject.layer == 9)
		{
			Object.Destroy(other.gameObject);
		}
		else if (other.gameObject.tag == "Enemy")
		{
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			if (!component.dead)
			{
				component.InstaKill();
			}
		}
	}
}
