using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
	public bool bigDoorController;

	private BigDoor[] bdoors;

	public bool gotPos;

	public Vector3 closedPos;

	public Vector3 openPos;

	private Vector3 openPosRelative;

	public bool startOpen;

	private Vector3 targetPos;

	public float speed;

	private bool inPos = true;

	private AudioSource aud;

	public AudioClip openSound;

	public AudioClip closeSound;

	private AudioSource aud2;

	public bool locked;

	public GameObject noPass;

	private NavMeshObstacle nmo;

	public GameObject[] activatedRooms;

	public GameObject[] deactivatedRooms;

	public Light openLight;

	private Door[] allDoors;

	private void Start()
	{
		nmo = GetComponent<NavMeshObstacle>();
		if (nmo != null)
		{
			nmo.enabled = false;
		}
		if (!bigDoorController)
		{
			aud = GetComponent<AudioSource>();
			if (!gotPos)
			{
				gotPos = true;
				closedPos = base.transform.localPosition;
				openPosRelative = base.transform.localPosition + openPos;
				if (startOpen)
				{
					base.transform.localPosition = openPosRelative;
				}
			}
		}
		else
		{
			bdoors = GetComponentsInChildren<BigDoor>();
		}
		aud2 = base.transform.GetChild(0).GetComponent<AudioSource>();
		if (openLight != null && !startOpen)
		{
			openLight.enabled = false;
		}
	}

	private void Update()
	{
		if (bigDoorController || inPos)
		{
			return;
		}
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPos, Time.deltaTime * speed);
		if (!(Vector3.Distance(base.transform.localPosition, targetPos) < 0.1f))
		{
			return;
		}
		base.transform.localPosition = targetPos;
		inPos = true;
		aud.clip = closeSound;
		aud.Play();
		if (!(nmo != null))
		{
			return;
		}
		if (base.transform.localPosition == closedPos)
		{
			nmo.enabled = true;
			if (openLight != null)
			{
				openLight.enabled = true;
			}
		}
		else
		{
			nmo.enabled = false;
		}
	}

	public void Open(bool enemy = false)
	{
		if (!enemy)
		{
			allDoors = Object.FindObjectsOfType<Door>();
			Door[] array = allDoors;
			foreach (Door door in array)
			{
				if (door != null && door != this && door.transform.localPosition != door.closedPos && !door.startOpen)
				{
					DoorController doorController = ((!door.bigDoorController) ? door.transform.parent.GetComponentInChildren<DoorController>() : door.GetComponentInChildren<DoorController>());
					if (doorController != null && doorController.type == 0)
					{
						door.Close();
					}
				}
			}
		}
		if (!bigDoorController)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = openSound;
			aud.Play();
			targetPos = openPosRelative;
			inPos = false;
		}
		else
		{
			BigDoor[] array2 = bdoors;
			foreach (BigDoor bigDoor in array2)
			{
				bigDoor.Open();
			}
		}
		if (activatedRooms.Length > 0)
		{
			GameObject[] array3 = activatedRooms;
			foreach (GameObject gameObject in array3)
			{
				gameObject.SetActive(true);
			}
		}
		if (openLight != null)
		{
			openLight.enabled = true;
		}
	}

	public void Optimize()
	{
		if (deactivatedRooms.Length > 0)
		{
			GameObject[] array = deactivatedRooms;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void Close()
	{
		if (startOpen)
		{
			startOpen = false;
		}
		if (!bigDoorController)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = openSound;
			aud.Play();
			targetPos = closedPos;
			inPos = false;
			return;
		}
		BigDoor[] array = bdoors;
		foreach (BigDoor bigDoor in array)
		{
			bigDoor.Close();
			if (openLight != null)
			{
				bigDoor.openLight = openLight;
			}
		}
	}

	public void Lock()
	{
		locked = true;
		noPass.SetActive(true);
		if (!bigDoorController)
		{
			if (base.transform.localPosition != closedPos)
			{
				Close();
			}
		}
		else
		{
			BigDoor[] array = bdoors;
			foreach (BigDoor bigDoor in array)
			{
				if (bigDoor.open)
				{
					bigDoor.Close();
				}
			}
		}
		aud2.pitch = 0.2f;
		aud2.Play();
	}

	public void Unlock()
	{
		locked = false;
		noPass.SetActive(false);
		aud2.pitch = 0.5f;
		aud2.Play();
	}
}
