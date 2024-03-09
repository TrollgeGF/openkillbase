using UnityEngine;
using UnityEngine.UI;

public class SecretMissionPanel : MonoBehaviour
{
	public int missionNumber;

	private void Start()
	{
		GotEnabled();
	}

	private void OnEnable()
	{
		GotEnabled();
	}

	private void GotEnabled()
	{
		if (GameProgressSaver.GetSecretMission(missionNumber))
		{
			Image component = GetComponent<Image>();
			component.fillCenter = true;
			Text componentInChildren = GetComponentInChildren<Text>();
			componentInChildren.color = Color.black;
			GetComponent<Button>().interactable = true;
			GetComponentInParent<LayerSelect>().SecretMissionDone();
		}
		else
		{
			Image component2 = GetComponent<Image>();
			component2.fillCenter = false;
			Text componentInChildren2 = GetComponentInChildren<Text>();
			componentInChildren2.color = new Color(0.5f, 0.5f, 0.5f);
			GetComponent<Button>().interactable = false;
		}
	}
}
