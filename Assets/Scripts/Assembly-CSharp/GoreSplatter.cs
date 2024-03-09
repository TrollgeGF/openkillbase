using UnityEngine;

public class GoreSplatter : MonoBehaviour
{
	private Rigidbody rb;

	private Vector3 direction;

	private float force;

	private bool goreOver;

	private Vector3 defaultScale;

	private bool freezeGore;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		defaultScale = base.transform.localScale;
		direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		force = Random.Range(10, 30);
		rb.AddForce(direction * force, ForceMode.VelocityChange);
		if (PlayerPrefs.GetInt("FreGor", 0) == 1)
		{
			freezeGore = true;
		}
		if (freezeGore)
		{
			Invoke("ReadyToStopGore", 5f);
		}
	}

	private void FixedUpdate()
	{
		if (freezeGore && goreOver && rb.velocity.y > -0.1f && rb.velocity.y < 0.1f)
		{
			StopGore();
		}
		if (base.transform.position.y < -500f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == 8 && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor"))
		{
			if (other.gameObject.GetComponentInParent<GoreZone>() != null)
			{
				base.transform.SetParent(other.gameObject.GetComponentInParent<GoreZone>().goreZone);
			}
			if (freezeGore)
			{
				goreOver = true;
			}
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (freezeGore && other.gameObject.layer == 8 && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor"))
		{
			goreOver = false;
		}
	}

	private void ReadyToStopGore()
	{
		if (!goreOver)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void StopGore()
	{
		Object.Destroy(rb);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		Object.Destroy(this);
	}
}
