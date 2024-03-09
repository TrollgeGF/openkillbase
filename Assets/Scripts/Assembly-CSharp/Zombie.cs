using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using cakeslice;

public class Zombie : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	private float originalHealth;

	private int difficulty;

	private Rigidbody[] rbs;

	public bool limp;

	public GameObject player;

	public NavMeshAgent nma;

	public Animator anim;

	private float currentSpeed;

	private Rigidbody rb;

	private ZombieMelee zm;

	private ZombieProjectiles zp;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	public float hurtSoundVol;

	public AudioClip deathSound;

	public float deathSoundVol;

	private GroundCheck gc;

	public bool grounded;

	private float defaultSpeed;

	public Vector3 agentVelocity;

	public GameObject bodyBlood;

	public GameObject limbBlood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject jawHalf;

	public GameObject brainChunk;

	public GameObject[] giblet;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private GoreZone gz;

	public Material deadMaterial;

	public Material simplifiedMaterial;

	private OptionsManager oman;

	private SkinnedMeshRenderer smr;

	private Material originalMaterial;

	private Outline oline;

	public GameObject chest;

	private float chestHP = 3f;

	public bool chestExploding;

	public GameObject chestExplosionStuff;

	public bool attacking;

	public LayerMask lmask;

	public Transform target;

	public List<EnemyIdentifier> enemyTargets = new List<EnemyIdentifier>();

	public bool friendly;

	public EnemyIdentifier targetedEnemy;

	private bool noheal;

	private float speedMultiplier = 1f;

	public bool stopped;

	public bool knockedBack;

	private float knockBackCharge;

	public bool falling;

	private void Start()
	{
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		zm = GetComponent<ZombieMelee>();
		zp = GetComponent<ZombieProjectiles>();
		anim = GetComponent<Animator>();
		gc = GetComponentInChildren<GroundCheck>();
		if (spawnIn)
		{
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
			Object.Instantiate(spawnEffect, position, base.transform.rotation);
			spawnIn = false;
		}
		originalHealth = health;
		oman = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		originalMaterial = smr.sharedMaterial;
		oline = GetComponentInChildren<Outline>();
		oline.enabled = false;
		if (!friendly)
		{
			target = player.transform;
			EnemyScanner componentInChildren = GetComponentInChildren<EnemyScanner>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.SetActive(false);
			}
		}
		if (limp)
		{
			noheal = true;
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			if (nma == null)
			{
				nma = GetComponent<NavMeshAgent>();
			}
			if (nma != null)
			{
				nma.acceleration = 60f;
				nma.angularSpeed = 2600f;
			}
			speedMultiplier = 1.25f;
		}
		if (nma != null)
		{
			defaultSpeed = nma.speed;
		}
	}

	private void Update()
	{
		if (anim != null)
		{
			anim.SetFloat("Speed", anim.speed);
		}
		if (knockBackCharge > 0f)
		{
			knockBackCharge = Mathf.MoveTowards(knockBackCharge, 0f, Time.deltaTime);
		}
		if (limp || !friendly || enemyTargets.Count <= 0)
		{
			return;
		}
		if (target == null)
		{
			float num = 100f;
			{
				foreach (EnemyIdentifier enemyTarget in enemyTargets)
				{
					if (Vector3.Distance(base.transform.position, enemyTarget.transform.position) < num)
					{
						num = Vector3.Distance(base.transform.position, enemyTarget.transform.position);
						target = enemyTarget.transform;
						targetedEnemy = enemyTarget;
					}
				}
				return;
			}
		}
		if (!targetedEnemy.dead)
		{
			return;
		}
		enemyTargets.Remove(targetedEnemy);
		if (enemyTargets.Count == 0)
		{
			target = null;
			return;
		}
		float num2 = 100f;
		foreach (EnemyIdentifier enemyTarget2 in enemyTargets)
		{
			if (Vector3.Distance(base.transform.position, enemyTarget2.transform.position) < num2)
			{
				num2 = Vector3.Distance(base.transform.position, enemyTarget2.transform.position);
				target = enemyTarget2.transform;
				targetedEnemy = enemyTarget2;
			}
		}
	}

	private void FixedUpdate()
	{
		if (limp)
		{
			return;
		}
		if (knockedBack && knockBackCharge <= 0f && gc.onGround)
		{
			StopKnockBack();
		}
		else if (knockedBack)
		{
			if (gc.onGround)
			{
				rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z * 0.9f);
			}
			nma.updatePosition = false;
			nma.updateRotation = false;
			nma.enabled = false;
			rb.isKinematic = false;
			rb.useGravity = true;
		}
		if (grounded && nma != null && nma.enabled)
		{
			if (nma.isStopped || nma.velocity == Vector3.zero || stopped)
			{
				anim.speed = 1f * speedMultiplier;
			}
			else
			{
				anim.speed = nma.velocity.magnitude / nma.speed * speedMultiplier;
			}
		}
		else if (!grounded && gc.onGround)
		{
			grounded = true;
			nma.speed = defaultSpeed;
		}
		if (!gc.onGround && !falling)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
			nma.enabled = false;
			anim.speed = 1f;
			falling = true;
			anim.SetBool("Falling", true);
			anim.SetTrigger("StartFalling");
			if (zp != null)
			{
				zp.CancelAttack();
			}
			if (zm != null)
			{
				zm.CancelAttack();
			}
		}
		else if (gc.onGround && falling)
		{
			nma.updatePosition = true;
			nma.updateRotation = true;
			rb.isKinematic = true;
			rb.useGravity = false;
			nma.enabled = true;
			nma.Warp(base.transform.position);
			falling = false;
			anim.SetBool("Falling", false);
		}
		if (!(simplifiedMaterial != null))
		{
			return;
		}
		if (smr.sharedMaterial != simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
		{
			smr.sharedMaterial = simplifiedMaterial;
		}
		else if (smr.sharedMaterial == simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
		{
			Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
			Vector3 vector2 = new Vector3(player.transform.position.x, player.transform.position.y + 0.6f, player.transform.position.z);
			RaycastHit hitInfo;
			if (Physics.Raycast(vector, vector2 - vector, out hitInfo, float.PositiveInfinity, lmask) && hitInfo.collider.gameObject.tag == "Player")
			{
				oline.enabled = true;
			}
			else
			{
				oline.enabled = false;
			}
		}
		else if (smr.sharedMaterial == simplifiedMaterial && (!oman.simplifyEnemies || Vector3.Distance(base.transform.position, player.transform.position) < oman.simplifiedDistance))
		{
			smr.sharedMaterial = originalMaterial;
			oline.enabled = false;
		}
	}

	public void KnockBack(Vector3 force)
	{
		nma.enabled = false;
		rb.isKinematic = false;
		rb.useGravity = true;
		if (!knockedBack || !gc.onGround)
		{
			rb.velocity = Vector3.zero;
		}
		if (!gc.onGround)
		{
			rb.AddForce(Vector3.up, ForceMode.VelocityChange);
		}
		rb.AddForce(force / 10f, ForceMode.VelocityChange);
		knockedBack = true;
		knockBackCharge = 0.1f;
	}

	public void StopKnockBack()
	{
		if (nma != null)
		{
			nma.updatePosition = true;
			nma.updateRotation = true;
			nma.enabled = true;
			rb.isKinematic = true;
			knockedBack = false;
			nma.Warp(base.transform.position);
		}
	}

	public void GetHurt(GameObject target, Vector3 force, float multiplier, float critMultiplier)
	{
		string hitLimb = "";
		bool dead = false;
		bool flag = false;
		bool flag2 = true;
		if (!gc.onGround)
		{
			multiplier *= 1.5f;
		}
		if (force != Vector3.zero && !limp)
		{
			KnockBack(force / 100f);
		}
		if (PlayerPrefs.GetInt("BlOn", 1) == 0)
		{
			flag2 = false;
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (chestExploding && health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb") && target.GetComponentInParent<EnemyIdentifier>() != null)
		{
			ChestExplodeEnd();
			Debug.Log("ChestExplodeEndPrematurely");
		}
		if (target.gameObject.tag == "Head")
		{
			float num = 1f * multiplier + multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire" && num > 0f)
			{
				GameObject gameObject = null;
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity) : Object.Instantiate(headBlood, target.transform.position, Quaternion.identity));
				Bloodsplatter component = gameObject.GetComponent<Bloodsplatter>();
				ParticleSystem component2 = component.GetComponent<ParticleSystem>();
				ParticleSystem.CollisionModule collision = component2.collision;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					if (Random.Range(0f, 1f) > 0.5f)
					{
						collision.enabled = false;
					}
					component.hpAmount = 3;
				}
				else if (eid.hitter == "nail")
				{
					component.hpAmount = 1;
					component.GetComponent<AudioSource>().volume *= 0.8f;
				}
				if (!noheal)
				{
					component.GetReady();
				}
			}
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "head";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				float num2 = 1f;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					num2 = 0.5f;
				}
				if (target.transform.parent != null && target.transform.parent.GetComponentInParent<Rigidbody>() != null)
				{
					target.transform.parent.GetComponentInParent<Rigidbody>().AddForce(force);
				}
				if (flag2 && eid.hitter != "harpoon")
				{
					for (int i = 0; (float)i < 6f * num2; i++)
					{
						Object.Instantiate(skullFragment, target.transform.position, Random.rotation);
					}
					for (int j = 0; (float)j < 4f * num2; j++)
					{
						Object.Instantiate(brainChunk, target.transform.position, Random.rotation);
					}
					for (int k = 0; (float)k < 2f * num2; k++)
					{
						Object.Instantiate(eyeBall, target.transform.position, Random.rotation);
						Object.Instantiate(jawHalf, target.transform.position, Random.rotation);
					}
				}
			}
		}
		else if (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			float num = 1f * multiplier + 0.5f * multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject2 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject2 = Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject2 = Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity);
				}
				if (gameObject2 != null)
				{
					Bloodsplatter component3 = gameObject2.GetComponent<Bloodsplatter>();
					ParticleSystem component4 = component3.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision2 = component4.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision2.enabled = false;
						}
						component3.hpAmount = 3;
					}
					else if (eid.hitter == "nail")
					{
						component3.hpAmount = 1;
						component3.GetComponent<AudioSource>().volume *= 0.8f;
					}
					if (!noheal && num > 0f)
					{
						component3.GetReady();
					}
				}
			}
			Vector3 normalized2 = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "limb";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.gameObject.tag == "Limb" && flag2 && eid.hitter != "harpoon")
				{
					float num3 = 1f;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						num3 = 0.5f;
					}
					for (int l = 0; (float)l < 4f * num3; l++)
					{
						Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
					}
				}
				else
				{
					target.transform.localScale = Vector3.zero;
				}
			}
		}
		else if (target.gameObject.tag == "Body" || (attacking && (eid.hitter == "shotgunzone" || eid.hitter == "punch") && player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
		{
			float num = 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (eid.hitter == "shotgunzone")
			{
				if (!attacking && (target.gameObject != chest || health - num > 0f))
				{
					num = 0f;
				}
				else if (attacking && (target.gameObject == chest || player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
				{
					num *= 2f;
					GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
				}
			}
			else if (eid.hitter == "punch" && attacking)
			{
				num = 2f;
				attacking = false;
				GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
			}
			health -= num;
			if (eid.hitter != "fire" && num > 0f)
			{
				GameObject gameObject3 = null;
				gameObject3 = ((!(num >= 1f) && !(health <= 0f)) ? Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity) : Object.Instantiate(bodyBlood, target.transform.position, Quaternion.identity));
				Bloodsplatter component5 = gameObject3.GetComponent<Bloodsplatter>();
				ParticleSystem component6 = component5.GetComponent<ParticleSystem>();
				ParticleSystem.CollisionModule collision3 = component6.collision;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					if (Random.Range(0f, 1f) > 0.5f)
					{
						collision3.enabled = false;
					}
					component5.hpAmount = 3;
				}
				else if (eid.hitter == "nail")
				{
					component5.hpAmount = 1;
					component5.GetComponent<AudioSource>().volume *= 0.8f;
				}
				if (!noheal)
				{
					component5.GetReady();
				}
			}
			if (health <= 0f && target.gameObject == chest)
			{
				if (eid.hitter == "shotgunzone")
				{
					chestHP = 0f;
				}
				else
				{
					chestHP -= num;
				}
				if (chestHP <= 0f && eid.hitter != "harpoon")
				{
					CharacterJoint[] componentsInChildren = target.GetComponentsInChildren<CharacterJoint>();
					GoreZone componentInParent = GetComponentInParent<GoreZone>();
					if (componentsInChildren.Length > 0)
					{
						CharacterJoint[] array = componentsInChildren;
						foreach (CharacterJoint characterJoint in array)
						{
							if (characterJoint.transform.parent.parent == chest.transform)
							{
								Rigidbody[] componentsInChildren2 = characterJoint.transform.GetComponentsInChildren<Rigidbody>();
								Rigidbody[] array2 = componentsInChildren2;
								foreach (Rigidbody rigidbody in array2)
								{
									rigidbody.isKinematic = false;
									rigidbody.useGravity = true;
								}
								Object.Destroy(characterJoint);
							}
						}
					}
					if (!limp && !eid.exploded && !eid.dead)
					{
						if (gc.onGround)
						{
							rb.isKinematic = true;
							knockedBack = false;
						}
						Debug.Log("Chest Exploding Now");
						anim.Rebind();
						anim.speed = 1f;
						anim.SetTrigger("ChestExplosion");
						chestExploding = true;
					}
					if (flag2)
					{
						float num4 = 1f;
						if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
						{
							num4 = 0.5f;
						}
						for (int num5 = 0; (float)num5 < 2f * num4; num5++)
						{
							Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
						}
						Object.Instantiate(chestExplosionStuff, target.transform.parent);
					}
					GameObject gameObject4 = Object.Instantiate(headBlood, target.transform.position, Quaternion.identity);
					gameObject4.GetComponent<Bloodsplatter>().hpAmount = 10;
					if (!noheal)
					{
						gameObject4.GetComponent<Bloodsplatter>().GetReady();
					}
					target.transform.localScale = Vector3.zero;
				}
			}
			if (!limp)
			{
				flag = true;
				hitLimb = "body";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.GetComponentInParent<Rigidbody>() != null)
				{
					target.GetComponentInParent<Rigidbody>().AddForce(force);
				}
			}
		}
		if (health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "Head") && eid.hitter != "harpoon")
		{
			if (target.transform.childCount > 0)
			{
				Transform child = target.transform.GetChild(0);
				CharacterJoint[] componentsInChildren3 = target.GetComponentsInChildren<CharacterJoint>();
				GoreZone componentInParent2 = GetComponentInParent<GoreZone>();
				if (componentsInChildren3.Length > 0)
				{
					CharacterJoint[] array3 = componentsInChildren3;
					foreach (CharacterJoint characterJoint2 in array3)
					{
						characterJoint2.transform.SetParent(componentInParent2.goreZone);
						Object.Destroy(characterJoint2);
					}
				}
				CharacterJoint component7 = target.GetComponent<CharacterJoint>();
				if (component7 != null)
				{
					component7.connectedBody = null;
					Object.Destroy(component7);
				}
				target.transform.position = child.position;
				target.transform.SetParent(child);
				Object.Destroy(target.GetComponent<Rigidbody>());
			}
			Object.Destroy(target.GetComponent<Collider>());
			target.transform.localScale = Vector3.zero;
		}
		else if (health <= 0f && target.gameObject.tag == "EndLimb" && eid.hitter != "harpoon")
		{
			target.transform.localScale = Vector3.zero;
		}
		if (health > 0f && hurtSounds.Length > 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = hurtSoundVol;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 12;
			aud.Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (!flag || !(eid.hitter != "enemy"))
		{
			return;
		}
		if (scalc == null)
		{
			scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		}
		if (health <= 0f)
		{
			dead = true;
			if (!gc.onGround && (eid.hitter == "explosion" || eid.hitter == "ffexplosion"))
			{
				scalc.shud.AddPoints(120, "<color=cyan>FIREWORKS</color>");
			}
			else if (!gc.onGround && eid.hitter != "deathzone")
			{
				scalc.shud.AddPoints(50, "<color=cyan>AIRSHOT</color>");
			}
		}
		if (eid.hitter != "secret")
		{
			scalc.HitCalculator(eid.hitter, "zombie", hitLimb, dead, base.gameObject);
		}
		if (GetComponentInChildren<Flammable>().burning && eid.hitter != "fire")
		{
			scalc.shud.AddPoints(50, "<color=cyan>FINISHED OFF</color>");
		}
	}

	public void GoLimp()
	{
		gz = GetComponentInParent<GoreZone>();
		attacking = false;
		Invoke("StopHealing", 1f);
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (gz != null && gz.checkpoint != null && eid.hitter != "enemy")
		{
			gz.AddDeath();
			gz.checkpoint.sm.kills++;
		}
		else if (eid.hitter != "enemy")
		{
			StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			component.kills++;
		}
		if (deadMaterial != null)
		{
			smr.sharedMaterial = deadMaterial;
		}
		else
		{
			smr.sharedMaterial = originalMaterial;
		}
		oline.enabled = false;
		if (zm != null)
		{
			zm.track = false;
			if (!chestExploding)
			{
				anim.StopPlayback();
			}
			if (zm.tr != null)
			{
				zm.tr.enabled = false;
			}
			Object.Destroy(base.gameObject.GetComponentInChildren<SwingCheck>().gameObject);
			Object.Destroy(zm);
		}
		if (zp != null)
		{
			zp.DamageEnd();
			if (!chestExploding)
			{
				anim.StopPlayback();
			}
			Object.Destroy(zp);
			Projectile componentInChildren = GetComponentInChildren<Projectile>();
			if (componentInChildren != null)
			{
				Object.Destroy(componentInChildren.gameObject);
			}
		}
		if (nma != null)
		{
			Object.Destroy(nma);
		}
		if (!chestExploding)
		{
			Object.Destroy(anim);
		}
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		if (deathSound != null)
		{
			aud.clip = deathSound;
			aud.volume = deathSoundVol;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
		if (!limp && !chestExploding)
		{
			rbs = GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array = rbs;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
			}
		}
		if (!limp)
		{
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
		}
		MusicManager componentInChildren2 = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		componentInChildren2.PlayCleanMusic();
		limp = true;
		EnemyScanner componentInChildren3 = GetComponentInChildren<EnemyScanner>();
		if (componentInChildren3 != null)
		{
			Object.Destroy(componentInChildren3.gameObject);
		}
	}

	public void ChestExplodeEnd()
	{
		anim.enabled = false;
		anim.StopPlayback();
		Object.Destroy(anim);
		rbs = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = rbs;
		foreach (Rigidbody rigidbody in array)
		{
			if (rigidbody != null)
			{
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
			}
		}
		chestExploding = false;
	}

	public void StopHealing()
	{
		noheal = true;
	}
}
