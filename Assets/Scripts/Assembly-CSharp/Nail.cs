using UnityEngine;

public class Nail : MonoBehaviour
{
	private bool hit;

	public float damage;

	private AudioSource aud;

	public AudioClip environmentHitSound;

	public AudioClip enemyHitSound;

	public string weaponType;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!hit && (other.gameObject.layer == 10 || other.gameObject.layer == 11) && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb" || other.gameObject.tag == "Enemy"))
		{
			hit = true;
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.hitter = "nail";
			if (!eid.hitterWeapons.Contains(weaponType))
			{
				eid.hitterWeapons.Add(weaponType);
			}
			eid.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 100f, base.transform.position, damage, false, 0f);
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = enemyHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.2f;
			aud.Play();
			GetComponent<Collider>().isTrigger = true;
			GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
			GetComponent<Rigidbody>().isKinematic = true;
			base.transform.position += base.transform.forward * -0.5f;
			base.transform.SetParent(other.transform, true);
			GetComponent<TrailRenderer>().enabled = false;
			Invoke("RemoveTime", 5f);
		}
		if (!hit && other.gameObject.layer == 8)
		{
			hit = true;
			Invoke("RemoveTime", 1f);
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = environmentHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.2f;
			aud.Play();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!hit && (other.gameObject.layer == 10 || other.gameObject.layer == 11) && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb" || other.gameObject.tag == "Enemy"))
		{
			hit = true;
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.hitter = "nail";
			eid.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 100f, base.transform.position, damage, false, 0f);
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = enemyHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.2f;
			aud.Play();
			GetComponent<Collider>().isTrigger = true;
			GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
			GetComponent<Rigidbody>().isKinematic = true;
			base.transform.position += base.transform.forward * -0.5f;
			base.transform.SetParent(other.transform, true);
			GetComponent<TrailRenderer>().enabled = false;
			Invoke("RemoveTime", 5f);
		}
	}

	private void RemoveTime()
	{
		Object.Destroy(base.gameObject);
	}
}
