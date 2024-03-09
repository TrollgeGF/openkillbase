using UnityEngine;

public class ItemIdentifier : MonoBehaviour
{
	public GeneralItemType generalType;

	public ItemType type;

	private Skull skull;

	public GameObject putDownItem;

	private void Start()
	{
		if (generalType == GeneralItemType.Skull)
		{
			skull = GetComponent<Skull>();
		}
	}

	public void PunchWith()
	{
		if (generalType == GeneralItemType.Skull)
		{
			skull.Flash();
		}
	}

	public void HitWith(GameObject target)
	{
		if (generalType == GeneralItemType.Skull && type == ItemType.SkullBlue)
		{
			Flammable component = target.GetComponent<Flammable>();
			component.Burn(4f);
		}
	}
}
