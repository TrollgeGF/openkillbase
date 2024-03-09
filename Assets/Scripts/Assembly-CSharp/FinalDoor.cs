using UnityEngine;

public class FinalDoor : MonoBehaviour
{
	public Door[] doors;

	public GameObject doorLight;

	public bool startOpen;

	public Material[] offMaterials;

	public Material[] onMaterials;

	private void Start()
	{
		if (doorLight == null)
		{
			doorLight = GetComponentInChildren<Light>().gameObject;
		}
		doorLight.SetActive(false);
		if (startOpen)
		{
			Open();
		}
	}

	public void Open()
	{
		MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		componentInChildren.ArenaMusicEnd();
		Invoke("OpenDoors", 1f);
		GetComponent<AudioSource>().Play();
		doorLight.SetActive(true);
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			int material = GetMaterial(meshRenderer);
			meshRenderer.sharedMaterial = onMaterials[material];
		}
	}

	private int GetMaterial(MeshRenderer mr)
	{
		bool flag = false;
		int num = 0;
		while (!flag)
		{
			if (mr.sharedMaterial == offMaterials[num])
			{
				flag = true;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	private void OpenDoors()
	{
		Door[] array = doors;
		foreach (Door door in array)
		{
			door.Open();
		}
	}
}
