using UnityEngine;

public class Bonus : MonoBehaviour
{
	private Vector3 cRotation;

	public GameObject breakEffect;

	private bool activated;

	private void Start()
	{
		cRotation = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
	}

	private void Update()
	{
		base.transform.Rotate(cRotation * Time.deltaTime * 5f);
	}

	public void Break()
	{
		Invoke("BeginBreak", 0.02f);
	}

	private void BeginBreak()
	{
		Object.Instantiate(breakEffect, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Player" && !activated)
		{
			activated = true;
			other.gameObject.GetComponentInChildren<Punch>().ParryFlash();
			Invoke("BeginBreak", 0.02f);
			StyleHUD componentInChildren = GameObject.FindWithTag("MainCamera").GetComponentInChildren<StyleHUD>();
			StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			componentInChildren.AddPoints(0, "<color=cyan>SECRET</color>");
			component.secrets++;
		}
	}
}
