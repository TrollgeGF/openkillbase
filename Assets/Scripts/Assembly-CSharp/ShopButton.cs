using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public bool failure;

	public GameObject clickSound;

	public GameObject failSound;

	public GameObject[] toActivate;

	public GameObject[] toDeactivate;

	public VariationInfo variationInfo;

	public void OnPointerClick(PointerEventData eventData)
	{
		GameObject[] array = toActivate;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
		}
		GameObject[] array2 = toDeactivate;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(false);
		}
		if (!failure)
		{
			if (variationInfo != null)
			{
				variationInfo.WeaponBought();
			}
			if (clickSound != null)
			{
				Object.Instantiate(clickSound);
			}
		}
		else if (failure && failSound != null)
		{
			Object.Instantiate(failSound);
		}
	}
}
