using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	private Rigidbody rb;

	private bool checkingSpeed;

	private float timeToDelete = 1f;

	public LayerMask lmask;

	public GameObject refBeam;

	public Vector3 hitPoint = Vector3.zero;

	private Collider[] cols;

	public bool shot;

	public GameObject coinBreak;

	public float power;

	private EnemyIdentifier eid;

	public bool quickDraw;

	public Material uselessMaterial;

	private void Start()
	{
		Invoke("GetDeleted", 5f);
		Invoke("StartCheckingSpeed", 0.1f);
		rb = GetComponent<Rigidbody>();
		cols = GetComponents<Collider>();
		Collider[] array = cols;
		foreach (Collider collider in array)
		{
			collider.enabled = false;
		}
	}

	private void Update()
	{
		if (!shot)
		{
			if (checkingSpeed && rb.velocity.magnitude < 1f)
			{
				timeToDelete -= Time.deltaTime * 10f;
			}
			else
			{
				timeToDelete = 1f;
			}
			if (timeToDelete <= 0f)
			{
				GetDeleted();
			}
		}
	}

	public void DelayedReflectRevolver(Vector3 hitp)
	{
		if (checkingSpeed)
		{
			rb.isKinematic = true;
			shot = true;
			hitPoint = hitp;
			Invoke("ReflectRevolver", 0.1f);
		}
	}

	public void ReflectRevolver()
	{
		Coin[] array = Object.FindObjectsOfType<Coin>();
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		bool flag = false;
		if (array.Length > 1)
		{
			Coin[] array2 = array;
			foreach (Coin coin in array2)
			{
				if (coin != this)
				{
					float sqrMagnitude = (coin.transform.position - position).sqrMagnitude;
					RaycastHit hitInfo;
					if (sqrMagnitude < num && !Physics.Raycast(base.transform.position, coin.transform.position - base.transform.position, out hitInfo, Vector3.Distance(base.transform.position, coin.transform.position) - 0.5f, lmask))
					{
						gameObject = coin.gameObject;
						num = sqrMagnitude;
					}
				}
			}
			if (gameObject != null)
			{
				flag = true;
				Coin component = gameObject.GetComponent<Coin>();
				component.power = power + 1f;
				if (quickDraw)
				{
					component.quickDraw = true;
				}
				component.DelayedReflectRevolver(gameObject.transform.position);
				GameObject gameObject2 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity);
				LineRenderer component2 = gameObject2.GetComponent<LineRenderer>();
				if (power > 2f)
				{
					Debug.Log("power " + power);
					AudioSource[] components = component2.GetComponents<AudioSource>();
					AudioSource[] array3 = components;
					foreach (AudioSource audioSource in array3)
					{
						audioSource.pitch = 1f + (power - 2f) / 5f;
						audioSource.Play();
					}
				}
				if (hitPoint == Vector3.zero)
				{
					component2.SetPosition(0, base.transform.position);
				}
				else
				{
					component2.SetPosition(0, hitPoint);
				}
				component2.SetPosition(1, gameObject.transform.position);
			}
		}
		if (!flag)
		{
			GameObject[] array4 = GameObject.FindGameObjectsWithTag("Enemy");
			gameObject = null;
			num = float.PositiveInfinity;
			position = base.transform.position;
			GameObject[] array5 = array4;
			foreach (GameObject gameObject3 in array5)
			{
				float sqrMagnitude2 = (gameObject3.transform.position - position).sqrMagnitude;
				if (!(sqrMagnitude2 < num))
				{
					continue;
				}
				eid = gameObject3.GetComponent<EnemyIdentifier>();
				if (eid != null && !eid.dead)
				{
					Transform transform = ((!(eid.weakPoint != null)) ? eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform : eid.weakPoint.transform);
					RaycastHit hitInfo2;
					if (!Physics.Raycast(base.transform.position, transform.position - base.transform.position, out hitInfo2, Vector3.Distance(base.transform.position, transform.position) - 0.5f, lmask))
					{
						gameObject = gameObject3;
						num = sqrMagnitude2;
					}
					else
					{
						eid = null;
					}
				}
				else
				{
					eid = null;
				}
			}
			if (gameObject != null)
			{
				if (eid == null)
				{
					eid = gameObject.GetComponent<EnemyIdentifier>();
				}
				StyleHUD componentInChildren = GameObject.FindWithTag("MainCamera").GetComponentInChildren<StyleHUD>();
				if (eid.weakPoint != null && eid.weakPoint.GetComponent<EnemyIdentifierIdentifier>() != null)
				{
					componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
					if (quickDraw)
					{
						componentInChildren.AddPoints(50, "<color=cyan>QUICKDRAW</color>");
					}
					eid.hitter = "revolver";
					if (!eid.hitterWeapons.Contains("revolver1"))
					{
						eid.hitterWeapons.Add("revolver1");
					}
					eid.DeliverDamage(eid.weakPoint, (base.transform.position - eid.weakPoint.transform.position).normalized * 10000f, hitPoint, power, false, 1f);
				}
				else if (eid.weakPoint != null)
				{
					Breakable componentInChildren2 = eid.weakPoint.GetComponentInChildren<Breakable>();
					componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
					if (componentInChildren2.precisionOnly)
					{
						componentInChildren.AddPoints(100, "<color=lime>INTERRUPTION</color>");
						GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().ParryFlash();
					}
					componentInChildren2.Break();
				}
				else
				{
					componentInChildren.AddPoints(50, "<color=cyan>RICOSHOT</color>");
					eid.hitter = "revolver";
					eid.DeliverDamage(eid.GetComponentInChildren<EnemyIdentifierIdentifier>().gameObject, (base.transform.position - eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position).normalized * 10000f, hitPoint, power, false, 1f);
				}
				GameObject gameObject4 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity);
				LineRenderer component3 = gameObject4.GetComponent<LineRenderer>();
				if (power > 2f)
				{
					AudioSource[] components2 = component3.GetComponents<AudioSource>();
					AudioSource[] array6 = components2;
					foreach (AudioSource audioSource2 in array6)
					{
						audioSource2.pitch = 1f + (power - 2f) / 5f;
						audioSource2.Play();
					}
				}
				if (hitPoint == Vector3.zero)
				{
					component3.SetPosition(0, base.transform.position);
				}
				else
				{
					component3.SetPosition(0, hitPoint);
				}
				if (eid.weakPoint != null)
				{
					component3.SetPosition(1, eid.weakPoint.transform.position);
				}
				else
				{
					component3.SetPosition(1, eid.GetComponentInChildren<EnemyIdentifierIdentifier>().transform.position);
				}
				eid = null;
			}
			else
			{
				gameObject = null;
				List<GameObject> list = new List<GameObject>();
				array4 = GameObject.FindGameObjectsWithTag("Glass");
				GameObject[] array7 = array4;
				foreach (GameObject item in array7)
				{
					list.Add(item);
				}
				array4 = GameObject.FindGameObjectsWithTag("GlassFloor");
				GameObject[] array8 = array4;
				foreach (GameObject item2 in array8)
				{
					list.Add(item2);
				}
				if (list.Count > 0)
				{
					gameObject = null;
					num = float.PositiveInfinity;
					position = base.transform.position;
					foreach (GameObject item3 in list)
					{
						float sqrMagnitude3 = (item3.transform.position - position).sqrMagnitude;
						if (!(sqrMagnitude3 < num))
						{
							continue;
						}
						Glass componentInChildren3 = item3.GetComponentInChildren<Glass>();
						if (componentInChildren3 != null && !componentInChildren3.broken)
						{
							Transform transform2 = item3.transform;
							RaycastHit hitInfo3;
							if (!Physics.Raycast(base.transform.position, transform2.position - base.transform.position, out hitInfo3, Vector3.Distance(base.transform.position, transform2.position) - 0.5f, lmask) || hitInfo3.transform.gameObject.tag == "Glass" || hitInfo3.transform.gameObject.tag == "GlassFloor")
							{
								gameObject = item3;
								num = sqrMagnitude3;
							}
						}
					}
					if (gameObject != null)
					{
						gameObject.GetComponentInChildren<Glass>().Shatter();
						GameObject gameObject5 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity);
						LineRenderer component4 = gameObject5.GetComponent<LineRenderer>();
						if (power > 2f)
						{
							AudioSource[] components3 = component4.GetComponents<AudioSource>();
							AudioSource[] array9 = components3;
							foreach (AudioSource audioSource3 in array9)
							{
								audioSource3.pitch = 1f + (power - 2f) / 5f;
								audioSource3.Play();
							}
						}
						if (hitPoint == Vector3.zero)
						{
							component4.SetPosition(0, base.transform.position);
						}
						else
						{
							component4.SetPosition(0, hitPoint);
						}
						component4.SetPosition(1, gameObject.transform.position);
					}
				}
				if (list.Count == 0 || gameObject == null)
				{
					Vector3 normalized = Random.insideUnitSphere.normalized;
					GameObject gameObject6 = Object.Instantiate(refBeam, base.transform.position, Quaternion.identity);
					LineRenderer component5 = gameObject6.GetComponent<LineRenderer>();
					if (power > 2f)
					{
						AudioSource[] components4 = component5.GetComponents<AudioSource>();
						AudioSource[] array10 = components4;
						foreach (AudioSource audioSource4 in array10)
						{
							audioSource4.pitch = 1f + (power - 2f) / 5f;
							audioSource4.Play();
						}
					}
					if (hitPoint == Vector3.zero)
					{
						component5.SetPosition(0, base.transform.position);
					}
					else
					{
						component5.SetPosition(0, hitPoint);
					}
					RaycastHit hitInfo4;
					if (Physics.Raycast(base.transform.position, normalized, out hitInfo4, float.PositiveInfinity, lmask))
					{
						component5.SetPosition(1, hitInfo4.point);
					}
					else
					{
						component5.SetPosition(1, base.transform.position + normalized * 1000f);
					}
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 8 || collision.gameObject.layer == 24)
		{
			GoreZone componentInParent = collision.transform.GetComponentInParent<GoreZone>();
			if (componentInParent != null)
			{
				base.transform.SetParent(componentInParent.goreZone, true);
			}
			GetDeleted();
		}
	}

	public void GetDeleted()
	{
		Object.Instantiate(coinBreak, base.transform.position, Quaternion.identity);
		GetComponent<MeshRenderer>().material = uselessMaterial;
		Object.Destroy(GetComponent<AudioSource>());
		Object.Destroy(base.transform.GetChild(0).GetComponent<AudioSource>());
		Object.Destroy(GetComponent<TrailRenderer>());
		Object.Destroy(GetComponent<SphereCollider>());
		Object.Destroy(this);
	}

	private void StartCheckingSpeed()
	{
		Collider[] array = cols;
		foreach (Collider collider in array)
		{
			collider.enabled = true;
		}
		checkingSpeed = true;
	}
}
