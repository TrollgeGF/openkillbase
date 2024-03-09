using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Revolver : MonoBehaviour
{
	public class RaycastResult : IComparable<RaycastResult>
	{
		public float distance;

		public Transform transform;

		public RaycastHit rrhit;

		public RaycastResult(RaycastHit hit)
		{
			distance = hit.distance;
			transform = hit.transform;
			rrhit = hit;
		}

		public int CompareTo(RaycastResult other)
		{
			return distance.CompareTo(other.distance);
		}
	}

	private InputManager inman;

	public int gunVariation;

	private WeaponHUD whud;

	public Transform kickBackPos;

	private AudioSource gunAud;

	private AudioSource superGunAud;

	public AudioClip[] gunShots;

	public AudioClip[] superGunShots;

	private int currentGunShot;

	public GameObject gunBarrel;

	private bool gunReady;

	private bool shootReady = true;

	private bool pierceReady = true;

	public float shootCharge;

	public float pierceCharge;

	public LayerMask pierceLayerMask;

	public LayerMask enemyLayerMask;

	private bool chargingPierce;

	public float pierceShotCharge;

	public Vector3 shotHitPoint;

	public GameObject beamPoint;

	public GameObject hitParticle;

	public GameObject superBeamPoint;

	public GameObject ricochetBeamPoint;

	public GameObject reflectedBeam;

	public GameObject superHitParticle;

	public GameObject ricochetHitParticle;

	public RaycastHit hit;

	public RaycastHit[] allHits;

	private int currentHit;

	private int currentHitMultiplier;

	public float recoilFOV;

	public GameObject chargeEffect;

	private AudioSource ceaud;

	private Light celight;

	private GameObject camObj;

	private Camera cam;

	private CameraController cc;

	private Transform camPositioner;

	private Vector3 tempCamPos;

	public Vector3 beamReflectPos;

	private GameObject beamDirectionSetter;

	public MeshRenderer screenMR;

	public Material batteryFull;

	public Material batteryMid;

	public Material batteryLow;

	public Material[] batteryCharges;

	private AudioSource screenAud;

	public AudioClip chargedSound;

	public AudioClip chargingSound;

	private int bodiesPierced;

	public GameObject blood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject dripBlood;

	private Enemy enemy;

	private CharacterJoint[] cjs;

	private CharacterJoint cj;

	private GameObject limb;

	private Transform firstChild;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject[] giblet;

	public GameObject brainChunk;

	public GameObject jawHalf;

	private int bulletForce;

	private bool slowMo;

	private bool timeStopped;

	private float untilTimeResume;

	private int enemiesLeftToHit;

	private int enemiesPierced;

	private RaycastHit subHit;

	private int currentShotType;

	private float damageMultiplier = 1f;

	private List<RaycastResult> hitList = new List<RaycastResult>();

	private SecondaryRevolver secRev;

	private bool twirling;

	private AudioClip chargeEffectSound;

	public AudioClip twirlSound;

	public bool twirlRecovery;

	private GameObject currentDrip;

	public GameObject coin;

	public LayerMask ignoreEnemyTrigger;

	private Turn cylinder;

	private SwitchMaterial rimLight;

	public GunControl gc;

	private Animator anim;

	private Punch punch;

	private NewMovement nmov;

	private Vector3 defaultGunPos;

	private Vector3 standardHoldPos;

	private Quaternion standardHoldRot;

	private Vector3 middleHoldPos = new Vector3(0.09f, -0.9f, 1.49f);

	private Quaternion middleHoldRot;

	public Image[] coinPanels;

	private float coinCharge = 400f;

	private Color panelColor;

	private WeaponCharges wc;

	private void Start()
	{
		inman = UnityEngine.Object.FindObjectOfType<InputManager>();
		gunReady = false;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		camObj = cam.gameObject;
		cc = camObj.GetComponent<CameraController>();
		nmov = cc.player.GetComponent<NewMovement>();
		camPositioner = camObj.transform.parent.Find("CamPositioner");
		shootCharge = 0f;
		pierceShotCharge = 0f;
		pierceCharge = 100f;
		pierceReady = false;
		shootReady = false;
		gunAud = GetComponent<AudioSource>();
		kickBackPos = base.transform.parent.GetChild(0);
		superGunAud = kickBackPos.GetComponent<AudioSource>();
		if (gunVariation == 0)
		{
			screenAud = screenMR.gameObject.GetComponent<AudioSource>();
		}
		else if (gunVariation == 1)
		{
			screenAud = GetComponentInChildren<Canvas>().GetComponent<AudioSource>();
		}
		secRev = base.transform.parent.GetComponentInChildren<SecondaryRevolver>();
		chargeEffect = gunBarrel.transform.GetChild(0).gameObject;
		ceaud = chargeEffect.GetComponent<AudioSource>();
		celight = chargeEffect.GetComponent<Light>();
		chargeEffectSound = ceaud.clip;
		whud = Camera.main.GetComponentInChildren<WeaponHUD>();
		if (gunVariation == 0)
		{
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			if (gunVariation == 0)
			{
				screenAud.pitch = 1f;
			}
			else if (gunVariation == 1)
			{
				screenAud.pitch = 1.25f;
			}
			screenAud.volume = 0.25f;
			screenAud.Play();
		}
		if (gunVariation == 2)
		{
			secRev.gameObject.SetActive(true);
			secRev.enabled = true;
		}
		cylinder = GetComponentInChildren<Turn>();
		gc = GetComponentInParent<GunControl>();
		beamDirectionSetter = new GameObject();
		anim = GetComponentInChildren<Animator>();
		defaultGunPos = base.transform.localPosition;
		standardHoldPos = base.transform.localPosition;
		standardHoldRot = base.transform.localRotation;
		middleHoldRot.eulerAngles = new Vector3(0f, 93f, 30f);
		if (PlayerPrefs.GetInt("HoldPos", 0) == 1)
		{
			base.transform.localPosition = middleHoldPos;
			base.transform.localRotation = middleHoldRot;
			defaultGunPos = middleHoldPos;
		}
		if (gunVariation == 1)
		{
			panelColor = coinPanels[0].color;
		}
		wc = UnityEngine.Object.FindObjectOfType<WeaponCharges>();
	}

	private void OnDisable()
	{
		if (wc == null)
		{
			wc = UnityEngine.Object.FindObjectOfType<WeaponCharges>();
		}
		if (gunVariation == 0)
		{
			wc.rev0charge = pierceCharge;
		}
		else if (gunVariation == 1)
		{
			wc.rev1charge = coinCharge;
		}
		pierceShotCharge = 0f;
		gunReady = false;
	}

	private void OnEnable()
	{
		if (wc == null)
		{
			wc = UnityEngine.Object.FindObjectOfType<WeaponCharges>();
		}
		if (gunVariation == 0)
		{
			pierceCharge = wc.rev0charge;
		}
		else if (gunVariation == 1)
		{
			coinCharge = wc.rev1charge;
			for (int i = 0; i < coinPanels.Length; i++)
			{
				coinPanels[i].fillAmount = coinCharge / 100f - (float)i;
				if (coinPanels[i].fillAmount < 1f)
				{
					coinPanels[i].color = Color.red;
				}
				else if (coinPanels[i].color == Color.red)
				{
					screenAud.pitch = 1f + (float)i / 2f;
					screenAud.Play();
					coinPanels[i].color = panelColor;
				}
			}
		}
		gunReady = false;
		CheckPosition();
	}

	private void Update()
	{
		if (whud == null)
		{
			whud = Camera.main.GetComponentInChildren<WeaponHUD>();
		}
		else if (whud.currentWeapon != 1 || whud.currentVariation != gunVariation)
		{
			whud.UpdateImage(1, gunVariation);
		}
		if (gunVariation == 2)
		{
			pierceCharge = shootCharge;
		}
		if (!shootReady)
		{
			if (gunVariation != 0)
			{
				if (shootCharge + 175f * Time.deltaTime < 100f)
				{
					shootCharge += 175f * Time.deltaTime;
				}
				else
				{
					shootCharge = 100f;
					shootReady = true;
				}
			}
			else if (shootCharge + 200f * Time.deltaTime < 100f)
			{
				shootCharge += 200f * Time.deltaTime;
			}
			else
			{
				shootCharge = 100f;
				shootReady = true;
			}
		}
		if (!pierceReady)
		{
			if (gunVariation == 0)
			{
				if (pierceCharge + 40f * Time.deltaTime < 100f)
				{
					pierceCharge += 40f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
					screenAud.clip = chargedSound;
					screenAud.loop = false;
					screenAud.volume = 0.35f;
					screenAud.pitch = UnityEngine.Random.Range(1f, 1.1f);
					screenAud.Play();
				}
				if (pierceCharge < 50f)
				{
					screenMR.material = batteryLow;
				}
				else if (pierceCharge < 100f)
				{
					screenMR.material = batteryMid;
				}
				else
				{
					screenMR.material = batteryFull;
				}
			}
			else if (gunVariation == 1)
			{
				if (pierceCharge + 480f * Time.deltaTime < 100f)
				{
					pierceCharge += 480f * Time.deltaTime;
				}
				else
				{
					pierceCharge = 100f;
					pierceReady = true;
				}
			}
		}
		else if (pierceShotCharge != 0f)
		{
			if (pierceShotCharge < 50f)
			{
				screenMR.material = batteryCharges[0];
			}
			else if (pierceShotCharge < 100f)
			{
				screenMR.material = batteryCharges[1];
			}
			else
			{
				screenMR.material = batteryCharges[2];
			}
			base.transform.localPosition = new Vector3(defaultGunPos.x + pierceShotCharge / 250f * UnityEngine.Random.Range(-0.05f, 0.05f), defaultGunPos.y + pierceShotCharge / 250f * UnityEngine.Random.Range(-0.05f, 0.05f), defaultGunPos.z + pierceShotCharge / 250f * UnityEngine.Random.Range(-0.05f, 0.05f));
			cylinder.spinSpeed = pierceShotCharge;
		}
		else if (gunVariation == 0 && screenMR.material != batteryFull)
		{
			screenMR.material = batteryFull;
		}
		if (gc.activated)
		{
			if (gunVariation == 0 && gunReady)
			{
				if ((Input.GetKeyUp(inman.inputs["Fire2"]) || Input.GetKeyDown(inman.inputs["Fire1"])) && shootReady && pierceShotCharge == 100f)
				{
					Shoot(2);
					pierceShotCharge = 0f;
				}
				else if (Input.GetKeyDown(inman.inputs["Fire1"]) && shootReady && !chargingPierce)
				{
					Shoot(1);
				}
				else if (Input.GetKey(inman.inputs["Fire2"]) && shootReady && pierceReady)
				{
					chargingPierce = true;
					if (pierceShotCharge + 175f * Time.deltaTime < 100f)
					{
						pierceShotCharge += 175f * Time.deltaTime;
					}
					else
					{
						pierceShotCharge = 100f;
					}
				}
				else
				{
					chargingPierce = false;
					if (pierceShotCharge - 175f * Time.deltaTime > 0f)
					{
						pierceShotCharge -= 175f * Time.deltaTime;
					}
					else
					{
						pierceShotCharge = 0f;
					}
				}
			}
			else if (gunVariation == 1)
			{
				if (Input.GetKeyDown(inman.inputs["Fire2"]) && pierceReady && coinCharge >= 100f)
				{
					coinCharge -= 100f;
					if (punch == null)
					{
						punch = cc.GetComponentInChildren<Punch>();
					}
					punch.CoinFlip();
					GameObject gameObject = UnityEngine.Object.Instantiate(coin, camObj.transform.position + camObj.transform.up * -0.5f, camObj.transform.rotation);
					gameObject.GetComponent<Rigidbody>().AddForce(camObj.transform.forward * 20f + Vector3.up * 15f + cc.player.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
					pierceCharge = 0f;
					pierceReady = false;
				}
				else if (gunReady && Input.GetKeyDown(inman.inputs["Fire1"]) && shootReady)
				{
					Shoot(1);
					twirling = false;
					if (ceaud.volume != 0f)
					{
						ceaud.volume = 0f;
					}
					twirlRecovery = false;
				}
				else
				{
					twirling = false;
				}
			}
			else if (gunVariation == 2 && gunReady && Input.GetKeyDown(inman.inputs["Fire2"]) && shootReady)
			{
				Shoot(1);
			}
		}
		if (pierceShotCharge == 0f && celight.enabled)
		{
			celight.enabled = false;
		}
		else if (pierceShotCharge != 0f)
		{
			celight.enabled = true;
			celight.range = pierceShotCharge * 0.01f;
		}
		chargeEffect.transform.localScale = Vector3.one * pierceShotCharge * 0.02f;
		if (gunVariation == 0)
		{
			ceaud.volume = 0.25f + pierceShotCharge * 0.005f;
			ceaud.pitch = pierceShotCharge * 0.005f;
		}
		if (gunVariation != 1 || coinCharge == 400f)
		{
			return;
		}
		coinCharge = Mathf.MoveTowards(coinCharge, 400f, Time.deltaTime * 25f);
		for (int i = 0; i < coinPanels.Length; i++)
		{
			coinPanels[i].fillAmount = coinCharge / 100f - (float)i;
			if (coinPanels[i].fillAmount < 1f)
			{
				coinPanels[i].color = Color.red;
			}
			else if (coinPanels[i].color == Color.red)
			{
				screenAud.pitch = 1f + (float)i / 2f;
				screenAud.Play();
				coinPanels[i].color = panelColor;
			}
		}
	}

	private void Shoot(int shotType)
	{
		if (camPositioner == null)
		{
			camPositioner = camObj.transform.parent.Find("CamPositioner");
		}
		switch (shotType)
		{
		case 1:
			cc.StopShake();
			currentShotType = shotType;
			bulletForce = 5000;
			if (Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, ignoreEnemyTrigger))
			{
				if (hit.transform.gameObject.layer == 8)
				{
					RaycastHit hitInfo;
					if (Physics.BoxCast(camPositioner.position, Vector3.one * 0.2f, camObj.transform.forward, out hitInfo, camObj.transform.rotation, Vector3.Distance(camPositioner.position, hit.point), enemyLayerMask))
					{
						ExecuteHits(hitInfo);
						shotHitPoint = hit.point;
						UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
					}
					else
					{
						ExecuteHits(hit);
						shotHitPoint = hit.point;
						UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
					}
				}
				else
				{
					ExecuteHits(hit);
					shotHitPoint = hit.point;
					UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
				}
			}
			else
			{
				shotHitPoint = camPositioner.position + camObj.transform.forward * 1000f;
			}
			UnityEngine.Object.Instantiate(beamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			shootReady = false;
			shootCharge = 0f;
			currentGunShot = UnityEngine.Random.Range(0, gunShots.Length);
			gunAud.clip = gunShots[currentGunShot];
			gunAud.volume = 0.5f;
			gunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			gunAud.Play();
			cam.fieldOfView += cc.defaultFov / 40f;
			break;
		case 2:
			cc.StopShake();
			bulletForce = 20000;
			Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, pierceLayerMask);
			shotHitPoint = hit.point;
			UnityEngine.Object.Instantiate(superBeamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			UnityEngine.Object.Instantiate(superHitParticle, hit.point, base.transform.rotation);
			beamDirectionSetter.transform.position = camObj.transform.position;
			beamDirectionSetter.transform.LookAt(hit.point);
			bodiesPierced = 0;
			allHits = Physics.BoxCastAll(camObj.transform.position, Vector3.one * 0.3f, camObj.transform.forward, camObj.transform.rotation, hit.distance, enemyLayerMask);
			enemiesPierced = 0;
			currentShotType = shotType;
			PiercingShotOrder();
			shootReady = false;
			shootCharge = 0f;
			pierceReady = false;
			pierceCharge = 0f;
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			screenAud.pitch = 1f;
			screenAud.volume = 0.25f;
			screenAud.Play();
			currentGunShot = UnityEngine.Random.Range(0, superGunShots.Length);
			superGunAud.clip = superGunShots[currentGunShot];
			superGunAud.volume = 0.5f;
			superGunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			superGunAud.Play();
			cam.fieldOfView += cc.defaultFov / 20f;
			break;
		case 4:
			cc.StopShake();
			Debug.ClearDeveloperConsole();
			bulletForce = 5000;
			Physics.Raycast(camPositioner.position, camObj.transform.forward, out hit, float.PositiveInfinity, ignoreEnemyTrigger);
			if (hit.transform.gameObject.layer == 8)
			{
				bodiesPierced = 0;
				tempCamPos = Vector3.Reflect(camObj.transform.forward, hit.normal);
				Physics.Raycast(hit.point, tempCamPos, out subHit, float.PositiveInfinity, pierceLayerMask);
				beamReflectPos = subHit.point;
				UnityEngine.Object.Instantiate(reflectedBeam, hit.point, base.transform.rotation);
				beamDirectionSetter.transform.position = hit.point;
				beamDirectionSetter.transform.LookAt(beamReflectPos);
				allHits = Physics.BoxCastAll(beamDirectionSetter.transform.position, Vector3.one * 0.6f, beamDirectionSetter.transform.forward, beamDirectionSetter.transform.rotation, hit.distance, enemyLayerMask);
				currentShotType = shotType;
				PiercingShotOrder();
			}
			else
			{
				ExecuteHits(hit);
			}
			shotHitPoint = hit.point;
			UnityEngine.Object.Instantiate(beamPoint, gunBarrel.transform.position, gunBarrel.transform.rotation);
			UnityEngine.Object.Instantiate(hitParticle, hit.point, base.transform.rotation);
			shootReady = false;
			shootCharge = 0f;
			pierceReady = false;
			pierceCharge = 0f;
			screenAud.clip = chargingSound;
			screenAud.loop = true;
			screenAud.pitch = 1.25f;
			screenAud.volume = 0.25f;
			screenAud.Play();
			currentGunShot = UnityEngine.Random.Range(0, gunShots.Length);
			gunAud.clip = gunShots[currentGunShot];
			gunAud.volume = 0.5f;
			gunAud.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			gunAud.Play();
			cam.fieldOfView = recoilFOV;
			break;
		}
		cylinder.DoTurn();
		anim.SetFloat("RandomChance", UnityEngine.Random.Range(0f, 1f));
		if (shotType == 1)
		{
			anim.SetTrigger("Shoot");
		}
		else
		{
			anim.SetTrigger("ChargeShoot");
		}
		gunReady = false;
	}

	private void PiercingShotOrder()
	{
		hitList.Clear();
		RaycastHit[] array = allHits;
		foreach (RaycastHit raycastHit in array)
		{
			hitList.Add(new RaycastResult(raycastHit));
		}
		if (gunVariation == 1)
		{
			hitList.Add(new RaycastResult(subHit));
		}
		else
		{
			hitList.Add(new RaycastResult(hit));
		}
		hitList.Sort();
		PiercingShotCheck();
	}

	private void PiercingShotCheck()
	{
		if (enemiesPierced < hitList.Count)
		{
			if (hitList[enemiesPierced].transform == null)
			{
				enemiesPierced++;
				PiercingShotCheck();
			}
			else if ((hitList[enemiesPierced].transform.gameObject.tag == "Enemy" || hitList[enemiesPierced].transform.gameObject.tag == "Limb" || hitList[enemiesPierced].transform.gameObject.tag == "Head" || hitList[enemiesPierced].transform.gameObject.tag == "EndLimb" || hitList[enemiesPierced].transform.gameObject.tag == "Body") && bodiesPierced < 3)
			{
				EnemyIdentifier eid = hitList[enemiesPierced].transform.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
				if (eid != null)
				{
					if (eid.type == EnemyType.Zombie && !eid.zombie.limp)
					{
						UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
						ExecuteHits(hitList[enemiesPierced].rrhit);
						bodiesPierced++;
						cc.HitStop(0.05f);
						if (eid.zombie.health <= 0f)
						{
							enemiesPierced++;
						}
						Invoke("PiercingShotCheck", 0.05f);
					}
					else if ((eid.type == EnemyType.Machine || eid.type == EnemyType.Statue) && !eid.dead)
					{
						UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
						ExecuteHits(hitList[enemiesPierced].rrhit);
						bodiesPierced++;
						cc.HitStop(0.05f);
						if (eid.dead)
						{
							enemiesPierced++;
						}
						Invoke("PiercingShotCheck", 0.05f);
					}
					else if (eid.type == EnemyType.Spider || eid.type == EnemyType.Drone)
					{
						UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
						ExecuteHits(hitList[enemiesPierced].rrhit);
						bodiesPierced++;
						cc.HitStop(0.05f);
						Invoke("PiercingShotCheck", 0.05f);
					}
					else
					{
						ExecuteHits(hitList[enemiesPierced].rrhit);
						enemiesPierced++;
						PiercingShotCheck();
					}
				}
				else if (hitList[enemiesPierced].transform.gameObject.GetComponentInParent<SpiderBody>() != null)
				{
					UnityEngine.Object.Instantiate(superHitParticle, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
					ExecuteHits(hitList[enemiesPierced].rrhit);
					bodiesPierced++;
					cc.HitStop(0.05f);
					if (hitList[enemiesPierced].rrhit.transform == null)
					{
						enemiesPierced++;
					}
					Invoke("PiercingShotCheck", 0.05f);
				}
				else
				{
					ExecuteHits(hitList[enemiesPierced].rrhit);
					enemiesPierced++;
					PiercingShotCheck();
				}
			}
			else if (currentShotType == 4 && hitList[enemiesPierced].transform.gameObject.layer == 8 && bodiesPierced < 2)
			{
				Invoke("ReflectCheck", 0.1f);
			}
			else if (currentShotType == 2 && (hitList[enemiesPierced].transform.gameObject.tag == "Glass" || hitList[enemiesPierced].transform.gameObject.tag == "GlassFloor"))
			{
				Glass component = hitList[enemiesPierced].transform.gameObject.GetComponent<Glass>();
				if (!component.broken)
				{
					component.Shatter();
				}
				enemiesPierced++;
				PiercingShotCheck();
			}
			else
			{
				enemiesPierced++;
				PiercingShotCheck();
			}
		}
		else
		{
			enemiesPierced = 0;
		}
	}

	private void ReflectCheck()
	{
		tempCamPos = Vector3.Reflect(beamDirectionSetter.transform.forward, hitList[enemiesPierced].rrhit.normal);
		Physics.Raycast(hitList[enemiesPierced].rrhit.point, tempCamPos, out subHit, float.PositiveInfinity, pierceLayerMask);
		beamReflectPos = subHit.point;
		UnityEngine.Object.Instantiate(reflectedBeam, hitList[enemiesPierced].rrhit.point, base.transform.rotation);
		beamDirectionSetter.transform.position = hitList[enemiesPierced].rrhit.point;
		beamDirectionSetter.transform.LookAt(beamReflectPos);
		allHits = Physics.BoxCastAll(beamDirectionSetter.transform.position, Vector3.one * 0.3f, beamDirectionSetter.transform.forward, beamDirectionSetter.transform.rotation, hit.distance, enemyLayerMask);
		Debug.Log(allHits);
		bodiesPierced++;
		PiercingShotOrder();
	}

	public void ExecuteHits(RaycastHit currentHit)
	{
		if (!(currentHit.transform != null))
		{
			return;
		}
		if (currentHit.transform.gameObject.tag == "Enemy" || currentHit.transform.gameObject.tag == "Body" || currentHit.transform.gameObject.tag == "Limb" || currentHit.transform.gameObject.tag == "EndLimb" || currentHit.transform.gameObject.tag == "Head")
		{
			if (currentShotType == 2)
			{
				cc.CameraShake(1f);
			}
			else
			{
				cc.CameraShake(0.5f);
			}
			EnemyIdentifier eid = currentHit.transform.GetComponent<EnemyIdentifierIdentifier>().eid;
			if (!eid.dead && anim.GetCurrentAnimatorStateInfo(0).IsName("PickUp"))
			{
				cc.GetComponentInChildren<StyleHUD>().AddPoints(50, "<color=cyan>QUICKDRAW</color>");
			}
			eid.rhit = currentHit;
			eid.hitter = "revolver";
			if (!eid.hitterWeapons.Contains("revolver" + gunVariation))
			{
				eid.hitterWeapons.Add("revolver" + gunVariation);
			}
			if (!eid.dead && currentHit.transform.gameObject.tag == "Head")
			{
				gc.headshots++;
				gc.headShotComboTime = 3f;
			}
			else if (currentHit.transform.gameObject.tag != "Head")
			{
				gc.headshots = 0;
				gc.headShotComboTime = 0f;
			}
			eid.DeliverDamage(currentHit.transform.gameObject, (currentHit.transform.position - base.transform.position).normalized * bulletForce, currentHit.point, damageMultiplier, false, 1f);
			if (gc.headshots > 1)
			{
				cc.GetComponentInChildren<StyleHUD>().AddPoints(gc.headshots * 20, "<color=cyan>HEADSHOT COMBO x" + gc.headshots + "</color>");
			}
		}
		else
		{
			gc.headshots = 0;
			gc.headShotComboTime = 0f;
		}
		Breakable component = currentHit.transform.GetComponent<Breakable>();
		if (component != null && (currentShotType == 2 || component.weak))
		{
			if (component.precisionOnly)
			{
				if (punch == null)
				{
					punch = cc.GetComponentInChildren<Punch>();
				}
				cc.GetComponentInChildren<StyleHUD>().AddPoints(100, "<color=lime>INTERRUPTION</color>");
				punch.ParryFlash();
			}
			component.Break();
		}
		Coin component2 = currentHit.transform.GetComponent<Coin>();
		if (component2 != null)
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("PickUp"))
			{
				component2.quickDraw = true;
			}
			component2.DelayedReflectRevolver(currentHit.point);
		}
	}

	private void Twirl()
	{
		twirlRecovery = true;
		if (ceaud.volume != 0.5f)
		{
			ceaud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
			ceaud.volume = 0.5f;
		}
		base.transform.Rotate(0f, 0f, Time.deltaTime * -1000f, Space.Self);
	}

	private void ReadyToShoot()
	{
		shootReady = true;
	}

	public void Punch()
	{
		gunReady = false;
		anim.SetTrigger("ChargeShoot");
	}

	public void ReadyGun()
	{
		gunReady = true;
	}

	public void CheckPosition()
	{
		if (defaultGunPos != Vector3.zero)
		{
			if (PlayerPrefs.GetInt("HoldPos", 0) == 1)
			{
				base.transform.localPosition = middleHoldPos;
				base.transform.localRotation = middleHoldRot;
				defaultGunPos = middleHoldPos;
			}
			else
			{
				base.transform.localPosition = standardHoldPos;
				base.transform.localRotation = standardHoldRot;
				defaultGunPos = standardHoldPos;
			}
		}
	}
}
