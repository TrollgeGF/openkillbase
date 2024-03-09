using UnityEngine;

public class Harpoon : MonoBehaviour
{
	private bool hit;

	private bool stopped;

	public float damage;

	private AudioSource aud;

	public AudioClip environmentHitSound;

	public AudioClip enemyHitSound;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!stopped)
		{
			base.transform.LookAt(base.transform.position + rb.velocity);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!hit && (other.gameObject.layer == 10 || other.gameObject.layer == 11) && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb"))
		{
			hit = true;
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.hitter = "harpoon";
			eid.DeliverDamage(other.gameObject, Vector3.zero, base.transform.position, damage, false, 0f);
			if (eid.dead)
			{
				FixedJoint fixedJoint = base.gameObject.AddComponent<FixedJoint>();
				fixedJoint.connectedBody = other.gameObject.GetComponent<Rigidbody>();
			}
			else
			{
				stopped = true;
				rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
				FixedJoint fixedJoint2 = other.gameObject.AddComponent<FixedJoint>();
				fixedJoint2.connectedBody = other.gameObject.GetComponent<Rigidbody>();
				GetComponent<TrailRenderer>().emitting = false;
			}
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = enemyHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.4f;
			aud.Play();
		}
		else if (!stopped && other.gameObject.layer == 8)
		{
			stopped = true;
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rb.isKinematic = true;
			if (other.gameObject.tag == "Door" || other.gameObject.tag == "Moving")
			{
				FixedJoint fixedJoint3 = base.gameObject.AddComponent<FixedJoint>();
				fixedJoint3.connectedBody = other.gameObject.GetComponent<Rigidbody>();
				rb.isKinematic = false;
				hit = true;
			}
			if (other.transform.GetComponentInParent<GoreZone>() != null)
			{
				base.transform.SetParent(other.transform.GetComponentInParent<GoreZone>().goreZone, true);
			}
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = environmentHitSound;
			aud.pitch = Random.Range(0.9f, 1.1f);
			aud.volume = 0.4f;
			aud.Play();
			GetComponent<TrailRenderer>().emitting = false;
		}
	}
}
