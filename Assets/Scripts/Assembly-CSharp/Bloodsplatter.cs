using System.Collections.Generic;
using UnityEngine;

public class Bloodsplatter : MonoBehaviour
{
	public ParticleSystem part;

	public List<ParticleCollisionEvent> collisionEvents;

	public GameObject bloodStain;

	private GameObject bldstn;

	private int i = 0;

	private AudioSource[] bloodStainAud;

	private AudioSource aud;

	public Material[] materials;

	private MeshRenderer mr;

	private NewMovement nmov;

	public int hpAmount;

	private SphereCollider col;

	public bool hpOnParticleCollision;

	private GoreZone gz;

	private OptionsManager opm;

	public bool halfChance;

	public bool ready = false;

	private void Start()
	{
		part = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		aud = GetComponent<AudioSource>();
		if (aud != null)
		{
			aud.pitch = Random.Range(0.75f, 1.5f);
			aud.Play();
		}
		col = GetComponent<SphereCollider>();
		Invoke("DestroyCollider", 0.25f);
		if (hpOnParticleCollision)
		{
			Invoke("Destroy", 3f);
		}
		else
		{
			Invoke("Destroy", 3f);
		}
		int @int = PlayerPrefs.GetInt("BlOn", 1);
		if (@int == 1)
		{
			part.Play();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Moving" || ((other.gameObject.tag == "Glass" || other.gameObject.tag == "GlassFloor") && other.transform.childCount > 0))
		{
			int num = part.GetCollisionEvents(other, collisionEvents);
			if (opm == null)
			{
				opm = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
			}
			if ((!halfChance && (float)Random.Range(0, 100) < opm.bloodstainChance) || (halfChance && (float)Random.Range(0, 100) < opm.bloodstainChance / 2f))
			{
				bldstn = Object.Instantiate(bloodStain, collisionEvents[0].intersection, base.transform.rotation, base.transform);
				bldstn.transform.forward = collisionEvents[0].normal * -1f;
				mr = bldstn.GetComponent<MeshRenderer>();
				mr.material = materials[Random.Range(0, materials.Length - 1)];
				bldstn.transform.Rotate(Vector3.forward * Random.Range(0, 359), Space.Self);
				Vector3 localScale = bldstn.transform.localScale;
				if (gz == null)
				{
					gz = other.GetComponentInParent<GoreZone>();
				}
				if (other.gameObject.tag == "Moving")
				{
					bldstn.transform.SetParent(other.transform, true);
				}
				else if (other.gameObject.tag == "Glass" || other.gameObject.tag == "GlassFloor")
				{
					bldstn.transform.SetParent(other.transform, true);
				}
				else
				{
					bldstn.transform.SetParent(gz.goreZone, true);
				}
			}
		}
		else if (ready && hpOnParticleCollision && other.gameObject.tag == "Player")
		{
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(5, true);
			MonoBehaviour.print("Get Health!");
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (ready && other.gameObject.tag == "Player")
		{
			Object.Destroy(col);
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(hpAmount, false);
		}
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void DestroyCollider()
	{
		if (col != null)
		{
			Object.Destroy(col);
		}
	}

	public void GetReady()
	{
		ready = true;
	}
}
