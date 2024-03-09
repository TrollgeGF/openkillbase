using System.Collections.Generic;
using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
	public EnemyType type;

	public Zombie zombie;

	public SpiderBody spider;

	public Machine machine;

	public Statue statue;

	public Wicked wicked;

	public Drone drone;

	public RaycastHit rhit;

	public string hitter;

	public List<string> hitterWeapons = new List<string>();

	public string[] weaknesses;

	public float weaknessMultiplier;

	public GameObject weakPoint;

	public bool exploded;

	public bool dead;

	public DoorController usingDoor;

	public bool ignoredByEnemies;

	private void Start()
	{
		if (type == EnemyType.Zombie)
		{
			zombie = GetComponent<Zombie>();
		}
		else if (type == EnemyType.Spider)
		{
			spider = GetComponent<SpiderBody>();
		}
		else if (type == EnemyType.Machine)
		{
			machine = GetComponent<Machine>();
		}
		else if (type == EnemyType.Drone)
		{
			drone = GetComponent<Drone>();
		}
	}

	public void DeliverDamage(GameObject target, Vector3 force, Vector3 hitPoint, float multiplier, bool tryForExplode, float critMultiplier)
	{
		if (weaknesses.Length > 0)
		{
			string[] array = weaknesses;
			foreach (string text in array)
			{
				if (hitter == text)
				{
					multiplier *= weaknessMultiplier;
				}
			}
		}
		switch (type)
		{
		case EnemyType.Zombie:
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GetHurt(target, force, multiplier, critMultiplier);
			if (tryForExplode && zombie.health <= 0f && !exploded)
			{
				Explode();
			}
			if (zombie.health <= 0f)
			{
				if (!dead && hitterWeapons.Count > 1)
				{
					GameObject.FindWithTag("Player").GetComponentInChildren<StyleHUD>().AddPoints(50, "<color=cyan>ARSENAL</color>");
				}
				dead = true;
				if (usingDoor != null)
				{
					usingDoor.Close();
					usingDoor = null;
				}
			}
			break;
		case EnemyType.Spider:
			if (spider == null)
			{
				spider = GetComponent<SpiderBody>();
			}
			if (hitter != "explosion" && hitter != "ffexplosion")
			{
				spider.GetHurt(target, force, hitPoint, multiplier);
			}
			if (spider.health <= 0f)
			{
				dead = true;
				if (usingDoor != null)
				{
					usingDoor.Close();
					usingDoor = null;
				}
			}
			break;
		case EnemyType.Machine:
			if (machine == null)
			{
				machine = GetComponent<Machine>();
			}
			machine.GetHurt(target, force, multiplier, critMultiplier);
			if (tryForExplode && machine.health <= 0f && !exploded)
			{
				Explode();
			}
			if (machine.health <= 0f)
			{
				dead = true;
				if (usingDoor != null)
				{
					usingDoor.Close();
					usingDoor = null;
				}
			}
			break;
		case EnemyType.Statue:
			if (statue == null)
			{
				statue = GetComponent<Statue>();
			}
			statue.GetHurt(target, force, multiplier, critMultiplier);
			if (tryForExplode && statue.health <= 0f && !exploded)
			{
				Explode();
			}
			if (statue.health <= 0f)
			{
				dead = true;
				if (usingDoor != null)
				{
					usingDoor.Close();
					usingDoor = null;
				}
			}
			break;
		case EnemyType.Wicked:
			if (wicked == null)
			{
				wicked = GetComponent<Wicked>();
			}
			wicked.GetHit();
			break;
		case EnemyType.Drone:
			if (drone == null)
			{
				drone = GetComponent<Drone>();
			}
			drone.GetHurt(force, multiplier);
			break;
		}
	}

	public void InstaKill()
	{
		if (type == EnemyType.Zombie && !dead)
		{
			dead = true;
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GoLimp();
			if (usingDoor != null)
			{
				usingDoor.Close();
				usingDoor = null;
			}
		}
	}

	public void Explode()
	{
		if (type != 0)
		{
			return;
		}
		if (zombie == null)
		{
			zombie = GetComponent<Zombie>();
		}
		if (exploded || zombie.chestExploding)
		{
			return;
		}
		exploded = true;
		if (zombie.chestExploding)
		{
			zombie.ChestExplodeEnd();
			Debug.Log("Premature Chest Explosion End #2");
		}
		Transform[] componentsInChildren = zombie.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.gameObject.tag == "Limb")
			{
				Object.Destroy(transform.GetComponent<CharacterJoint>());
				transform.transform.SetParent(transform.GetComponentInParent<GoreZone>().goreZone, true);
			}
			else if (transform.gameObject.tag == "Head" || transform.gameObject.tag == "EndLimb")
			{
				zombie.GetHurt(transform.gameObject, (base.transform.position - transform.position).normalized * 1000f, 2f, 1f);
			}
		}
		dead = true;
		if (usingDoor != null)
		{
			usingDoor.Close();
			usingDoor = null;
		}
	}
}
