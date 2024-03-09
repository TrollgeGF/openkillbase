using UnityEngine;
using UnityEngine.UI;

public class Nailgun : MonoBehaviour
{
	private InputManager inman;

	public int variation;

	public GameObject[] shootPoints;

	private Spin[] barrels;

	private float spinSpeed;

	private int barrelNum;

	private Light[] barrelLights;

	private Material barrelHeat;

	private float heatUp = -0.5f;

	public GameObject muzzleFlash;

	public GameObject muzzleFlash2;

	public float fireRate;

	private float fireCooldown;

	private AudioSource aud;

	private AudioSource barrelAud;

	public GameObject nail;

	public GameObject harpoon;

	private CameraController cc;

	public float spread;

	private float currentSpread;

	private int burstAmount;

	private Animator anim;

	private bool canShoot = false;

	private NewMovement nm;

	private float harpoonCharge = 1f;

	private Slider harpoonSlider;

	private GameObject readyMessage;

	private bool pushing;

	private WeaponHUD whud;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		barrels = GetComponentsInChildren<Spin>();
		barrelLights = barrels[0].transform.parent.GetComponentsInChildren<Light>();
		barrelHeat = barrelLights[0].transform.parent.GetComponent<MeshRenderer>().sharedMaterial;
		barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, 0f);
		barrelAud = barrels[0].GetComponent<AudioSource>();
		aud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		nm = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
		harpoonSlider = GetComponentInChildren<Slider>();
		readyMessage = harpoonSlider.transform.parent.Find("Ready").gameObject;
	}

	private void OnDisable()
	{
		canShoot = false;
		harpoonCharge = 1f;
		if (nm == null)
		{
			nm = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
		}
		nm.slowMode = false;
		nm.pushForce = Vector3.zero;
	}

	private void Update()
	{
		if (whud == null)
		{
			whud = Camera.main.GetComponentInChildren<WeaponHUD>();
		}
		else if (whud.currentWeapon != 3 || whud.currentVariation != variation)
		{
			whud.UpdateImage(3, variation);
		}
		if (canShoot && Input.GetKey(inman.inputs["Fire1"]))
		{
			nm.slowMode = true;
			if (Mathf.Abs(nm.rb.velocity.x) < 0.1f && Mathf.Abs(nm.rb.velocity.z) < 0.1f)
			{
				if (heatUp < 0.5f)
				{
					nm.pushForce = new Vector3(nm.transform.forward.x * Mathf.Abs(0.5f - heatUp) * -1f, 0f, nm.transform.forward.z * Mathf.Abs(0.5f - heatUp) * -1f);
				}
				else
				{
					nm.pushForce = Vector3.zero;
				}
				pushing = true;
			}
			else if (pushing)
			{
				pushing = false;
				nm.pushForce = Vector3.zero;
			}
			heatUp = Mathf.MoveTowards(heatUp, 1f, Time.deltaTime * 0.5f);
			Light[] array = barrelLights;
			foreach (Light light in array)
			{
				light.intensity = heatUp * 10f;
			}
			barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, heatUp * 0.35f);
			if (heatUp < 0.5f)
			{
				spinSpeed = 1500f + Mathf.Abs(0.5f - heatUp) * 1000f;
			}
			else
			{
				spinSpeed = 1500f;
			}
			barrelAud.pitch = spinSpeed / 1500f * 2f;
			Spin[] array2 = barrels;
			foreach (Spin spin in array2)
			{
				spin.speed = spinSpeed;
			}
		}
		else
		{
			nm.slowMode = false;
			if (pushing)
			{
				pushing = false;
				nm.pushForce = Vector3.zero;
			}
			if (spinSpeed > 0f)
			{
				if (heatUp > 0f)
				{
					spinSpeed = 1500f + heatUp * 1000f;
				}
				else
				{
					spinSpeed = Mathf.MoveTowards(spinSpeed, 0f, Time.deltaTime * 3000f);
				}
				barrelAud.pitch = spinSpeed / 1500f * 2f;
				Spin[] array3 = barrels;
				foreach (Spin spin2 in array3)
				{
					spin2.speed = spinSpeed;
				}
			}
			if (heatUp > -0.5f)
			{
				heatUp = Mathf.MoveTowards(heatUp, -0.5f, Time.deltaTime * 2f);
				Light[] array4 = barrelLights;
				foreach (Light light2 in array4)
				{
					light2.intensity = heatUp * 10f;
				}
				barrelHeat.color = new Color(barrelHeat.color.r, barrelHeat.color.g, barrelHeat.color.b, heatUp * 0.25f);
			}
		}
		if (Input.GetKeyDown(inman.inputs["Fire2"]) && harpoonCharge == 1f)
		{
			harpoonCharge = 0f;
			readyMessage.SetActive(false);
			anim.SetTrigger("Shoot");
			GameObject gameObject = Object.Instantiate(harpoon, base.transform.position + base.transform.forward * 2.75f, base.transform.rotation);
			gameObject.transform.forward = base.transform.forward;
			gameObject.GetComponent<Rigidbody>().AddForce(base.transform.forward * 100f, ForceMode.VelocityChange);
			Object.Instantiate(muzzleFlash2, shootPoints[barrelNum].transform);
			cc.CameraShake(0.5f);
		}
		if (harpoonCharge < 1f)
		{
			if (Input.GetKey(inman.inputs["Fire1"]))
			{
				harpoonCharge = Mathf.MoveTowards(harpoonCharge, 1f, Time.deltaTime * 0.175f);
			}
			else
			{
				harpoonCharge = Mathf.MoveTowards(harpoonCharge, 1f, Time.deltaTime * 0.35f);
			}
			harpoonSlider.value = harpoonCharge;
			if (harpoonCharge == 1f)
			{
				readyMessage.SetActive(true);
				readyMessage.GetComponent<AudioSource>().Play();
			}
		}
	}

	private void FixedUpdate()
	{
		if (canShoot && Input.GetKey(inman.inputs["Fire1"]) && fireCooldown == 0f)
		{
			fireCooldown = fireRate;
			anim.SetTrigger("Shoot");
			barrelNum++;
			if (barrelNum >= shootPoints.Length)
			{
				barrelNum = 0;
			}
			GameObject gameObject = Object.Instantiate(muzzleFlash, shootPoints[barrelNum].transform);
			if (heatUp > 0f)
			{
				gameObject.GetComponent<AudioSource>().volume = 0.65f - heatUp * 0.1f;
			}
			currentSpread = spread;
			GameObject gameObject2 = Object.Instantiate(nail, base.transform.position + base.transform.forward, base.transform.rotation);
			gameObject2.transform.forward = base.transform.forward * -1f;
			gameObject2.transform.Rotate(Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f));
			gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * -100f, ForceMode.VelocityChange);
			gameObject2.GetComponent<Nail>().weaponType = "nailgun" + variation;
			cc.CameraShake(0.1f);
		}
		if (fireCooldown > 0f)
		{
			if (heatUp < 0.5f)
			{
				fireCooldown = Mathf.MoveTowards(fireCooldown, 0f, 0.0065f * (Mathf.Abs(0.5f - heatUp) / 1.35f + 1f));
			}
			else
			{
				fireCooldown = Mathf.MoveTowards(fireCooldown, 0f, 0.0065f);
			}
		}
	}

	public void BurstFire()
	{
		burstAmount--;
		barrelNum++;
		if (barrelNum >= shootPoints.Length)
		{
			barrelNum = 0;
		}
		GameObject gameObject = Object.Instantiate(muzzleFlash2, shootPoints[barrelNum].transform);
		if (heatUp > 0f)
		{
			currentSpread = spread + heatUp * 10f;
			gameObject.GetComponent<AudioSource>().volume = 0.65f - heatUp * 0.1f;
		}
		else
		{
			currentSpread = spread;
		}
		GameObject gameObject2 = Object.Instantiate(nail, base.transform.position + base.transform.forward, base.transform.rotation);
		gameObject2.transform.forward = base.transform.forward * -1f;
		gameObject2.transform.Rotate(Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f), Random.Range((0f - currentSpread) / 3f, currentSpread / 3f));
		gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * -100f, ForceMode.VelocityChange);
		gameObject2.GetComponent<Nail>().weaponType = "nailgun" + variation;
		cc.CameraShake(0.5f);
		if (burstAmount > 0)
		{
			Invoke("BurstFire", 0.03f);
		}
	}

	public void CanShoot()
	{
		canShoot = true;
	}
}
