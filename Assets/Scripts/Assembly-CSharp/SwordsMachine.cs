using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwordsMachine : MonoBehaviour
{
	public bool friendly;

	public Transform target;

	public Transform targetZone;

	public List<EnemyIdentifier> enemyTargets = new List<EnemyIdentifier>();

	public bool targetingEnemy;

	private EnemyIdentifier targetedEnemy;

	private NavMeshAgent nma;

	public GameObject player;

	private Rigidbody playerRb;

	private Animator anim;

	private Rigidbody rb;

	private Machine mach;

	public float phaseChangeHealth;

	public bool firstPhase;

	public bool active = true;

	public Transform rightArm;

	public bool inAction;

	public bool inSemiAction;

	private bool moveAtTarget;

	private Vector3 moveTarget;

	private float moveSpeed;

	public TrailRenderer swordTrail;

	public SkinnedMeshRenderer swordMR;

	public Material heatMat;

	private Material origMat;

	private AudioSource swordAud;

	public GameObject swingSound;

	public GameObject head;

	public GameObject flash;

	public GameObject gunFlash;

	private bool runningAttack = true;

	public float runningAttackCharge;

	public bool damaging;

	public int damage;

	private SwingCheck sc;

	public float runningAttackChance = 50f;

	private EnemyShotgun shotgun;

	private bool shotgunning;

	private bool gunDelay;

	public GameObject shotgunPickUp;

	public GameObject activateOnPhaseChange;

	private bool usingShotgun = false;

	public Transform secondPhasePosTarget;

	public CheckPoint cpToReset;

	public float swordThrowCharge = 3f;

	public int throwType;

	public GameObject[] thrownSword;

	private GameObject currentThrownSword;

	public Transform handTransform;

	public LayerMask swordThrowMask;

	private float swordThrowChance = 50f;

	private float spiralSwordChance = 50f;

	public float chaseThrowCharge = 0f;

	public GameObject bigPainSound;

	private Vector3 targetFuturePos;

	private int difficulty;

	public bool enraged;

	private float rageLeft;

	private SkinnedMeshRenderer smr;

	private Material normalMaterial;

	public Material enragedMaterial;

	private float normalAnimSpeed;

	private float normalMovSpeed;

	public GameObject enrageEffect;

	public GameObject currentEnrageEffect;

	private AudioSource enrageAud;

	public Door[] doorsInPath;

	private EnemyScanner esc;

	private void Start()
	{
		nma = GetComponent<NavMeshAgent>();
		player = GameObject.FindWithTag("Player");
		anim = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
		mach = GetComponent<Machine>();
		swordTrail.emitting = false;
		origMat = swordMR.sharedMaterial;
		swordAud = swordTrail.GetComponent<AudioSource>();
		sc = GetComponentInChildren<SwingCheck>();
		shotgun = GetComponentInChildren<EnemyShotgun>();
		gunDelay = true;
		if (!friendly)
		{
			target = player.transform;
			MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
			componentInChildren.PlayBattleMusic();
		}
		else if (target == null && targetZone != null)
		{
			target = targetZone;
		}
		else if (target.GetComponent<EnemyIdentifier>() != null)
		{
			targetingEnemy = true;
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			nma.speed += 3f;
			anim.speed = 1.2f;
			anim.SetFloat("ThrowSpeedMultiplier", 1.35f);
		}
		smr = base.transform.Find("SwordMachine").GetComponent<SkinnedMeshRenderer>();
		normalMaterial = smr.material;
		normalAnimSpeed = anim.speed;
		normalMovSpeed = nma.speed;
	}

	private void OnDisable()
	{
		if (GetComponent<BossHealthBar>() != null)
		{
			GetComponent<BossHealthBar>().DisappearBar();
		}
		if (currentThrownSword != null)
		{
			Object.Destroy(currentThrownSword);
		}
	}

	private void Update()
	{
		if (active)
		{
			if (friendly && enemyTargets.Count > 0)
			{
				if (target == null || target == targetZone)
				{
					float num = 100f;
					List<EnemyIdentifier> list = new List<EnemyIdentifier>();
					foreach (EnemyIdentifier enemyTarget in enemyTargets)
					{
						if (!Physics.Raycast(base.transform.position, enemyTarget.transform.position - base.transform.position, Vector3.Distance(enemyTarget.transform.position, base.transform.position), swordThrowMask))
						{
							list.Add(enemyTarget);
						}
					}
					bool flag = false;
					Door[] array = doorsInPath;
					foreach (Door door in array)
					{
						if (door.locked)
						{
							flag = true;
							break;
						}
					}
					if (!flag && list.Count == 0)
					{
						if (esc == null)
						{
							esc = GetComponentInChildren<EnemyScanner>();
						}
						foreach (EnemyIdentifier enemyTarget2 in enemyTargets)
						{
							if (esc != null && esc.spottedEnemies.Contains(enemyTarget2.gameObject) && !enemyTarget2.ignoredByEnemies)
							{
								esc.spottedEnemies.Remove(enemyTarget2.gameObject);
							}
						}
						enemyTargets.Clear();
						target = targetZone;
						targetingEnemy = false;
					}
					else
					{
						List<EnemyIdentifier> list2 = new List<EnemyIdentifier>();
						foreach (EnemyIdentifier enemyTarget3 in enemyTargets)
						{
							if (Vector3.Distance(base.transform.position, enemyTarget3.transform.position) < num && !enemyTarget3.dead)
							{
								num = Vector3.Distance(base.transform.position, enemyTarget3.transform.position);
								target = enemyTarget3.transform;
								targetingEnemy = true;
								targetedEnemy = enemyTarget3;
							}
							else if (enemyTarget3.dead)
							{
								list2.Add(enemyTarget3);
							}
						}
						if (list2.Count > 0)
						{
							foreach (EnemyIdentifier item in list2)
							{
								enemyTargets.Remove(item);
							}
						}
					}
				}
				else if (targetedEnemy.dead)
				{
					enemyTargets.Remove(targetedEnemy);
					if (enemyTargets.Count == 0 && targetZone != null)
					{
						target = targetZone;
						targetingEnemy = false;
					}
					else if (enemyTargets.Count == 0)
					{
						target = null;
						targetingEnemy = false;
					}
					else
					{
						float num2 = 100f;
						List<EnemyIdentifier> list3 = new List<EnemyIdentifier>();
						foreach (EnemyIdentifier enemyTarget4 in enemyTargets)
						{
							if (Vector3.Distance(base.transform.position, enemyTarget4.transform.position) < num2 && !enemyTarget4.dead)
							{
								num2 = Vector3.Distance(base.transform.position, enemyTarget4.transform.position);
								target = enemyTarget4.transform;
								targetingEnemy = true;
								targetedEnemy = enemyTarget4;
							}
							else if (enemyTarget4.dead)
							{
								list3.Add(enemyTarget4);
							}
						}
						if (list3.Count > 0)
						{
							foreach (EnemyIdentifier item2 in list3)
							{
								enemyTargets.Remove(item2);
							}
						}
					}
				}
			}
			if (!inAction)
			{
				nma.SetDestination(target.position);
				if (nma.velocity.magnitude > 0.1f)
				{
					anim.SetBool("Running", true);
				}
				else
				{
					anim.SetBool("Running", false);
				}
			}
			if (!friendly || targetingEnemy)
			{
				if (firstPhase && !enraged && !inAction && shotgun.gunReady && !gunDelay && !shotgunning && Vector3.Distance(target.position, base.transform.position) > 5f)
				{
					shotgunning = true;
					anim.SetLayerWeight(1, 1f);
					anim.SetTrigger("Shoot");
				}
				else if (!firstPhase && !enraged && !inAction && !inSemiAction && ((swordThrowCharge == 0f && Vector3.Distance(target.position, base.transform.position) > 5f) || Vector3.Distance(target.position, base.transform.position) > 20f))
				{
					swordThrowCharge = 3f;
					if ((float)Random.Range(0, 100) <= swordThrowChance || target.position.y > base.transform.position.y + 3f || Vector3.Distance(target.position, base.transform.position) > 16f)
					{
						inAction = true;
						throwType = 2;
						SwordThrow();
						if (swordThrowChance > 50f)
						{
							swordThrowChance = 25f;
						}
						else
						{
							swordThrowChance -= 25f;
						}
					}
					else if (swordThrowChance < 50f)
					{
						swordThrowChance = 75f;
					}
					else
					{
						swordThrowChance += 25f;
					}
				}
				if (runningAttack && !inAction && !inSemiAction && Vector3.Distance(target.position, base.transform.position) <= 8f && Vector3.Distance(target.position, base.transform.position) >= 5f)
				{
					runningAttackCharge = 3f;
					if ((float)Random.Range(0, 100) <= runningAttackChance)
					{
						if (runningAttackChance > 50f)
						{
							runningAttackChance = 50f;
						}
						runningAttackChance -= 25f;
						inAction = true;
						RunningSwing();
						if (shotgunning)
						{
							anim.SetLayerWeight(1, 0f);
							shotgunning = false;
							if (!gunDelay)
							{
								gunDelay = true;
								Invoke("ShootDelay", Random.Range(5, 10));
							}
						}
					}
					else
					{
						if (runningAttackChance < 50f)
						{
							runningAttackChance = 50f;
						}
						runningAttackChance += 25f;
						runningAttack = false;
					}
				}
				else if (!inAction && !inSemiAction && Vector3.Distance(target.position, base.transform.position) <= 5f)
				{
					inAction = true;
					if (shotgunning)
					{
						anim.SetLayerWeight(1, 0f);
						shotgunning = false;
						if (!gunDelay)
						{
							gunDelay = true;
							Invoke("ShootDelay", Random.Range(5, 10));
						}
					}
					if (!firstPhase && !enraged)
					{
						if ((float)Random.Range(0, 100) <= spiralSwordChance)
						{
							SwordSpiral();
							if (spiralSwordChance > 50f)
							{
								spiralSwordChance = 25f;
							}
							else
							{
								spiralSwordChance -= 25f;
							}
						}
						else
						{
							Combo();
							if (spiralSwordChance < 50f)
							{
								spiralSwordChance = 50f;
							}
							spiralSwordChance += 25f;
						}
					}
					else
					{
						Combo();
					}
				}
				if (!runningAttack && runningAttackCharge > 0f)
				{
					runningAttackCharge -= Time.deltaTime;
					if (runningAttackCharge <= 0f)
					{
						runningAttackCharge = 0f;
						runningAttack = true;
					}
				}
				if (!firstPhase)
				{
					if (swordThrowCharge > 0f && Vector3.Distance(target.position, base.transform.position) > 5f)
					{
						swordThrowCharge = Mathf.MoveTowards(swordThrowCharge, 0f, Time.deltaTime);
					}
					else
					{
						swordThrowCharge = 0f;
					}
					if (chaseThrowCharge > 0f)
					{
						chaseThrowCharge = Mathf.MoveTowards(chaseThrowCharge, 0f, Time.deltaTime);
					}
				}
			}
		}
		if (rageLeft > 0f)
		{
			rageLeft = Mathf.MoveTowards(rageLeft, 0f, Time.deltaTime);
			if (enrageAud != null && rageLeft < 3f)
			{
				enrageAud.pitch = rageLeft / 3f;
			}
			if (rageLeft <= 0f)
			{
				enraged = false;
				smr.material = normalMaterial;
				nma.speed = normalMovSpeed;
				anim.speed = normalAnimSpeed;
				if (currentEnrageEffect != null)
				{
					Object.Destroy(currentEnrageEffect);
				}
			}
		}
		if (firstPhase && mach.health <= phaseChangeHealth)
		{
			firstPhase = false;
			phaseChangeHealth = 0f;
			player.GetComponent<NewMovement>().GetHealth(999, true);
			EndFirstPhase();
		}
		if (firstPhase && mach.health < 110f && !usingShotgun)
		{
			usingShotgun = true;
			gunDelay = false;
		}
		if (mach.health < 95f)
		{
			gunDelay = false;
		}
	}

	private void FixedUpdate()
	{
		if (moveAtTarget)
		{
			rb.velocity = moveTarget * moveSpeed;
		}
		else
		{
			rb.velocity = Vector3.zero;
		}
	}

	public void RunningSwing()
	{
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		anim.SetTrigger("RunningSwing");
		rb.velocity = Vector3.zero;
		moveSpeed = 30f;
		damage = 40;
	}

	private void Combo()
	{
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		anim.SetTrigger("Combo");
		rb.velocity = Vector3.zero;
		moveSpeed = 60f;
		damage = 25;
	}

	private void SwordThrow()
	{
		Debug.Log("SwordThrow");
		anim.SetBool("Running", false);
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		anim.SetTrigger("SwordThrow");
		rb.velocity = Vector3.zero;
		damage = 0;
	}

	private void SwordSpiral()
	{
		throwType = 1;
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		anim.SetTrigger("SwordSpiral");
		rb.velocity = Vector3.zero;
		damage = 0;
	}

	public void StartMoving()
	{
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		rb.velocity = Vector3.zero;
		moveTarget = base.transform.forward;
		moveAtTarget = true;
	}

	public void StopMoving()
	{
		moveAtTarget = false;
		rb.velocity = Vector3.zero;
	}

	public void LookAt()
	{
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
	}

	public void StopAction()
	{
		mach.parryable = false;
		nma.updatePosition = true;
		nma.updateRotation = true;
		nma.enabled = true;
		inAction = false;
		runningAttack = true;
	}

	public void SemiStopAction()
	{
		Debug.Log("StopSemiAction");
		mach.parryable = false;
		nma.updatePosition = true;
		nma.updateRotation = true;
		nma.enabled = true;
		inSemiAction = true;
		inAction = false;
		anim.SetTrigger("AnimationCancel");
	}

	public void HeatSword()
	{
		swordTrail.emitting = true;
		swordMR.sharedMaterial = heatMat;
		swordAud.pitch = 1.5f;
		Object.Instantiate(flash, head.transform);
		mach.parryable = true;
	}

	public void HeatSwordThrow()
	{
		swordTrail.emitting = true;
		swordMR.sharedMaterial = heatMat;
		swordAud.pitch = 1.5f;
		Object.Instantiate(gunFlash, head.transform);
		mach.parryable = true;
		if (throwType == 2)
		{
			if (target == player.transform && playerRb == null)
			{
				playerRb = player.GetComponent<Rigidbody>();
			}
			if (target == player.transform)
			{
				targetFuturePos = target.transform.position + playerRb.velocity * (Vector3.Distance(base.transform.position, target.transform.position) / 80f) * Vector3.Distance(base.transform.position, target.transform.position) * 0.08f * anim.speed;
			}
			else
			{
				targetFuturePos = target.transform.position + target.GetComponent<NavMeshAgent>().velocity;
			}
			base.transform.LookAt(new Vector3(targetFuturePos.x, base.transform.position.y, targetFuturePos.z));
		}
	}

	public void CoolSword()
	{
		swordTrail.emitting = false;
		swordMR.sharedMaterial = origMat;
		swordAud.pitch = 1f;
	}

	public void DamageStart()
	{
		Object.Instantiate(swingSound, swordTrail.transform);
		damaging = true;
	}

	public void DamageStop()
	{
		damaging = false;
		mach.parryable = false;
	}

	public void ShootGun()
	{
		if (!inAction)
		{
			shotgun.Shoot();
		}
	}

	public void StopShootAnimation()
	{
		mach.parryable = false;
		anim.SetLayerWeight(1, 0f);
		gunDelay = true;
		shotgunning = false;
		Invoke("ShootDelay", Random.Range(5, 20));
	}

	private void ShootDelay()
	{
		gunDelay = false;
	}

	public void FlashGun()
	{
		GameObject gameObject = Object.Instantiate(gunFlash, head.transform);
	}

	public void SwordSpawn()
	{
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Knockdown") || anim.GetCurrentAnimatorStateInfo(0).IsName("LightKnockdown"))
		{
			Debug.Log("SwordSpawn");
		}
		mach.parryable = false;
		if (throwType != 2)
		{
			targetFuturePos = target.transform.position;
		}
		else if (target == player.transform)
		{
			targetFuturePos = new Vector3(targetFuturePos.x, target.transform.position.y + playerRb.velocity.y * Vector3.Distance(base.transform.position, target.transform.position) * 0.01f, targetFuturePos.z);
		}
		else
		{
			targetFuturePos = target.transform.position + target.GetComponent<NavMeshAgent>().velocity;
		}
		base.transform.LookAt(new Vector3(targetFuturePos.x, base.transform.position.y, targetFuturePos.z));
		currentThrownSword = Object.Instantiate(thrownSword[throwType], new Vector3(base.transform.position.x, handTransform.position.y, base.transform.position.z), Quaternion.identity);
		if (throwType != 1)
		{
			currentThrownSword.transform.rotation = base.transform.rotation;
		}
		swordMR.sharedMaterial = origMat;
		swordMR.enabled = false;
		swordTrail.emitting = false;
		swordAud.pitch = 0f;
		RaycastHit hitInfo;
		Physics.Raycast(base.transform.position + Vector3.up * 2f, (targetFuturePos - base.transform.position).normalized, out hitInfo, float.PositiveInfinity, swordThrowMask);
		currentThrownSword.GetComponentInChildren<ThrownSword>().SetPoints(hitInfo.point, handTransform);
		if (throwType == 2)
		{
			SemiStopAction();
		}
		Invoke("SwordCatch", 5f);
	}

	public void SwordCatch()
	{
		mach.parryable = false;
		inAction = true;
		inSemiAction = false;
		anim.SetTrigger("SwordCatch");
		swordMR.enabled = true;
		swordAud.pitch = 1f;
		swordThrowCharge = 3f;
		CancelInvoke("SwordCatch");
	}

	private void EndFirstPhase()
	{
		Debug.Log("EndFirstPhase");
		mach.parryable = false;
		inAction = true;
		inSemiAction = false;
		anim.SetLayerWeight(1, 0f);
		gunDelay = true;
		shotgunning = false;
		damaging = false;
		swordTrail.emitting = false;
		swordMR.sharedMaterial = origMat;
		swordAud.pitch = 1f;
		nma.enabled = true;
		nma.speed = 20f;
		active = false;
		moveAtTarget = false;
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
		Object.Instantiate(mach.limbBlood, rightArm.position, Quaternion.identity);
		Object.Instantiate(shotgunPickUp, rightArm.GetComponentInChildren<EnemyShotgun>().transform.position, rightArm.GetComponentInChildren<EnemyShotgun>().transform.rotation);
		CharacterJoint[] componentsInChildren = rightArm.GetComponentsInChildren<CharacterJoint>();
		if (componentsInChildren.Length > 0)
		{
			CharacterJoint[] array = componentsInChildren;
			foreach (CharacterJoint characterJoint in array)
			{
				Object.Destroy(characterJoint);
				characterJoint.transform.parent = null;
				characterJoint.transform.localScale = Vector3.zero;
			}
		}
		anim.Rebind();
		anim.SetTrigger("Knockdown");
		GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().SlowDown(0.15f);
		Object.Instantiate(bigPainSound, base.transform);
		if (secondPhasePosTarget != null)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().ArenaMusicEnd();
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
		normalMovSpeed = nma.speed;
		rageLeft = 0.01f;
	}

	public void Knockdown(bool light = false)
	{
		Debug.Log("Knockdown");
		mach.parryable = false;
		inAction = true;
		inSemiAction = false;
		anim.SetLayerWeight(1, 0f);
		gunDelay = true;
		shotgunning = false;
		damaging = false;
		swordMR.enabled = true;
		swordTrail.emitting = false;
		swordMR.sharedMaterial = origMat;
		swordAud.pitch = 1f;
		nma.enabled = true;
		nma.speed = 20f;
		active = false;
		moveAtTarget = false;
		nma.updatePosition = false;
		nma.updateRotation = false;
		nma.enabled = false;
		base.transform.LookAt(new Vector3(target.position.x, base.transform.position.y, target.position.z));
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
		moveAtTarget = false;
		if (light)
		{
			anim.Play("LightKnockdown");
		}
		else
		{
			anim.Play("Knockdown");
		}
		if (mach == null)
		{
			mach = GetComponent<Machine>();
		}
		if (!light)
		{
			GetComponent<EnemyIdentifier>().hitter = "projectile";
			if (mach.health > 20f)
			{
				mach.GetHurt(GetComponentInChildren<EnemyIdentifierIdentifier>().gameObject, Vector3.zero, 20f, 0f);
			}
			else
			{
				mach.GetHurt(GetComponentInChildren<EnemyIdentifierIdentifier>().gameObject, Vector3.zero, mach.health - 0.1f, 0f);
			}
		}
		GameObject gameObject = Object.Instantiate(mach.headBlood, GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position, Quaternion.identity);
		gameObject.GetComponent<Bloodsplatter>().GetReady();
		gameObject.GetComponent<ParticleSystem>().Play();
		if (!light)
		{
			Object.Instantiate(bigPainSound, base.transform);
		}
		Enrage();
	}

	public void Disappear()
	{
		if (secondPhasePosTarget != null && !firstPhase)
		{
			GetComponent<BossHealthBar>().DisappearBar();
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
			Object.Instantiate(mach.spawnEffect, position, base.transform.rotation);
			base.gameObject.SetActive(false);
			SwordsMachine[] componentsInChildren = secondPhasePosTarget.GetComponentsInChildren<SwordsMachine>();
			if (componentsInChildren.Length > 0)
			{
				SwordsMachine[] array = componentsInChildren;
				foreach (SwordsMachine swordsMachine in array)
				{
					swordsMachine.gameObject.SetActive(false);
					Object.Destroy(swordsMachine.gameObject);
				}
			}
			base.transform.position = secondPhasePosTarget.position;
			base.transform.parent = secondPhasePosTarget;
			base.gameObject.SetActive(true);
		}
		else
		{
			RaycastHit hitInfo;
			Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, float.PositiveInfinity, swordThrowMask);
			base.transform.position = hitInfo.point;
		}
		moveAtTarget = false;
		rb.isKinematic = false;
		active = true;
		nma.enabled = true;
		nma.updatePosition = true;
		nma.updateRotation = true;
		inAction = false;
		inSemiAction = false;
		if (activateOnPhaseChange != null && !firstPhase)
		{
			activateOnPhaseChange.SetActive(true);
		}
		GetComponent<AudioSource>().volume = 0f;
		if (secondPhasePosTarget != null && !firstPhase)
		{
			secondPhasePosTarget = null;
			cpToReset.UpdateRooms();
		}
	}

	public void Enrage()
	{
		enraged = true;
		rageLeft = 10f;
		anim.speed = normalAnimSpeed * 1.15f;
		nma.speed = normalMovSpeed * 1.25f;
		smr.material = enragedMaterial;
		GameObject gameObject = Object.Instantiate(bigPainSound, base.transform);
		gameObject.GetComponent<AudioSource>().pitch = 2f;
		if (currentEnrageEffect == null)
		{
			currentEnrageEffect = Object.Instantiate(enrageEffect, mach.chest.transform);
			enrageAud = currentEnrageEffect.GetComponent<AudioSource>();
		}
		enrageAud.pitch = 1f;
	}
}
