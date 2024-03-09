using UnityEngine;

public class Breakable : MonoBehaviour
{
	public bool weak;

	public bool precisionOnly;

	public bool playerOnly;

	public GameObject breakParticle;

	public GameObject[] breakChunks;

	public void Break()
	{
		Object.Instantiate(breakParticle, base.transform.position, base.transform.rotation);
		ItemPickUp[] componentsInChildren = GetComponentsInChildren<ItemPickUp>();
		if (componentsInChildren.Length > 0)
		{
			ItemPickUp[] array = componentsInChildren;
			foreach (ItemPickUp itemPickUp in array)
			{
				itemPickUp.transform.SetParent(base.transform.parent, true);
				Rigidbody component = itemPickUp.GetComponent<Rigidbody>();
				component.isKinematic = false;
				component.useGravity = true;
			}
		}
		if (breakChunks.Length > 0)
		{
			GameObject[] array2 = breakChunks;
			foreach (GameObject original in array2)
			{
				GameObject gameObject = Object.Instantiate(original, base.transform.position, Random.rotation);
				Vector3 force = new Vector3(Random.Range(-45, 45), Random.Range(-45, 45), Random.Range(-45, 45));
				gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
				gameObject.transform.SetParent(base.transform.GetComponentInParent<GoreZone>().goreZone);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
