using UnityEngine;

public class Flammable : MonoBehaviour
{
	public float heat;

	public GameObject fire;

	private GameObject currentFire;

	private AudioSource currentFireAud;

	private Light currentFireLight;

	public bool burning;

	private bool fading;

	public bool secondary;

	private bool enemy;

	private EnemyIdentifierIdentifier eidid;

	private void Start()
	{
		if (base.gameObject.GetComponent<EnemyIdentifierIdentifier>() != null)
		{
			enemy = true;
			eidid = base.gameObject.GetComponent<EnemyIdentifierIdentifier>();
		}
	}

	private void FixedUpdate()
	{
		heat = Mathf.MoveTowards(heat, 0f, 0.005f);
		if (burning && (heat <= 0f || base.transform.localScale == Vector3.zero))
		{
			Fade();
		}
		if (fading && currentFire != null)
		{
			currentFire.transform.localScale = Vector3.MoveTowards(currentFire.transform.localScale, Vector3.zero, 0.075f);
			if (currentFireAud == null)
			{
				currentFireAud = currentFire.GetComponentInChildren<AudioSource>();
			}
			currentFireAud.volume = Mathf.MoveTowards(currentFireAud.volume, 0f, 0.005f);
			if (!secondary && currentFireLight != null)
			{
				currentFireLight.range = Mathf.MoveTowards(currentFireLight.range, 0f, 0.05f);
			}
			if (currentFire.transform.localScale == Vector3.zero)
			{
				fading = false;
				Object.Destroy(currentFire);
			}
		}
	}

	public void Burn(float newHeat)
	{
		if (GetComponent<Collider>() != null)
		{
			burning = true;
			if (newHeat > heat)
			{
				heat = newHeat;
			}
			if (currentFire == null)
			{
				currentFire = Object.Instantiate(fire);
				currentFire.transform.position = GetComponent<Collider>().bounds.center;
				currentFire.transform.localScale = GetComponent<Collider>().bounds.size;
				currentFire.transform.SetParent(base.transform, true);
				currentFireAud = currentFire.GetComponentInChildren<AudioSource>();
				if (!secondary && PlayerPrefs.GetInt("SimFir", 0) == 0)
				{
					currentFireLight = currentFire.GetComponent<Light>();
					currentFireLight.enabled = true;
				}
			}
			if (secondary)
			{
				return;
			}
			if (enemy)
			{
				Pulse();
			}
			Flammable[] componentsInChildren = GetComponentsInChildren<Flammable>();
			Flammable[] array = componentsInChildren;
			foreach (Flammable flammable in array)
			{
				if (flammable != this)
				{
					flammable.secondary = true;
					flammable.Burn(heat);
				}
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Pulse()
	{
		if (burning)
		{
			if (enemy)
			{
				eidid.eid.hitter = "fire";
				eidid.eid.DeliverDamage(eidid.gameObject, Vector3.zero, eidid.transform.position, 0.2f, false, 0f);
			}
			Invoke("Pulse", 0.5f);
		}
	}

	public void Fade()
	{
		burning = false;
		fading = true;
	}

	private void OnDestroy()
	{
		if (currentFire != null)
		{
			Object.Destroy(currentFire);
		}
	}
}
