using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Rigidbody rb;

	public float speed;

	public float speedRandomizer;

	private AudioSource aud;

	public GameObject explosionEffect;

	public float damage;

	public bool friendly;

	public bool playerBullet;

	public string bulletType;

	public string weaponType;

	public bool decorative;

	private Vector3 origScale;

	private bool active = true;

	public EnemyType safeEnemyType;

	public bool explosive;

	public bool bigExplosion;

	public bool hittingPlayer;

	private NewMovement nmov;

	public bool boosted;

	private Collider col;

	public bool undeflectable;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		aud.pitch = Random.Range(1.8f, 2f);
		aud.Play();
		if (decorative)
		{
			origScale = base.transform.localScale;
			base.transform.localScale = Vector3.zero;
		}
		if (speed != 0f)
		{
			speed += Random.Range(0f - speedRandomizer, speedRandomizer);
		}
		if (col == null)
		{
			col = GetComponent<Collider>();
		}
	}

	private void FixedUpdate()
	{
		if (!hittingPlayer && !undeflectable && !decorative && speed != 0f)
		{
			rb.velocity = base.transform.forward * speed;
		}
		if (decorative && base.transform.localScale.x < origScale.x)
		{
			aud.pitch = base.transform.localScale.x;
			base.transform.localScale += Vector3.one * Time.deltaTime * speed;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!friendly && other.gameObject.tag == "Player")
		{
			if (explosive)
			{
				Explode();
				return;
			}
			hittingPlayer = true;
			rb.velocity = Vector3.zero;
			base.transform.position = new Vector3(other.transform.position.x, base.transform.position.y, other.transform.position.z);
			nmov = other.gameObject.GetComponentInParent<NewMovement>();
			Invoke("RecheckPlayerHit", 0.05f);
		}
		else if (active && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb"))
		{
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			if (eid.type == safeEnemyType && !friendly)
			{
				return;
			}
			active = false;
			bool tryForExplode = false;
			bool dead = eid.dead;
			if (playerBullet)
			{
				eid.hitter = bulletType;
				if (!eid.hitterWeapons.Contains(weaponType))
				{
					eid.hitterWeapons.Add(weaponType);
				}
			}
			else if (!friendly)
			{
				eid.hitter = "enemy";
			}
			else
			{
				eid.hitter = "projectile";
				tryForExplode = true;
			}
			if (boosted)
			{
				Object.FindObjectOfType<StyleHUD>().AddPoints(200, "<color=lime>PROJECTILE BOOST</color>");
			}
			if (playerBullet)
			{
				eid.DeliverDamage(other.gameObject, rb.velocity.normalized * 750f, base.transform.position, damage / 4f, tryForExplode, 0f);
			}
			else if (friendly)
			{
				eid.DeliverDamage(other.gameObject, rb.velocity.normalized * 1000f, base.transform.position, damage / 4f, tryForExplode, 0f);
			}
			else
			{
				eid.DeliverDamage(other.gameObject, rb.velocity.normalized * 100f, base.transform.position, damage / 10f, tryForExplode, 0f);
			}
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			if (!dead)
			{
				GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().HitStop(0.005f);
			}
			if (!dead || other.gameObject.layer == 11)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				active = true;
			}
		}
		else if (!hittingPlayer && (other.gameObject.layer == 8 || other.gameObject.layer == 24))
		{
			Breakable component = other.gameObject.GetComponent<Breakable>();
			if (component != null && !component.precisionOnly && component.weak)
			{
				component.Break();
			}
			if (explosive)
			{
				Explode();
				return;
			}
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
	}

	public void Explode()
	{
		if (!active)
		{
			return;
		}
		active = false;
		GameObject gameObject = Object.Instantiate(explosionEffect, base.transform.position - rb.velocity * 0.02f, base.transform.rotation);
		Explosion[] componentsInChildren = gameObject.GetComponentsInChildren<Explosion>();
		Explosion[] array = componentsInChildren;
		foreach (Explosion explosion in array)
		{
			if (bigExplosion)
			{
				explosion.maxSize *= 1.5f;
			}
			if (explosion.damage != 0)
			{
				explosion.damage = Mathf.RoundToInt(damage);
			}
			explosion.enemy = true;
		}
		Object.Destroy(base.gameObject);
	}

	private void RecheckPlayerHit()
	{
		if (hittingPlayer)
		{
			hittingPlayer = false;
			col.enabled = false;
			undeflectable = true;
			Invoke("TimeToDie", 0.01f);
		}
	}

	private void TimeToDie()
	{
		Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
		nmov.GetHurt(Mathf.RoundToInt(damage), true);
		Object.Destroy(base.gameObject);
	}
}
