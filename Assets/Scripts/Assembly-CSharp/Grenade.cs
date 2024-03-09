using UnityEngine;

public class Grenade : MonoBehaviour
{
	public string hitterWeapon;

	public GameObject explosion;

	private bool exploded;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!exploded)
		{
			exploded = true;
			GameObject gameObject = Object.Instantiate(this.explosion, base.transform.position, Quaternion.identity);
			Explosion[] componentsInChildren = gameObject.GetComponentsInChildren<Explosion>();
			Explosion[] array = componentsInChildren;
			foreach (Explosion explosion in array)
			{
				explosion.GetComponent<Explosion>().hitterWeapon = hitterWeapon;
			}
			if (collision.gameObject.layer != 8)
			{
				GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().HitStop(0.05f);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
