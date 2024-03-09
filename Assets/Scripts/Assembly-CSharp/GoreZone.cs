using UnityEngine;

public class GoreZone : MonoBehaviour
{
	public Transform goreZone;

	public CheckPoint checkpoint;

	public float maxGore;

	public int goreAmount;

	private void Start()
	{
		maxGore = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>().maxGore;
		if (goreZone == null)
		{
			GameObject gameObject = new GameObject();
			goreZone = gameObject.transform;
			goreZone.SetParent(base.transform, true);
		}
	}

	private void FixedUpdate()
	{
		goreAmount = goreZone.childCount;
		if ((float)goreZone.childCount > maxGore)
		{
			Object.Destroy(goreZone.GetChild(0).gameObject);
		}
	}

	public void Combine()
	{
		StaticBatchingUtility.Combine(goreZone.gameObject);
	}

	public void AddDeath()
	{
		checkpoint.restartDeaths++;
	}
}
