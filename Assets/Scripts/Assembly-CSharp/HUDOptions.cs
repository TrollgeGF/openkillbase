using UnityEngine;
using UnityEngine.UI;

public class HUDOptions : MonoBehaviour
{
	public Dropdown hudType;

	private HudController[] hudCons;

	private HudController mainCon;

	public Toggle weaponIcon;

	public Toggle styleMeter;

	public Toggle styleInfo;

	private Crosshair crosshair;

	public Dropdown crossHairType;

	public Dropdown crossHairColor;

	public Dropdown crossHairHud;

	private void Start()
	{
		crosshair = GetComponentInChildren<Crosshair>();
		crossHairType.value = PlayerPrefs.GetInt("CroHai", 1);
		crossHairType.RefreshShownValue();
		crossHairColor.value = PlayerPrefs.GetInt("CroCol", 1);
		crossHairColor.RefreshShownValue();
		crossHairHud.value = PlayerPrefs.GetInt("Chud", 2);
		crossHairHud.RefreshShownValue();
		hudType.value = PlayerPrefs.GetInt("AltHud", 1);
		hudType.RefreshShownValue();
		if (PlayerPrefs.GetInt("WeaIco", 1) == 0)
		{
			weaponIcon.isOn = false;
		}
		if (PlayerPrefs.GetInt("StyMet", 1) == 0)
		{
			styleMeter.isOn = false;
		}
		if (PlayerPrefs.GetInt("StyInf", 1) == 0)
		{
			styleInfo.isOn = false;
		}
	}

	public void CrossHairType(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("CroHai", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void CrossHairColor(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("CroCol", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void CrossHairHud(int stuff)
	{
		if (crosshair == null)
		{
			crosshair = GetComponentInChildren<Crosshair>();
		}
		PlayerPrefs.SetInt("Chud", stuff);
		if (crosshair != null)
		{
			crosshair.CheckCrossHair();
		}
	}

	public void HudType(int stuff)
	{
		if (hudCons == null || hudCons.Length < 3)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		PlayerPrefs.SetInt("AltHud", stuff);
		HudController[] array = hudCons;
		foreach (HudController hudController in array)
		{
			hudController.CheckSituation();
		}
		GetComponent<OptionsMenuToManager>().CheckEasterEgg();
	}

	public void WeaponIcon(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 2)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("WeaIco", 1);
			HudController[] array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.weaponIcon.transform.localPosition = new Vector3(hudController.weaponIcon.transform.localPosition.x, hudController.weaponIcon.transform.localPosition.y, 45f);
				}
				else
				{
					hudController.weaponIcon.SetActive(true);
				}
			}
			return;
		}
		PlayerPrefs.SetInt("WeaIco", 0);
		HudController[] array2 = hudCons;
		foreach (HudController hudController2 in array2)
		{
			if (!hudController2.altHud)
			{
				hudController2.weaponIcon.transform.localPosition = new Vector3(hudController2.weaponIcon.transform.localPosition.x, hudController2.weaponIcon.transform.localPosition.y, -9999f);
			}
			else
			{
				hudController2.weaponIcon.SetActive(false);
			}
		}
	}

	public void StyleMeter(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 2)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("StyMet", 1);
			HudController[] array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.styleMeter.transform.localPosition = new Vector3(hudController.styleMeter.transform.localPosition.x, hudController.styleMeter.transform.localPosition.y, 0f);
				}
			}
			return;
		}
		PlayerPrefs.SetInt("StyMet", 0);
		HudController[] array2 = hudCons;
		foreach (HudController hudController2 in array2)
		{
			if (!hudController2.altHud)
			{
				hudController2.styleMeter.transform.localPosition = new Vector3(hudController2.styleMeter.transform.localPosition.x, hudController2.styleMeter.transform.localPosition.y, -9999f);
			}
		}
	}

	public void StyleInfo(bool stuff)
	{
		if (hudCons == null || hudCons.Length < 2)
		{
			hudCons = Object.FindObjectsOfType<HudController>();
		}
		if (stuff)
		{
			PlayerPrefs.SetInt("StyInf", 1);
			HudController[] array = hudCons;
			foreach (HudController hudController in array)
			{
				if (!hudController.altHud)
				{
					hudController.styleInfo.transform.localPosition = new Vector3(hudController.styleInfo.transform.localPosition.x, hudController.styleInfo.transform.localPosition.y, 0f);
					hudController.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = true;
				}
			}
			return;
		}
		PlayerPrefs.SetInt("StyInf", 0);
		HudController[] array2 = hudCons;
		foreach (HudController hudController2 in array2)
		{
			if (!hudController2.altHud)
			{
				hudController2.styleInfo.transform.localPosition = new Vector3(hudController2.styleInfo.transform.localPosition.x, hudController2.styleInfo.transform.localPosition.y, -9999f);
				hudController2.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = false;
			}
		}
	}
}
