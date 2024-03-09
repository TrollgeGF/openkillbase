using UnityEngine;

public class HudController : MonoBehaviour
{
	public bool altHud;

	public bool colorless;

	private GameObject altHudObj;

	private GameObject gunCanvas;

	public GameObject weaponIcon;

	public GameObject styleMeter;

	public GameObject styleInfo;

	private void Start()
	{
		CheckSituation();
		if (PlayerPrefs.GetInt("WeaIco", 1) == 0)
		{
			if (!altHud)
			{
				weaponIcon.transform.localPosition = new Vector3(weaponIcon.transform.localPosition.x, weaponIcon.transform.localPosition.y, 45f);
			}
			else
			{
				weaponIcon.SetActive(false);
			}
		}
		if (!altHud)
		{
			if (PlayerPrefs.GetInt("StyMet", 1) == 0)
			{
				styleMeter.transform.localPosition = new Vector3(styleMeter.transform.localPosition.x, styleMeter.transform.localPosition.y, -9999f);
			}
			if (PlayerPrefs.GetInt("StyInf", 1) == 0)
			{
				styleInfo.transform.localPosition = new Vector3(styleInfo.transform.localPosition.x, styleInfo.transform.localPosition.y, -9999f);
				base.transform.Find("StyleCanvas").GetComponent<AudioSource>().enabled = false;
			}
		}
	}

	public void CheckSituation()
	{
		if (altHud)
		{
			if (altHudObj == null)
			{
				altHudObj = base.transform.GetChild(0).gameObject;
			}
			if (PlayerPrefs.GetInt("AltHud", 1) == 2 && !colorless)
			{
				altHudObj.SetActive(true);
			}
			else if (PlayerPrefs.GetInt("AltHud", 1) == 3 && colorless)
			{
				altHudObj.SetActive(true);
			}
			else
			{
				altHudObj.SetActive(false);
			}
		}
		else if (PlayerPrefs.GetInt("AltHud", 1) != 1)
		{
			if (gunCanvas == null)
			{
				gunCanvas = base.transform.Find("GunCanvas").gameObject;
			}
			gunCanvas.transform.localPosition = new Vector3(gunCanvas.transform.localPosition.x, gunCanvas.transform.localPosition.y, -100f);
		}
		else
		{
			if (gunCanvas == null)
			{
				gunCanvas = base.transform.Find("GunCanvas").gameObject;
			}
			gunCanvas.transform.localPosition = new Vector3(gunCanvas.transform.localPosition.x, gunCanvas.transform.localPosition.y, 1f);
		}
	}
}
