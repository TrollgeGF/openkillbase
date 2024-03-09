using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public bool onGround = false;

	public bool test;

	public bool heavyFall;

	public GameObject shockwave;

	public bool enemy;

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24) && (!enemy || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor"))
		{
			onGround = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Slippery" && (other.gameObject.layer == 8 || other.gameObject.layer == 24) && (!enemy || other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "GlassFloor") && onGround)
		{
			onGround = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!heavyFall)
		{
			return;
		}
		if (other.gameObject.layer == 10)
		{
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.hitter = "ground slam";
			eid.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position) * 5000f, other.transform.position, 2f, true, 0f);
			if (!eid.exploded)
			{
				heavyFall = false;
				onGround = true;
			}
		}
		else if (other.gameObject.layer == 8 || other.gameObject.layer == 24)
		{
			Breakable component = other.gameObject.GetComponent<Breakable>();
			if (shockwave != null)
			{
				Object.Instantiate(shockwave, base.transform.position, Quaternion.identity);
			}
			if (component != null && component.weak)
			{
				component.Break();
			}
		}
	}
}
