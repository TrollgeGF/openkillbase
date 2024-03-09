using UnityEngine;

public class BigDoorOpener : MonoBehaviour
{
	public BigDoor[] bigDoors;

	private void Start()
	{
		BigDoor[] array = bigDoors;
		foreach (BigDoor bigDoor in array)
		{
			bigDoor.Open();
		}
	}

	private void OnEnable()
	{
		BigDoor[] array = bigDoors;
		foreach (BigDoor bigDoor in array)
		{
			bigDoor.Open();
		}
	}

	private void OnDisable()
	{
		BigDoor[] array = bigDoors;
		foreach (BigDoor bigDoor in array)
		{
			bigDoor.Close();
		}
	}
}
