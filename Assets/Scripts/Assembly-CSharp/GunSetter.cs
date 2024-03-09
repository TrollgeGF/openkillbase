using System.Collections.Generic;
using UnityEngine;

public class GunSetter : MonoBehaviour
{
	public GunControl gunc;

	[Header("Revolver")]
	public GameObject revolverPierce;

	public GameObject revolverRicochet;

	public GameObject revolverBerserker;

	[Header("Shotgun")]
	public GameObject shotgunGrenade;

	public GameObject shotgunPump;

	[Header("Nailgun")]
	public GameObject nailHarpoon;

	private void Awake()
	{
		gunc = GetComponent<GunControl>();
		if (base.enabled)
		{
			ResetWeapons();
		}
	}

	public void ResetWeapons()
	{
		if (gunc == null)
		{
			gunc = GetComponent<GunControl>();
		}
		foreach (List<GameObject> slot in gunc.slots)
		{
			foreach (GameObject item in slot)
			{
				Object.Destroy(item);
			}
			slot.Clear();
		}
		if (PlayerPrefs.GetInt("rev0", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("rev0") > 0)
			{
				gunc.slot1.Add(Object.Instantiate(revolverPierce, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("rev0", 0);
			}
		}
		if (PlayerPrefs.GetInt("rev1", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("rev1") > 0)
			{
				gunc.slot1.Add(Object.Instantiate(revolverBerserker, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("rev1", 0);
			}
		}
		if (PlayerPrefs.GetInt("rev2", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("rev2") > 0)
			{
				gunc.slot1.Add(Object.Instantiate(revolverRicochet, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("rev2", 0);
			}
		}
		if (PlayerPrefs.GetInt("sho0", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("sho0") > 0)
			{
				gunc.slot2.Add(Object.Instantiate(shotgunGrenade, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("sho0", 0);
			}
		}
		if (PlayerPrefs.GetInt("sho1", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("sho1") > 0)
			{
				gunc.slot2.Add(Object.Instantiate(shotgunPump, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("sho1", 0);
			}
		}
		if (PlayerPrefs.GetInt("nai0", 0) == 1)
		{
			if (GameProgressSaver.CheckGear("nai0") > 0)
			{
				gunc.slot3.Add(Object.Instantiate(nailHarpoon, base.transform));
			}
			else
			{
				PlayerPrefs.SetInt("nai0", 0);
			}
		}
		gunc.UpdateWeaponList();
	}

	public void ForceWeapon(string weaponName)
	{
		if (gunc == null)
		{
			gunc = GetComponent<GunControl>();
		}
		switch (weaponName)
		{
		case "rev0":
			gunc.ForceWeapon(revolverPierce);
			break;
		case "rev2":
			gunc.ForceWeapon(revolverRicochet);
			break;
		case "sho0":
			gunc.ForceWeapon(shotgunGrenade);
			break;
		case "sho1":
			gunc.ForceWeapon(shotgunPump);
			break;
		}
	}
}
