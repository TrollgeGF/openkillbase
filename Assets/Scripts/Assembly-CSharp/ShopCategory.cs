using UnityEngine;

public class ShopCategory : MonoBehaviour
{
	public string weaponName;

	private void Start()
	{
		if (GameProgressSaver.CheckGear(weaponName) == 0)
		{
			base.gameObject.SetActive(false);
		}
	}
}
