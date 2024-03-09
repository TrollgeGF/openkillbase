using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public StatsManager sm;

	private bool activated;

	public GameObject toActivate;

	public GameObject[] rooms;

	public GameObject[] roomsToInherit;

	private GameObject[] roomsPriv;

	public List<GameObject> defaultRooms = new List<GameObject>();

	public Door[] doorsToUnlock;

	public List<GameObject> newRooms = new List<GameObject>();

	private int i;

	private GameObject tempRoom;

	private GameObject player;

	private NewMovement nm;

	private float tempRot;

	public GameObject graphic;

	public int restartDeaths;

	public int stylePoints;

	private StyleHUD shud;

	private void Start()
	{
		GameObject[] array = rooms;
		foreach (GameObject item in array)
		{
			defaultRooms.Add(item);
		}
		for (int j = 0; j < defaultRooms.Count; j++)
		{
			newRooms.Add(tempRoom);
			newRooms[j] = Object.Instantiate(defaultRooms[j], defaultRooms[j].transform.position, defaultRooms[j].transform.rotation, defaultRooms[j].transform.parent);
			defaultRooms[j].gameObject.SetActive(false);
			newRooms[j].gameObject.SetActive(true);
			defaultRooms[j].transform.position = new Vector3(defaultRooms[j].transform.position.x + 10000f, defaultRooms[j].transform.position.y, defaultRooms[j].transform.position.z);
		}
		player = GameObject.FindWithTag("Player");
		sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (activated || !(other.gameObject.tag == "Player"))
		{
			return;
		}
		sm = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		sm.currentCheckPoint = this;
		activated = true;
		GetComponent<AudioSource>().Play();
		Object.Destroy(graphic);
		stylePoints = sm.stylePoints;
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
		if (roomsToInherit.Length == 0)
		{
			return;
		}
		GameObject[] array = roomsToInherit;
		foreach (GameObject gameObject in array)
		{
			string text = gameObject.name;
			text = text.Replace("(Clone)", "");
			GameObject targetRoom = null;
			for (int j = 0; j < gameObject.transform.parent.childCount; j++)
			{
				GameObject gameObject2 = gameObject.transform.parent.GetChild(j).gameObject;
				string text2 = gameObject2.name.Replace("(Clone)", "");
				if (text2 == text)
				{
					targetRoom = gameObject2;
				}
			}
			InheritRoom(targetRoom);
		}
	}

	public void OnRespawn()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		player.transform.position = Vector3.one * -1000f;
		this.i = 0;
		sm.kills -= restartDeaths;
		restartDeaths = 0;
		sm.stylePoints = stylePoints;
		if (shud == null)
		{
			shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
		}
		shud.ComboOver();
		if (doorsToUnlock.Length > 0)
		{
			Door[] array = doorsToUnlock;
			foreach (Door door in array)
			{
				door.Unlock();
				if (door.startOpen)
				{
					door.Open();
				}
			}
		}
		ResetRoom();
	}

	private void ResetRoom()
	{
		Vector3 position = newRooms[i].transform.position;
		Object.Destroy(newRooms[i]);
		newRooms[i] = Object.Instantiate(defaultRooms[i], position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
		newRooms[i].SetActive(true);
		if (i + 1 < defaultRooms.Count)
		{
			i++;
			ResetRoom();
			return;
		}
		toActivate.SetActive(true);
		player.transform.position = base.transform.position + base.transform.right * 0.1f;
		player.GetComponent<Rigidbody>().velocity = Vector3.zero;
		if (nm == null)
		{
			nm = player.GetComponent<NewMovement>();
		}
		nm.cc.ResetCamera(base.transform.rotation.eulerAngles.y + 0.1f, true);
		nm.Respawn();
		nm.GetHealth(0, true);
		nm.cc.StopShake();
	}

	public void UpdateRooms()
	{
		Vector3 position = newRooms[i].transform.position;
		Object.Destroy(newRooms[i]);
		newRooms[i] = Object.Instantiate(defaultRooms[i], position, defaultRooms[i].transform.rotation, defaultRooms[i].transform.parent);
		newRooms[i].SetActive(true);
		if (i + 1 < defaultRooms.Count)
		{
			i++;
			UpdateRooms();
		}
		else
		{
			i = 0;
		}
	}

	public void GetRoomToInherit(GameObject roomToInherit, CheckPoint cp)
	{
	}

	public void InheritRoom(GameObject targetRoom)
	{
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		defaultRooms.Add(targetRoom);
		int index = defaultRooms.IndexOf(targetRoom);
		defaultRooms[index].GetComponent<GoreZone>().checkpoint = this;
		newRooms.Add(tempRoom);
		newRooms[index] = Object.Instantiate(defaultRooms[index], defaultRooms[index].transform.position, defaultRooms[index].transform.rotation, defaultRooms[index].transform.parent);
		defaultRooms[index].gameObject.SetActive(false);
		newRooms[index].gameObject.SetActive(true);
		defaultRooms[index].transform.position = new Vector3(defaultRooms[index].transform.position.x + 10000f, defaultRooms[index].transform.position.y, defaultRooms[index].transform.position.z);
	}
}
