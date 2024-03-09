using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public bool enemy;

	private bool harmless;

	public bool lowQuality;

	private CameraController cc;

	private Light light;

	private MeshRenderer mr;

	private Color materialColor;

	private bool fading;

	public float speed;

	public float maxSize;

	public LayerMask lmask;

	public int damage;

	public GameObject explosionChunk;

	public bool ignite;

	public bool safeForPlayer;

	public bool friendlyFire;

	private List<Collider> hitColliders = new List<Collider>();

	public string hitterWeapon;

	private void Start()
	{
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		cc.CameraShake(1.5f);
		if (speed == 0f)
		{
			speed = 1f;
		}
		if (!lowQuality && PlayerPrefs.GetInt("SimExp", 0) == 1)
		{
			lowQuality = true;
		}
		if (lowQuality)
		{
			return;
		}
		light = GetComponentInChildren<Light>();
		light.enabled = true;
		if (explosionChunk != null)
		{
			for (int i = 0; i < Random.Range(24, 30); i++)
			{
				GameObject gameObject = Object.Instantiate(explosionChunk, base.transform.position, Random.rotation);
				Vector3 vector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				gameObject.GetComponent<Rigidbody>().AddForce(vector * 1000f);
			}
		}
	}

	private void FixedUpdate()
	{
		base.transform.localScale += Vector3.one * 0.05f * speed;
		if (light != null)
		{
			light.range += 0.05f * speed;
		}
		if (!fading && base.transform.lossyScale.x * GetComponent<SphereCollider>().radius > maxSize)
		{
			Fade();
		}
		if (fading)
		{
			materialColor.a -= 0.02f;
			if (light != null)
			{
				light.intensity -= 0.65f;
			}
			mr.material.SetColor("_Color", materialColor);
			if (materialColor.a <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		RaycastHit hitInfo;
		if (!harmless && !hitColliders.Contains(other) && (!Physics.Raycast(base.transform.position + (other.transform.position - base.transform.position).normalized * 0.1f, other.transform.position - base.transform.position, out hitInfo, Vector3.Distance(base.transform.position + (other.transform.position - base.transform.position).normalized * 0.1f, other.transform.position), lmask, QueryTriggerInteraction.Ignore) || hitInfo.transform.gameObject == other.gameObject || other.gameObject.layer == 8 || other.gameObject.layer == 24 || other.gameObject.layer == 11))
		{
			if (other.gameObject.layer == 11)
			{
				Collider[] componentsInChildren = other.transform.GetComponentsInChildren<Collider>();
				bool flag = false;
				Collider[] array = componentsInChildren;
				foreach (Collider collider in array)
				{
					if (collider.gameObject == other.gameObject)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			if (other.gameObject.tag == "Player")
			{
				hitColliders.Add(other);
				if (!safeForPlayer)
				{
					if (damage > 0)
					{
						if (enemy)
						{
							other.gameObject.GetComponent<NewMovement>().GetHurt(damage, true, 1f, true);
						}
						else
						{
							other.gameObject.GetComponent<NewMovement>().GetHurt(damage, true, 0f, true);
						}
					}
					Debug.Log(other.gameObject);
					if (Mathf.Abs(base.transform.position.x - other.transform.position.x) < 0.25f && Mathf.Abs(base.transform.position.z - other.transform.position.z) < 0.25f)
					{
						other.gameObject.GetComponent<NewMovement>().Launch(other.transform.position, 200f, maxSize);
					}
					else
					{
						other.gameObject.GetComponent<NewMovement>().Launch(base.transform.position, 200f, maxSize);
					}
				}
			}
			else if (other.gameObject.layer == 10 || other.gameObject.layer == 11)
			{
				EnemyIdentifierIdentifier component = other.GetComponent<EnemyIdentifierIdentifier>();
				if (component != null)
				{
					Collider component2 = component.eid.GetComponent<Collider>();
					if (component2 != null && !hitColliders.Contains(component2) && !component.eid.dead)
					{
						hitColliders.Add(component2);
						if (component.eid.type != EnemyType.Spider)
						{
							if (friendlyFire)
							{
								component.eid.hitter = "ffexplosion";
							}
							else if (enemy)
							{
								component.eid.hitter = "enemy";
							}
							else
							{
								component.eid.hitter = "explosion";
							}
							if (!component.eid.hitterWeapons.Contains(hitterWeapon))
							{
								component.eid.hitterWeapons.Add(hitterWeapon);
							}
							Vector3 vector = (other.transform.position - base.transform.position).normalized;
							if (vector.y <= 0.5f)
							{
								vector = new Vector3(vector.x, vector.y + 0.5f, vector.z);
							}
							else if (vector.y < 1f)
							{
								vector = new Vector3(vector.x, 1f, vector.z);
							}
							component.eid.DeliverDamage(other.gameObject, vector * 50000f, other.transform.position, (float)damage / 10f, false, 0f);
							if (ignite)
							{
								Flammable componentInChildren = component.eid.GetComponentInChildren<Flammable>();
								if (componentInChildren != null)
								{
									componentInChildren.Burn(damage / 10);
								}
							}
						}
					}
					else if (component.eid.dead)
					{
						hitColliders.Add(other);
						component.eid.hitter = "explosion";
						component.eid.DeliverDamage(other.gameObject, (other.transform.position - base.transform.position).normalized * 5000f, other.transform.position, (float)damage / 10f, false, 0f);
						if (ignite)
						{
							Flammable componentInChildren2 = component.eid.GetComponentInChildren<Flammable>();
							if (componentInChildren2 != null)
							{
								componentInChildren2.Burn(damage / 10);
							}
						}
					}
				}
			}
			else if (other.GetComponent<Breakable>() != null)
			{
				other.GetComponent<Breakable>().Break();
			}
			else if (other.GetComponent<Glass>() != null)
			{
				other.GetComponent<Glass>().Shatter();
			}
			else if (ignite && other.GetComponent<Flammable>() != null)
			{
				other.GetComponent<Flammable>().Burn(4f);
			}
		}
		if (harmless)
		{
			return;
		}
		if (other.gameObject.tag != "Player" && other.GetComponent<Rigidbody>() != null)
		{
			if (!hitColliders.Contains(other))
			{
				hitColliders.Add(other);
			}
			Vector3 normalized = (other.transform.position - base.transform.position).normalized;
			normalized = new Vector3(normalized.x * (5f - Vector3.Distance(other.transform.position, base.transform.position)) * 7500f, 18750f, normalized.z * (5f - Vector3.Distance(other.transform.position, base.transform.position)) * 7500f);
			other.GetComponent<Rigidbody>().AddForce(normalized);
		}
		if (other.gameObject.layer == 14)
		{
			ThrownSword component3 = other.GetComponent<ThrownSword>();
			Projectile component4 = other.GetComponent<Projectile>();
			if (component3 != null)
			{
				component3.deflected = true;
			}
			if (component4 != null)
			{
				other.transform.LookAt(2f * other.transform.position - base.transform.position);
				component4.friendly = true;
			}
		}
	}

	private void Fade()
	{
		harmless = true;
		mr = GetComponent<MeshRenderer>();
		materialColor = mr.material.GetColor("_Color");
		fading = true;
		speed /= 4f;
	}

	private void BecomeHarmless()
	{
		harmless = true;
	}
}
