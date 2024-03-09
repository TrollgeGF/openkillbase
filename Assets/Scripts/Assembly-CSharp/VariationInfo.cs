using UnityEngine;
using UnityEngine.UI;

public class VariationInfo : MonoBehaviour
{
	public GameObject varPage;

	private int money;

	public Text moneyText;

	public int cost;

	public Text costText;

	public ShopButton buyButton;

	private Text buttonText;

	public GameObject buySound;

	public Button equipButton;

	private GameObject equipImage;

	public bool alreadyOwned;

	public string weaponName;

	private GunSetter gs;

	private void Start()
	{
		buttonText = buyButton.GetComponentInChildren<Text>();
		buyButton.variationInfo = this;
		equipImage = equipButton.transform.GetChild(0).gameObject;
		if (GameProgressSaver.CheckGear(weaponName) > 0)
		{
			alreadyOwned = true;
		}
		UpdateMoney();
	}

	private void OnEnable()
	{
		UpdateMoney();
	}

	public void UpdateMoney()
	{
		if (!alreadyOwned)
		{
			money = GameProgressSaver.GetMoney();
			moneyText.text = DivideMoney(money) + "<color=orange>P</color>";
			if (cost > money)
			{
				costText.text = "<color=red>" + DivideMoney(cost) + "P</color>";
				if (buttonText == null)
				{
					buttonText = buyButton.GetComponentInChildren<Text>();
				}
				buttonText.text = costText.text;
				buyButton.failure = true;
				buyButton.GetComponent<Button>().interactable = false;
				buyButton.GetComponent<Image>().color = Color.red;
			}
			else
			{
				costText.text = DivideMoney(cost) + "<color=orange>P</color>";
				if (buttonText == null)
				{
					buttonText = buyButton.GetComponentInChildren<Text>();
				}
				buttonText.text = costText.text;
				buyButton.failure = false;
				buyButton.GetComponent<Button>().interactable = true;
				buyButton.GetComponent<Image>().color = Color.white;
			}
		}
		else
		{
			costText.text = "ALREADY OWNED";
			if (buttonText == null)
			{
				buttonText = buyButton.GetComponentInChildren<Text>();
			}
			buttonText.text = costText.text;
			buyButton.failure = true;
			buyButton.GetComponent<Button>().interactable = false;
			buyButton.GetComponent<Image>().color = Color.white;
			equipButton.interactable = true;
			if (PlayerPrefs.GetInt(weaponName, 0) > 0)
			{
				equipImage.SetActive(true);
			}
		}
	}

	public void WeaponBought()
	{
		alreadyOwned = true;
		Object.Instantiate(buySound);
		GameProgressSaver.AddMoney(cost * -1);
		GameProgressSaver.AddGear(weaponName);
		PlayerPrefs.SetInt(weaponName, 1);
		if (gs == null)
		{
			gs = GameObject.FindWithTag("Player").GetComponentInChildren<GunSetter>();
		}
		gs.ResetWeapons();
		gs.ForceWeapon(weaponName);
		gs.gunc.NoWeapon();
		base.gameObject.GetComponentInParent<ShopZone>().UpdateMoney();
	}

	public void EquipWeapon()
	{
		if (PlayerPrefs.GetInt(weaponName, 0) > 0)
		{
			equipImage.SetActive(false);
			PlayerPrefs.SetInt(weaponName, 0);
			if (gs == null)
			{
				gs = GameObject.FindWithTag("Player").GetComponentInChildren<GunSetter>();
			}
			gs.ResetWeapons();
		}
		else
		{
			equipImage.SetActive(true);
			PlayerPrefs.SetInt(weaponName, 1);
			if (gs == null)
			{
				gs = GameObject.FindWithTag("Player").GetComponentInChildren<GunSetter>();
			}
			gs.ResetWeapons();
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
