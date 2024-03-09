using UnityEngine;

public class GearCheckEnabler : MonoBehaviour
{
	public string gear;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	private void Start()
	{
		int num = GameProgressSaver.CheckGear(gear);
		if (num > 0)
		{
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
		}
	}
}
