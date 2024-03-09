using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
	private int crossHairType;

	private Image mainch;

	public Image[] altchs;

	public Image[] chuds;

	public Sprite[] circles;

	public Material invertMaterial;

	private void Start()
	{
		mainch = GetComponent<Image>();
		GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().crosshair = base.gameObject;
		CheckCrossHair();
	}

	public void CheckCrossHair()
	{
		if (mainch == null)
		{
			mainch = GetComponent<Image>();
		}
		crossHairType = PlayerPrefs.GetInt("CroHai", 1);
		switch (crossHairType)
		{
		case 0:
		{
			mainch.enabled = false;
			Image[] array2 = altchs;
			foreach (Image image2 in array2)
			{
				image2.enabled = false;
			}
			break;
		}
		case 1:
		{
			mainch.enabled = true;
			Image[] array3 = altchs;
			foreach (Image image3 in array3)
			{
				image3.enabled = false;
			}
			break;
		}
		case 2:
		{
			mainch.enabled = true;
			Image[] array = altchs;
			foreach (Image image in array)
			{
				image.enabled = true;
			}
			break;
		}
		}
		Color color = Color.white;
		int @int = PlayerPrefs.GetInt("CroCol", 1);
		switch (@int)
		{
		case 0:
		case 1:
			color = Color.white;
			break;
		case 2:
			color = Color.gray;
			break;
		case 3:
			color = Color.black;
			break;
		case 4:
			color = Color.red;
			break;
		case 5:
			color = Color.green;
			break;
		case 6:
			color = Color.blue;
			break;
		case 7:
			color = Color.cyan;
			break;
		case 8:
			color = Color.yellow;
			break;
		case 9:
			color = Color.magenta;
			break;
		}
		if (@int == 0)
		{
			mainch.material = invertMaterial;
		}
		else
		{
			mainch.material = null;
		}
		mainch.color = color;
		Image[] array4 = altchs;
		foreach (Image image4 in array4)
		{
			image4.color = color;
			if (@int == 0)
			{
				image4.material = invertMaterial;
			}
			else
			{
				image4.material = null;
			}
		}
		int int2 = PlayerPrefs.GetInt("Chud", 2);
		if (int2 == 0)
		{
			Image[] array5 = chuds;
			foreach (Image image5 in array5)
			{
				image5.enabled = false;
			}
			return;
		}
		Image[] array6 = chuds;
		foreach (Image image6 in array6)
		{
			image6.enabled = true;
			image6.sprite = circles[int2 - 1];
		}
	}
}
