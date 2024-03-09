using UnityEngine;

public class WeaponCharges : MonoBehaviour
{
	public float rev0charge = 100f;

	public float rev1charge = 400f;

	private void Update()
	{
		if (rev0charge < 100f)
		{
			rev0charge = Mathf.MoveTowards(rev0charge, 100f, 40f * Time.deltaTime);
		}
		if (rev1charge < 400f)
		{
			rev1charge = Mathf.MoveTowards(rev1charge, 400f, 25f * Time.deltaTime);
		}
	}
}
