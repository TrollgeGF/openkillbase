using UnityEngine;

public class Punch : MonoBehaviour
{
	private InputManager inman;

	public bool ready = true;

	private Animator anim;

	private SkinnedMeshRenderer smr;

	private Revolver rev;

	private AudioSource aud;

	private AudioSource aud2;

	private GameObject camObj;

	private CameraController cc;

	private RaycastHit hit;

	public LayerMask deflectionLayerMask;

	public LayerMask ignoreEnemyTrigger;

	private NewMovement nmov;

	private TrailRenderer tr;

	private Light parryLight;

	private GameObject currentDustParticle;

	public GameObject dustParticle;

	public GameObject parryFlash;

	public AudioClip normalHit;

	public AudioClip heavyHit;

	public AudioClip specialHit;

	private StyleHUD shud;

	private StatsManager sman;

	public bool holding;

	public Transform holder;

	public GameObject heldItem;

	private bool shopping;

	public GameObject parriedProjectileHitObject;

	private ProjectileParryZone ppz;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		anim = GetComponent<Animator>();
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		rev = base.transform.parent.parent.GetComponentInChildren<Revolver>();
		camObj = GameObject.FindWithTag("MainCamera");
		cc = camObj.GetComponent<CameraController>();
		aud = GetComponent<AudioSource>();
		aud2 = base.transform.GetChild(2).GetComponent<AudioSource>();
		nmov = GetComponentInParent<NewMovement>();
		tr = GetComponentInChildren<TrailRenderer>();
		parryLight = aud2.GetComponent<Light>();
		shud = camObj.GetComponentInChildren<StyleHUD>();
		sman = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(inman.inputs["Punch"]) && ready && !shopping)
		{
			if (rev == null)
			{
				ready = false;
				PunchStart();
			}
			else if (rev.gunVariation != 2)
			{
				PunchStart();
			}
		}
		float layerWeight = anim.GetLayerWeight(1);
		if (shopping && layerWeight < 1f)
		{
			anim.SetLayerWeight(1, Mathf.MoveTowards(layerWeight, 1f, Time.deltaTime / 10f + 5f * Time.deltaTime * (1f - layerWeight)));
		}
		else if (!shopping && layerWeight > 0f)
		{
			anim.SetLayerWeight(1, Mathf.MoveTowards(layerWeight, 0f, Time.deltaTime / 10f + 5f * Time.deltaTime * layerWeight));
		}
		if (Input.GetButtonDown("Fire1") && shopping)
		{
			anim.SetTrigger("ShopTap");
		}
	}

	private void PunchStart()
	{
		ready = false;
		anim.SetFloat("PunchRandomizer", Random.Range(0f, 1f));
		anim.SetTrigger("Punch");
		aud.pitch = Random.Range(0.9f, 1.1f);
		aud.Play();
		tr.widthMultiplier = 0.5f;
		if (rev == null)
		{
			rev = base.transform.parent.parent.GetComponentInChildren<Revolver>();
			if (rev != null)
			{
				rev.Punch();
			}
		}
		else
		{
			rev.Punch();
		}
		if (holding)
		{
			GetComponentInChildren<ItemIdentifier>().PunchWith();
		}
	}

	private void ActiveStart()
	{
		if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 4f, deflectionLayerMask) || Physics.BoxCast(camObj.transform.position, Vector3.one * 0.3f, camObj.transform.forward, out hit, camObj.transform.rotation, 4f, deflectionLayerMask))
		{
			if (hit.transform.gameObject.layer == 14)
			{
				Projectile component = hit.transform.gameObject.GetComponent<Projectile>();
				ThrownSword component2 = hit.transform.gameObject.GetComponent<ThrownSword>();
				if (component != null && !component.undeflectable)
				{
					component.hittingPlayer = false;
					component.friendly = true;
					component.speed *= 2f;
					component.explosionEffect = parriedProjectileHitObject;
					if (component.playerBullet)
					{
						component.boosted = true;
						component.GetComponent<SphereCollider>().radius *= 4f;
						component.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					}
					anim.Play("Hook", -1, 0.065f);
					if (!component.playerBullet)
					{
						Parry();
					}
					else
					{
						ParryFlash();
					}
					RaycastHit hitInfo;
					if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hitInfo, float.PositiveInfinity, ignoreEnemyTrigger))
					{
						hit.transform.LookAt(hitInfo.point);
					}
					else
					{
						hit.transform.LookAt(camObj.transform.position + camObj.transform.forward * 10f);
					}
				}
				else if (component2 != null && !component2.friendly && component2.active)
				{
					component2.GetParried();
					anim.Play("Hook", -1, 0.065f);
					Parry();
				}
			}
		}
		else
		{
			if (ppz == null)
			{
				ppz = base.transform.parent.GetComponentInChildren<ProjectileParryZone>();
			}
			if (ppz != null)
			{
				Projectile projectile = ppz.CheckParryZone();
				if (projectile != null && !projectile.undeflectable)
				{
					projectile.hittingPlayer = false;
					projectile.friendly = true;
					projectile.speed *= 2f;
					projectile.explosionEffect = parriedProjectileHitObject;
					projectile.CancelInvoke("TimeToDie");
					projectile.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					if (projectile.playerBullet)
					{
						projectile.boosted = true;
						projectile.GetComponent<SphereCollider>().radius *= 4f;
					}
					anim.Play("Hook", -1, 0.065f);
					if (!projectile.playerBullet)
					{
						Parry();
					}
					else
					{
						ParryFlash();
					}
					RaycastHit hitInfo2;
					if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hitInfo2, float.PositiveInfinity, ignoreEnemyTrigger))
					{
						projectile.transform.LookAt(hitInfo2.point);
					}
					else
					{
						projectile.transform.LookAt(camObj.transform.position + camObj.transform.forward * 10f);
					}
				}
			}
		}
		if (Physics.Raycast(camObj.transform.position, camObj.transform.forward, out hit, 3f, ignoreEnemyTrigger))
		{
			if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 24)
			{
				cc.CameraShake(0.2f);
				aud2.clip = normalHit;
				aud2.Play();
				currentDustParticle = Object.Instantiate(dustParticle, hit.point, base.transform.rotation);
				currentDustParticle.transform.forward = hit.normal;
				Breakable component3 = hit.collider.gameObject.GetComponent<Breakable>();
				if (component3 != null && component3.weak)
				{
					component3.Break();
				}
			}
			else if (hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "Head" || hit.transform.gameObject.tag == "Body" || hit.transform.gameObject.tag == "Limb" || hit.transform.gameObject.tag == "EndLimb")
			{
				if (anim.GetFloat("PunchRandomizer") < 0.5f)
				{
					anim.Play("Jab", -1, 0.075f);
				}
				else
				{
					anim.Play("Jab2", -1, 0.075f);
				}
				aud2.clip = heavyHit;
				aud2.Play();
				cc.HitStop(0.1f);
				cc.CameraShake(0.5f);
				EnemyIdentifier eid = hit.transform.GetComponent<EnemyIdentifierIdentifier>().eid;
				eid.rhit = hit;
				eid.hitter = "punch";
				eid.DeliverDamage(hit.transform.gameObject, (eid.transform.position - base.transform.position).normalized * 50000f, hit.point, 1f, false, 0f);
				if (holding)
				{
					heldItem.GetComponent<ItemIdentifier>().HitWith(hit.transform.gameObject);
				}
			}
			else
			{
				if (hit.transform.gameObject.layer != 22)
				{
					return;
				}
				ItemPickUp component4 = hit.transform.GetComponent<ItemPickUp>();
				ItemPlaceZone component5 = hit.transform.GetComponent<ItemPlaceZone>();
				if (component5 != null && holding)
				{
					holding = false;
					anim.SetBool("Holding", false);
					GameObject gameObject = Object.Instantiate(heldItem.GetComponent<ItemIdentifier>().putDownItem, hit.transform);
					component5.CheckItem();
					Object.Destroy(heldItem);
					Object.Instantiate(gameObject.GetComponent<ItemPickUp>().pickUpSound);
				}
				if (component4 != null && !holding)
				{
					holding = true;
					anim.SetBool("Holding", true);
					heldItem = Object.Instantiate(component4.item, holder);
					Object.Instantiate(component4.pickUpSound);
					ItemPlaceZone componentInParent = component4.GetComponentInParent<ItemPlaceZone>();
					component4.gameObject.SetActive(false);
					Object.Destroy(component4.gameObject);
					if (componentInParent != null)
					{
						componentInParent.CheckItem();
					}
				}
			}
		}
		else
		{
			cc.CameraShake(0.2f);
		}
	}

	public void CoinFlip()
	{
		if (ready)
		{
			anim.SetTrigger("CoinFlip");
		}
	}

	private void ActiveEnd()
	{
		tr.widthMultiplier = 0f;
	}

	private void PunchEnd()
	{
	}

	private void ReadyToPunch()
	{
		ready = true;
	}

	public void Parry()
	{
		aud.pitch = Random.Range(0.7f, 0.8f);
		nmov.GetHealth(999, true);
		ParryFlash();
		shud.AddPoints(100, "<color=lime>PARRY</color>");
	}

	public void ParryFlash()
	{
		parryLight.enabled = true;
		parryFlash.SetActive(true);
		Invoke("hideFlash", 0.1f);
		aud2.clip = specialHit;
		aud2.Play();
		parryFlash.GetComponent<AudioSource>().Play();
		cc.TrueStop(0.25f);
		cc.CameraShake(0.5f);
	}

	private void hideFlash()
	{
		parryLight.enabled = false;
		parryFlash.SetActive(false);
	}

	public void Hide()
	{
	}

	public void ShopMode()
	{
		shopping = true;
	}

	public void StopShop()
	{
		shopping = false;
	}
}
