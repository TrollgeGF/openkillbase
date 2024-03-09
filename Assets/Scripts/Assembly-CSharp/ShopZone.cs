using UnityEngine;
using UnityEngine.UI;

public class ShopZone : MonoBehaviour
{
	private GunControl gc;

	private bool inUse;

	private Canvas shopCanvas;

	private Punch pun;

	private AudioSource music;

	public Text moneyText;

	private void Start()
	{
		shopCanvas = GetComponentInChildren<Canvas>();
		shopCanvas.gameObject.SetActive(false);
		music = base.transform.Find("Music").GetComponent<AudioSource>();
		if (moneyText != null)
		{
			moneyText.text = "" + DivideMoney(GameProgressSaver.GetMoney()) + "<color=orange>P</color>";
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.NoWeapon();
			if (pun == null)
			{
				pun = other.GetComponentInChildren<Punch>();
			}
			pun.ShopMode();
			inUse = true;
			shopCanvas.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (gc == null)
			{
				gc = other.GetComponentInChildren<GunControl>();
			}
			gc.YesWeapon();
			inUse = false;
			pun.StopShop();
			shopCanvas.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (!(music != null))
		{
			return;
		}
		if (inUse)
		{
			if (music.pitch < 1f)
			{
				music.pitch = Mathf.MoveTowards(music.pitch, 1f, Time.deltaTime);
			}
		}
		else if (music.pitch > 0f)
		{
			music.pitch = Mathf.MoveTowards(music.pitch, 0f, Time.deltaTime);
		}
	}

	public void UpdateMoney()
	{
		moneyText.text = "" + DivideMoney(GameProgressSaver.GetMoney()) + "<color=orange>P</color>";
		VariationInfo[] componentsInChildren = GetComponentsInChildren<VariationInfo>();
		VariationInfo[] array = componentsInChildren;
		foreach (VariationInfo variationInfo in array)
		{
			variationInfo.UpdateMoney();
		}
	}

	public string DivideMoney(int dosh)
	{
		int num = dosh;
		int num2 = 0;
		int num3 = 0;
		while (num >= 1000)
		{
			num2++;
			num -= 1000;
		}
		while (num2 >= 1000)
		{
			num3++;
			num2 -= 1000;
		}
		if (num3 > 0)
		{
			string text = "" + num3 + ",";
			if (num2 < 10)
			{
				string text2 = text;
				text = text2 + "00" + num2 + ",";
			}
			else if (num2 < 100)
			{
				string text2 = text;
				text = text2 + "0" + num2 + ",";
			}
			else
			{
				text = text + num2 + ",";
			}
			if (num < 10)
			{
				return text + "00" + num;
			}
			if (num < 100)
			{
				return text + "0" + num;
			}
			return text + num;
		}
		if (num2 > 0)
		{
			string text = "" + num2 + ",";
			if (num < 10)
			{
				return text + "00" + num;
			}
			if (num < 100)
			{
				return text + "0" + num;
			}
			return text + num;
		}
		return "" + num;
	}
}
