using UnityEngine;

public class ItemPlaceZone : MonoBehaviour
{
	public ItemType acceptedItemType;

	public GameObject[] activateOnSuccess;

	public GameObject[] deactivateOnSuccess;

	public GameObject[] activateOnFailure;

	public Door[] doors;

	public Door[] reverseDoors;

	private Collider col;

	private void Start()
	{
		col = GetComponent<Collider>();
		CheckItem();
	}

	public void CheckItem()
	{
		ItemPickUp componentInChildren = GetComponentInChildren<ItemPickUp>();
		if (componentInChildren != null)
		{
			if (componentInChildren.type == acceptedItemType)
			{
				GameObject[] array = activateOnSuccess;
				foreach (GameObject gameObject in array)
				{
					gameObject.SetActive(true);
				}
				GameObject[] array2 = deactivateOnSuccess;
				foreach (GameObject gameObject2 in array2)
				{
					gameObject2.SetActive(false);
				}
				Door[] array3 = doors;
				foreach (Door door in array3)
				{
					door.Open();
				}
				Door[] array4 = reverseDoors;
				foreach (Door door2 in array4)
				{
					door2.Close();
				}
			}
			else
			{
				GameObject[] array5 = activateOnFailure;
				foreach (GameObject gameObject3 in array5)
				{
					gameObject3.SetActive(true);
				}
			}
			col.enabled = false;
			return;
		}
		GameObject[] array6 = activateOnSuccess;
		foreach (GameObject gameObject4 in array6)
		{
			gameObject4.SetActive(false);
		}
		GameObject[] array7 = activateOnFailure;
		foreach (GameObject gameObject5 in array7)
		{
			gameObject5.SetActive(false);
		}
		GameObject[] array8 = deactivateOnSuccess;
		foreach (GameObject gameObject6 in array8)
		{
			gameObject6.SetActive(true);
		}
		Door[] array9 = doors;
		foreach (Door door3 in array9)
		{
			if (door3.transform.localPosition != door3.closedPos)
			{
				door3.Close();
			}
		}
		Door[] array10 = reverseDoors;
		foreach (Door door4 in array10)
		{
			if (door4.transform.localPosition != door4.closedPos)
			{
				door4.Open();
			}
		}
		col.enabled = true;
	}
}
