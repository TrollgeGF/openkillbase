using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
	public int levelNumber;

	private Text panelTitle;

	private bool allSecrets;

	public Sprite lockedSprite;

	public Sprite unlockedSprite;

	private Sprite origSprite;

	public Image[] secretIcons;

	public Image challengeIcon;

	private int tempInt;

	private string origName;

	private LayerSelect ls;

	private GameObject challengeChecker;

	private void OnEnable()
	{
		if (ls == null)
		{
			ls = GetComponentInParent<LayerSelect>();
		}
		if (origSprite == null)
		{
			origSprite = base.transform.Find("Image").GetComponent<Image>().sprite;
		}
		if (origName == null)
		{
			origName = base.transform.Find("Name").GetComponent<Text>().text;
		}
		tempInt = GameProgressSaver.GetProgress(PlayerPrefs.GetInt("Diff", 2));
		Debug.Log("Getting Progress for: " + PlayerPrefs.GetInt("Diff", 2) + ". Progress is: " + tempInt + ". Level Number is: " + levelNumber);
		if (tempInt < levelNumber)
		{
			int num = levelNumber;
			int num2 = 0;
			while (num > 5)
			{
				num -= 5;
				num2++;
			}
			base.transform.Find("Name").GetComponent<Text>().text = "" + num2 + "-" + num + ": ???";
			base.transform.Find("Image").GetComponent<Image>().sprite = lockedSprite;
			GetComponent<Button>().enabled = false;
		}
		else
		{
			if (tempInt == levelNumber)
			{
				base.transform.Find("Image").GetComponent<Image>().sprite = unlockedSprite;
			}
			else
			{
				base.transform.Find("Image").GetComponent<Image>().sprite = origSprite;
			}
			base.transform.Find("Name").GetComponent<Text>().text = origName;
			GetComponent<Button>().enabled = true;
			if (challengeChecker == null)
			{
				challengeChecker = challengeIcon.transform.Find("EventTrigger").gameObject;
			}
			if (tempInt > levelNumber)
			{
				challengeChecker.SetActive(true);
			}
		}
		string path = Application.persistentDataPath + "/lvl" + levelNumber + "progress.bepis";
		if (File.Exists(path))
		{
			Debug.Log("Found Level " + levelNumber + " Data");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
			fileStream.Close();
			int @int = PlayerPrefs.GetInt("Diff", 2);
			if (rankData.levelNumber == levelNumber)
			{
				Text componentInChildren = base.transform.Find("Stats").Find("Rank").GetComponentInChildren<Text>();
				if (rankData.ranks[@int] == 12)
				{
					componentInChildren.text = "<color=#FFFFFF>P</color>";
					Image component = componentInChildren.transform.parent.GetComponent<Image>();
					component.color = new Color(1f, 0.686f, 0f, 1f);
					component.fillCenter = true;
					ls.AddScore(4, true);
				}
				else if (rankData.ranks[@int] < 0)
				{
					componentInChildren.text = "";
					Image component2 = componentInChildren.transform.parent.GetComponent<Image>();
					component2.color = Color.white;
					component2.fillCenter = false;
				}
				else
				{
					switch (rankData.ranks[@int])
					{
					case 1:
						componentInChildren.text = "<color=#4CFF00>C</color>";
						ls.AddScore(1);
						break;
					case 2:
						componentInChildren.text = "<color=#FFD800>B</color>";
						ls.AddScore(2);
						break;
					case 3:
						componentInChildren.text = "<color=#FF6A00>A</color>";
						ls.AddScore(3);
						break;
					case 4:
					case 5:
					case 6:
						ls.AddScore(4);
						componentInChildren.text = "<color=#FF0000>S</color>";
						break;
					default:
						ls.AddScore(0);
						componentInChildren.text = "<color=#0094FF>D</color>";
						break;
					}
					Image component3 = componentInChildren.transform.parent.GetComponent<Image>();
					component3.color = Color.white;
					component3.fillCenter = false;
				}
				if (rankData.secretsAmount > 0)
				{
					allSecrets = true;
					for (int i = 0; i < 5; i++)
					{
						if (i < rankData.secretsAmount && rankData.secretsFound[i])
						{
							secretIcons[i].fillCenter = true;
						}
						else if (i < rankData.secretsAmount)
						{
							allSecrets = false;
							secretIcons[i].fillCenter = false;
						}
						else if (i >= rankData.secretsAmount)
						{
							secretIcons[i].enabled = false;
						}
					}
				}
				else
				{
					Image[] array = secretIcons;
					foreach (Image image in array)
					{
						image.enabled = false;
					}
				}
				if (rankData.challenge)
				{
					Debug.Log("Challenge Complete " + rankData.levelNumber);
					challengeIcon.fillCenter = true;
					Text componentInChildren2 = challengeIcon.GetComponentInChildren<Text>();
					componentInChildren2.text = "C O M P L E T E";
					if (rankData.ranks[@int] == 12 && (allSecrets || rankData.secretsAmount == 0))
					{
						componentInChildren2.color = new Color(0.6f, 0.4f, 0f, 1f);
					}
					else
					{
						componentInChildren2.color = Color.black;
					}
				}
				else
				{
					challengeIcon.fillCenter = false;
					Text componentInChildren3 = challengeIcon.GetComponentInChildren<Text>();
					componentInChildren3.text = "C H A L L E N G E";
					componentInChildren3.color = Color.white;
				}
			}
			else
			{
				Debug.Log("Error in finding " + levelNumber + " Data");
				Image component4 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
				component4.color = Color.white;
				component4.fillCenter = false;
				component4.GetComponentInChildren<Text>().text = "";
				allSecrets = false;
				Image[] array2 = secretIcons;
				foreach (Image image2 in array2)
				{
					image2.enabled = true;
					image2.fillCenter = false;
				}
			}
			if (rankData.challenge && rankData.ranks[@int] == 12 && (allSecrets || rankData.secretsAmount == 0))
			{
				ls.Gold();
				GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 0.75f);
			}
			else
			{
				GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
			}
		}
		else
		{
			Debug.Log("Didn't Find Level " + levelNumber + " Data");
			Image component5 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
			component5.color = Color.white;
			component5.fillCenter = false;
			component5.GetComponentInChildren<Text>().text = "";
			allSecrets = false;
			Image[] array3 = secretIcons;
			foreach (Image image3 in array3)
			{
				image3.enabled = true;
				image3.fillCenter = false;
			}
		}
	}
}
