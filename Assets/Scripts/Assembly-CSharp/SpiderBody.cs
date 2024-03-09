using UnityEngine;
using UnityEngine.AI;

public class SpiderBody : MonoBehaviour
{
	private Rigidbody[] rbs;

	public bool limp;

	private GameObject player;

	private NewMovement nmov;

	private Revolver rev;

	private NavMeshAgent nma;

	private Quaternion followPlayerRot;

	public GameObject proj;

	private RaycastHit hit;

	private RaycastHit hit2;

	public LayerMask aimlm;

	private bool readyToShoot = true;

	private float burstCharge;

	private int currentBurst;

	public float health;

	private bool dead;

	private Rigidbody rb;

	private bool falling;

	private Enemy enemy;

	private Transform firstChild;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private Transform[] bodyChild;

	public GameObject impactParticle;

	public GameObject impactSprite;

	private Quaternion spriteRot;

	private Vector3 spritePos;

	private Transform mouth;

	private GameObject currentProj;

	private bool charging;

	public GameObject chargeEffect;

	private GameObject currentCE;

	private float beamCharge;

	private AudioSource ceAud;

	private Light ceLight;

	private Vector3 predictedPlayerPos;

	public GameObject spiderBeam;

	private GameObject currentBeam;

	public GameObject beamExplosion;

	private GameObject currentExplosion;

	private float beamProbability;

	private Quaternion predictedRot;

	private bool rotating;

	public GameObject dripBlood;

	private GameObject currentDrip;

	public GameObject smallBlood;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	public GameObject spark;

	private int difficulty;

	private float coolDownMultiplier = 1f;

	private int beamsAmount = 1;

	private float maxHealth;

	public GameObject enrageEffect;

	private GameObject currentEnrageEffect;

	public Material enrageMaterial;

	public Material enrageMaterial2;

	private Material origMaterial;

	private bool parryable;

	private void Start()
	{
		burstCharge = 5f;
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		rev = player.GetComponentInChildren<Revolver>();
		nma = GetComponent<NavMeshAgent>();
		nmov = player.GetComponent<NewMovement>();
		mouth = base.transform.GetChild(0);
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		maxHealth = health;
		if (difficulty != 2 && difficulty >= 3)
		{
			coolDownMultiplier = 1.25f;
		}
		origMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
	}

	private void FixedUpdate()
	{
		if (!dead && !charging && beamCharge == 0f)
		{
			if (!nma.enabled)
			{
				base.transform.rotation = Quaternion.identity;
				nma.enabled = true;
				nma.isStopped = false;
				nma.speed = 3.5f;
			}
			if (nma != null)
			{
				nma.SetDestination(player.transform.position);
			}
			if (currentBurst > 5 && burstCharge == 0f)
			{
				currentBurst = 0;
				burstCharge = 5f;
			}
			if (burstCharge > 0f)
			{
				burstCharge -= 0.04f * coolDownMultiplier;
			}
			if (burstCharge < 0f)
			{
				burstCharge = 0f;
			}
			if (!readyToShoot || burstCharge != 0f || !Physics.Raycast(base.transform.position, base.transform.forward, out hit, 150f, aimlm) || !(hit.transform != null) || !(hit.transform.gameObject.tag == "Player"))
			{
				return;
			}
			if (currentBurst != 0)
			{
				ShootProj();
				return;
			}
			if ((Random.Range(0f, health * 0.4f) >= beamProbability && beamProbability <= 5f) || Vector3.Distance(base.transform.position, hit.point) > 20f)
			{
				ShootProj();
				beamProbability += 1f;
				return;
			}
			ChargeBeam();
			if (difficulty > 2 && health < maxHealth / 2f)
			{
				beamsAmount = 2;
			}
			if (health > 10f)
			{
				beamProbability = 0f;
			}
			else
			{
				beamProbability = 1f;
			}
		}
		else if (!dead && charging)
		{
			if (beamCharge + 0.005f * coolDownMultiplier < 1f)
			{
				nma.speed = 0f;
				nma.SetDestination(base.transform.position);
				nma.isStopped = true;
				beamCharge += 0.005f * coolDownMultiplier;
				currentCE.transform.localScale = Vector3.one * beamCharge * 5f;
				ceAud.pitch = beamCharge * 2f;
				ceLight.intensity = beamCharge * 30f;
			}
			else
			{
				beamCharge = 1f;
				charging = false;
				BeamChargeEnd();
			}
		}
	}

	private void Update()
	{
		if (!dead)
		{
			if (beamCharge < 1f)
			{
				base.transform.LookAt(player.transform);
			}
			else if (rotating && beamCharge == 1f)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, predictedRot, Time.deltaTime * 200f / 2.5f);
			}
			else if (!rotating && beamCharge == 1f)
			{
				predictedRot = Quaternion.LookRotation(player.transform.position - base.transform.position);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, predictedRot, Time.deltaTime * 100f / 2.5f);
			}
			if (difficulty > 2 && currentEnrageEffect == null && health < maxHealth / 2f)
			{
				Enrage();
			}
		}
	}

	public void GetHurt(GameObject target, Vector3 force, Vector3 hitPoint, float multiplier)
	{
		bool flag = false;
		bool flag2 = true;
		if (PlayerPrefs.GetInt("BlOn", 1) == 0)
		{
			flag2 = false;
		}
		GameObject gameObject = Object.Instantiate(smallBlood, hitPoint, Quaternion.identity);
		if (health > 0f)
		{
			gameObject.GetComponent<Bloodsplatter>().GetReady();
		}
		if (multiplier >= 1f)
		{
			gameObject.GetComponent<Bloodsplatter>().hpAmount = 30;
		}
		if (flag2)
		{
			gameObject.GetComponent<ParticleSystem>().Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (eid.hitter != "shotgun")
		{
			currentDrip = Object.Instantiate(dripBlood, hitPoint, Quaternion.identity);
			currentDrip.transform.parent = base.transform;
			currentDrip.transform.LookAt(base.transform);
			currentDrip.transform.Rotate(180f, 180f, 180f);
			if (flag2)
			{
				currentDrip.GetComponent<ParticleSystem>().Play();
			}
		}
		if (dead)
		{
			return;
		}
		health -= 1f * multiplier;
		if (scalc == null)
		{
			scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		}
		if (health <= 0f)
		{
			flag = true;
		}
		if (parryable && (eid.hitter == "shotgunzone" || eid.hitter == "punch"))
		{
			player.GetComponentInChildren<Punch>().Parry();
			currentExplosion = Object.Instantiate(beamExplosion, base.transform.position, Quaternion.identity);
			health -= 5f;
			Explosion[] componentsInChildren = currentExplosion.GetComponentsInChildren<Explosion>();
			Explosion[] array = componentsInChildren;
			foreach (Explosion explosion in array)
			{
				explosion.maxSize *= 1.75f;
				explosion.damage = 50;
				explosion.safeForPlayer = true;
				explosion.friendlyFire = true;
			}
			if (currentEnrageEffect == null)
			{
				CancelInvoke("BeamFire");
				Invoke("StopWaiting", 1f);
				Object.Destroy(currentCE);
			}
		}
		scalc.HitCalculator(eid.hitter, "spider", "", flag, base.gameObject);
		if (health <= 0f && !dead)
		{
			dead = true;
			Die();
		}
	}

	public void Die()
	{
		rb = GetComponentInChildren<Rigidbody>();
		falling = true;
		parryable = false;
		if (rev == null)
		{
			rev = player.GetComponentInChildren<Revolver>();
		}
		rb.isKinematic = false;
		rb.useGravity = true;
		for (int i = 1; i < base.transform.parent.childCount - 1; i++)
		{
			Object.Destroy(base.transform.parent.GetChild(i).gameObject);
		}
		if (currentCE != null)
		{
			Object.Destroy(currentCE);
		}
		Object.Destroy(nma);
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		component.kills++;
		ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
		if (componentInParent != null)
		{
			componentInParent.deadEnemies++;
		}
		if (currentEnrageEffect != null)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material = origMaterial;
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.material = origMaterial;
			}
			Object.Destroy(currentEnrageEffect);
		}
	}

	private void ShootProj()
	{
		currentBurst++;
		currentProj = Object.Instantiate(proj, mouth.position, base.transform.rotation);
		currentProj.transform.LookAt(hit.point);
		currentProj.GetComponent<Projectile>().safeEnemyType = EnemyType.Spider;
		if (difficulty > 2)
		{
			currentProj.GetComponent<Projectile>().speed *= 1.25f;
		}
		readyToShoot = false;
		if (difficulty > 2)
		{
			Invoke("ReadyToShoot", 0.1f);
		}
		else
		{
			Invoke("ReadyToShoot", 0.1f);
		}
	}

	private void ChargeBeam()
	{
		charging = true;
		currentCE = Object.Instantiate(chargeEffect, mouth);
		currentCE.transform.localScale = Vector3.zero;
		ceAud = currentCE.GetComponent<AudioSource>();
		ceLight = currentCE.GetComponent<Light>();
	}

	private void BeamChargeEnd()
	{
		if (beamsAmount <= 1)
		{
			ceAud.Stop();
		}
		Vector3 vector = new Vector3(nmov.rb.velocity.x, nmov.rb.velocity.y / 2f, nmov.rb.velocity.z);
		predictedPlayerPos = player.transform.position + vector / 2f;
		base.transform.LookAt(predictedPlayerPos);
		nma.enabled = false;
		predictedRot = Quaternion.LookRotation(predictedPlayerPos - base.transform.position);
		rotating = true;
		GameObject gameObject = Object.Instantiate(spark, mouth.position, mouth.rotation);
		gameObject.transform.localScale *= 2f;
		gameObject.transform.LookAt(predictedPlayerPos);
		if (difficulty > 2)
		{
			Invoke("BeamFire", 0.5f);
		}
		else
		{
			Invoke("BeamFire", 0.5f);
		}
		parryable = true;
	}

	private void BeamFire()
	{
		parryable = false;
		if (!dead)
		{
			Physics.Raycast(mouth.position, predictedPlayerPos - mouth.position, out hit2, float.PositiveInfinity, aimlm);
			currentBeam = Object.Instantiate(spiderBeam, mouth.position, mouth.rotation);
			LineRenderer component = currentBeam.GetComponent<LineRenderer>();
			component.SetPosition(0, mouth.position);
			component.SetPosition(1, hit2.point);
			currentExplosion = Object.Instantiate(beamExplosion, hit2.point, Quaternion.identity);
			currentExplosion.transform.forward = hit2.normal;
			Explosion[] componentsInChildren = currentExplosion.GetComponentsInChildren<Explosion>();
			Explosion[] array = componentsInChildren;
			foreach (Explosion explosion in array)
			{
				explosion.maxSize *= 2.25f;
				explosion.damage = 50;
				explosion.enemy = true;
			}
			rotating = false;
			if (beamsAmount > 1)
			{
				beamsAmount--;
				ceAud.pitch = 4f;
				ceAud.volume = 1f;
				Invoke("BeamChargeEnd", 0.5f);
			}
			else
			{
				Object.Destroy(currentCE);
				Invoke("StopWaiting", 1f);
			}
		}
	}

	private void StopWaiting()
	{
		if (!dead)
		{
			beamCharge = 0f;
		}
	}

	private void ReadyToShoot()
	{
		readyToShoot = true;
	}

	public void TriggerHit(Collider other)
	{
		if (falling && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "LimbEnd"))
		{
			EnemyIdentifier enemyIdentifier = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			enemyIdentifier.DeliverDamage(other.gameObject, Vector3.zero, other.transform.position, 999999f, true, 0f);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (falling && other.gameObject.tag == "Floor")
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			Object.Instantiate(impactParticle, base.transform.position, base.transform.rotation);
			spriteRot.eulerAngles = new Vector3(other.contacts[0].normal.x + 90f, other.contacts[0].normal.y, other.contacts[0].normal.z);
			spritePos = new Vector3(other.contacts[0].point.x, other.contacts[0].point.y + 0.1f, other.contacts[0].point.z);
			GameObject gameObject = Object.Instantiate(impactSprite, spritePos, spriteRot);
			gameObject.transform.SetParent(GetComponentInParent<GoreZone>().goreZone, true);
			base.transform.position = base.transform.position - base.transform.up * 1.5f;
			falling = false;
			NavMeshObstacle component = rb.GetComponent<NavMeshObstacle>();
			component.enabled = true;
			CameraController componentInChildren = player.GetComponentInChildren<CameraController>();
			componentInChildren.CameraShake(2f);
		}
	}

	public void Enrage()
	{
		if (!dead)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material = enrageMaterial;
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.material = enrageMaterial2;
			}
			currentEnrageEffect = Object.Instantiate(enrageEffect, base.transform);
			currentEnrageEffect.transform.localScale = Vector3.one * 0.2f;
		}
	}
}
