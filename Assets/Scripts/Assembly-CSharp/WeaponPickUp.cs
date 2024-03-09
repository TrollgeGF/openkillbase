using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
	public GameObject weapon;

	public int inventorySlot;

	public GunSetter gs;

	private GunControl gc;

	public string pPref;

	private void Start()
	{
		inventorySlot--;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			gc = collision.gameObject.GetComponentInChildren<GunControl>();
			GotActivated();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			gc = other.gameObject.GetComponentInChildren<GunControl>();
			GotActivated();
		}
	}

	private void GotActivated()
	{
		gc.noWeapons = false;
		if (gs != null)
		{
			gs.enabled = true;
			gs.ResetWeapons();
		}
		bool flag = false;
		for (int i = 0; i < gc.slots[inventorySlot].Count; i++)
		{
			if (gc.slots[inventorySlot][i].name == weapon.name + "(Clone)")
			{
				flag = true;
			}
		}
		if (!flag)
		{
			GameObject item = Object.Instantiate(weapon, gc.transform);
			gc.slots[inventorySlot].Add(item);
			gc.ForceWeapon(weapon);
			gc.noWeapons = false;
			gc.UpdateWeaponList();
		}
		PlayerPrefs.SetInt(pPref, 1);
		GameProgressSaver.AddGear(pPref);
		Object.Destroy(base.gameObject);
	}
}
