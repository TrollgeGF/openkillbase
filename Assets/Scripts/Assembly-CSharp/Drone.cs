using UnityEngine;
using UnityEngine.AI;

public class Drone : MonoBehaviour
{
	public bool friendly;

	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	public bool crashing;

	private Vector3 crashTarget;

	private Rigidbody rb;

	private bool canInterruptCrash;

	public bool playerSpotted;

	public bool toLastKnownPos;

	public Vector3 posOnNavMesh;

	public LayerMask lmask;

	private NavMeshPath nmp;

	private int currentCorner;

	private int maxCorners;

	public Vector3[] corners;

	private Vector3 nextRandomPos;

	private GameObject camObj;

	private Transform target;

	public GameObject smallBlood;

	public GameObject bigBlood;

	public GameObject explosion;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private AudioSource aud;

	public AudioClip hurtSound;

	public AudioClip deathSound;

	public AudioClip windUpSound;

	public AudioClip spotSound;

	public AudioClip loseSound;

	private float dodgeCooldown;

	private float attackCooldown;

	public GameObject projectile;

	private ParticleSystem part;

	private bool killedByPlayer;

	private void Start()
	{
		camObj = GameObject.FindWithTag("MainCamera");
		rb = GetComponent<Rigidbody>();
		nmp = new NavMeshPath();
		part = GetComponentInChildren<ParticleSystem>();
		if (!friendly)
		{
			target = camObj.transform;
		}
		dodgeCooldown = Random.Range(0.5f, 3f);
		attackCooldown = Random.Range(1f, 3f);
		if (spawnIn)
		{
			Object.Instantiate(spawnEffect, base.transform.position, Quaternion.identity);
		}
	}

	private void Update()
	{
		if (!crashing && playerSpotted)
		{
			base.transform.LookAt(target);
			if (dodgeCooldown > 0f)
			{
				dodgeCooldown = Mathf.MoveTowards(dodgeCooldown, 0f, Time.deltaTime);
			}
			else
			{
				dodgeCooldown = Random.Range(1f, 3f);
				RandomDodge();
			}
			if (attackCooldown > 0f)
			{
				attackCooldown = Mathf.MoveTowards(attackCooldown, 0f, Time.deltaTime);
				return;
			}
			attackCooldown = Random.Range(2f, 4f);
			PlaySound(windUpSound);
			part.Play();
			Invoke("Shoot", 0.75f);
		}
	}

	private void FixedUpdate()
	{
		if (crashing)
		{
			rb.AddForce(base.transform.forward * 50f, ForceMode.Acceleration);
			base.transform.Rotate(0f, 0f, 10f, Space.Self);
		}
		else if (playerSpotted)
		{
			rb.velocity *= 0.95f;
			if (Vector3.Distance(base.transform.position, target.transform.position) > 15f)
			{
				rb.AddForce(base.transform.forward * 50f, ForceMode.Acceleration);
			}
			else if (Vector3.Distance(base.transform.position, target.transform.position) < 5f)
			{
				rb.AddForce(base.transform.forward * -50f, ForceMode.Impulse);
			}
			if (Physics.Raycast(base.transform.position, target.transform.position - base.transform.position, Vector3.Distance(base.transform.position, target.transform.position) - 1f, lmask))
			{
				playerSpotted = false;
				PlaySound(loseSound);
				NavMeshHit hit;
				NavMesh.SamplePosition(base.transform.position, out hit, float.PositiveInfinity, -1);
				posOnNavMesh = hit.position;
				NavMesh.SamplePosition(target.position, out hit, float.PositiveInfinity, -1);
				Vector3 position = hit.position;
				NavMesh.CalculatePath(posOnNavMesh, position, -1, nmp);
				currentCorner = 0;
				maxCorners = nmp.corners.Length;
				corners = nmp.corners;
				toLastKnownPos = true;
			}
		}
		else if (toLastKnownPos)
		{
			float num = Vector3.Distance(base.transform.position, nmp.corners[currentCorner] + Vector3.up * 5f);
			for (int i = currentCorner; i < maxCorners; i++)
			{
				if (Vector3.Distance(base.transform.position, nmp.corners[i] + Vector3.up * 5f) < num)
				{
					currentCorner = i;
					num = Vector3.Distance(base.transform.position, nmp.corners[i] + Vector3.up * 5f);
				}
			}
			base.transform.LookAt(nmp.corners[currentCorner] + Vector3.up * 5f);
			rb.AddForce(base.transform.forward * 10f, ForceMode.Acceleration);
			if (Vector3.Distance(base.transform.position, nmp.corners[currentCorner] + Vector3.up * 5f) < 1f)
			{
				currentCorner++;
				if (currentCorner >= maxCorners)
				{
					toLastKnownPos = false;
					RaycastHit hitInfo;
					Physics.Raycast(base.transform.position, Random.onUnitSphere, out hitInfo, float.PositiveInfinity, lmask);
					nextRandomPos = hitInfo.point;
				}
			}
		}
		else
		{
			base.transform.LookAt(nextRandomPos);
			rb.AddForce(base.transform.forward * 10f, ForceMode.Acceleration);
			if (Vector3.Distance(base.transform.position, nextRandomPos) < 5f)
			{
				RaycastHit hitInfo2;
				Physics.Raycast(base.transform.position, Random.onUnitSphere, out hitInfo2, float.PositiveInfinity, lmask);
				nextRandomPos = hitInfo2.point;
			}
		}
		if (!crashing && !playerSpotted && !Physics.Raycast(base.transform.position, target.transform.position - base.transform.position, Vector3.Distance(base.transform.position, target.transform.position) - 1f, lmask))
		{
			PlaySound(spotSound);
			playerSpotted = true;
		}
	}

	public void RandomDodge()
	{
		rb.AddForce(base.transform.up * Random.Range(-5f, 5f) + (base.transform.right * Random.Range(-5f, 5f)).normalized * 50f, ForceMode.Impulse);
	}

	public void GetHurt(Vector3 force, float multiplier)
	{
		bool dead = false;
		if (!crashing)
		{
			health -= 1f * multiplier;
			if (scalc == null)
			{
				scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
			}
			if (health <= 0f)
			{
				dead = true;
			}
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			scalc.HitCalculator(eid.hitter, "drone", "", dead, base.gameObject);
			if (health <= 0f && !crashing)
			{
				if (eid.hitter != "enemy")
				{
					killedByPlayer = true;
				}
				crashing = true;
				rb.velocity = Vector3.zero;
				crashTarget = target.transform.position;
				base.transform.LookAt(crashTarget);
				if (aud == null)
				{
					aud = GetComponent<AudioSource>();
				}
				aud.clip = deathSound;
				aud.volume = 0.75f;
				aud.pitch = Random.Range(0.85f, 1.35f);
				aud.priority = 11;
				aud.Play();
				GameObject gameObject = Object.Instantiate(bigBlood, base.transform.position, Quaternion.identity);
				gameObject.GetComponent<Bloodsplatter>().GetReady();
				gameObject.GetComponent<ParticleSystem>().Play();
				ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
				if (componentInParent != null)
				{
					componentInParent.deadEnemies++;
				}
				Invoke("CanInterruptCrash", 0.5f);
			}
			else
			{
				PlaySound(hurtSound);
				GameObject gameObject2 = Object.Instantiate(smallBlood, base.transform.position, Quaternion.identity);
				if (health > 0f)
				{
					gameObject2.GetComponent<Bloodsplatter>().GetReady();
				}
				if (multiplier >= 1f)
				{
					gameObject2.GetComponent<Bloodsplatter>().hpAmount = 30;
				}
				gameObject2.GetComponent<ParticleSystem>().Play();
			}
		}
		else if (multiplier >= 1f || canInterruptCrash)
		{
			Explode();
		}
	}

	public void PlaySound(AudioClip clippe)
	{
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		aud.clip = clippe;
		aud.volume = 0.5f;
		aud.pitch = Random.Range(0.85f, 1.35f);
		aud.priority = 12;
		aud.Play();
	}

	public void Explode()
	{
		GameObject gameObject = Object.Instantiate(this.explosion, base.transform.position, Quaternion.identity);
		if (killedByPlayer)
		{
			Explosion[] componentsInChildren = gameObject.GetComponentsInChildren<Explosion>();
			Explosion[] array = componentsInChildren;
			foreach (Explosion explosion in array)
			{
				explosion.friendlyFire = true;
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void Shoot()
	{
		if (!crashing)
		{
			GameObject gameObject = Object.Instantiate(projectile, base.transform.position + base.transform.forward, base.transform.rotation);
			gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, Random.Range(0, 360));
			gameObject.transform.localScale *= 0.5f;
			Projectile component = gameObject.GetComponent<Projectile>();
			component.safeEnemyType = EnemyType.Drone;
			component.speed = 35f;
			GameObject gameObject2 = Object.Instantiate(projectile, gameObject.transform.position + gameObject.transform.up, gameObject.transform.rotation);
			gameObject2.transform.localScale *= 0.5f;
			component = gameObject2.GetComponent<Projectile>();
			component.safeEnemyType = EnemyType.Drone;
			component.speed = 35f;
			gameObject2 = Object.Instantiate(projectile, gameObject.transform.position - gameObject.transform.up, gameObject.transform.rotation);
			gameObject2.transform.localScale *= 0.5f;
			component = gameObject2.GetComponent<Projectile>();
			component.safeEnemyType = EnemyType.Drone;
			component.speed = 35f;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (crashing && (collision.gameObject.layer == 8 || collision.gameObject.layer == 24 || collision.gameObject.layer == 10 || collision.gameObject.layer == 11 || collision.gameObject.tag == "Player"))
		{
			Explode();
		}
	}

	private void CanInterruptCrash()
	{
		canInterruptCrash = true;
	}
}
