using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Glass : MonoBehaviour
{
	public bool broken;

	public bool wall;

	private Transform[] glasses;

	public GameObject shatterParticle;

	public AudioClip scream;

	private StyleHUD shud;

	private int kills;

	private bool playerOn;

	private Collider[] cols;

	private List<GameObject> droppedEnemies = new List<GameObject>();

	public void Shatter()
	{
		cols = GetComponentsInChildren<Collider>();
		if (playerOn)
		{
			if (!wall)
			{
				GameObject.FindWithTag("Player").GetComponentInChildren<GroundCheck>().onGround = false;
			}
			else
			{
				WallCheck componentInChildren = GameObject.FindWithTag("Player").GetComponentInChildren<WallCheck>();
				Collider[] array = cols;
				foreach (Collider item in array)
				{
					if (componentInChildren.cols.Contains(item))
					{
						componentInChildren.cols.Remove(item);
					}
				}
			}
		}
		glasses = base.transform.GetComponentsInChildren<Transform>();
		Transform[] array2 = glasses;
		foreach (Transform transform in array2)
		{
			if (transform.gameObject != base.gameObject)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		Collider[] array3 = cols;
		foreach (Collider collider in array3)
		{
			if (!collider.isTrigger)
			{
				collider.enabled = false;
			}
		}
		base.gameObject.layer = 17;
		broken = true;
		Invoke("BecomeObstacle", 0.5f);
		Object.Instantiate(shatterParticle, base.transform);
	}

	private void OnTriggerStay(Collider other)
	{
		if (broken && !wall && other.gameObject.tag == "Enemy" && !droppedEnemies.Contains(other.gameObject))
		{
			droppedEnemies.Add(other.gameObject);
			EnemyIdentifier component = other.GetComponent<EnemyIdentifier>();
			if (component.type == EnemyType.Zombie)
			{
				component.GetComponentInChildren<GroundCheck>().onGround = false;
			}
			AudioSource componentInChildren = other.transform.GetChild(1).GetComponentInChildren<AudioSource>();
			componentInChildren.clip = scream;
			componentInChildren.volume = 1f;
			componentInChildren.priority = 78;
			componentInChildren.pitch = Random.Range(0.8f, 1.2f);
			componentInChildren.Play();
			kills++;
		}
		if (!broken && other.gameObject.tag == "Player")
		{
			playerOn = true;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if (!broken && other.gameObject.tag == "Player")
		{
			playerOn = true;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (!broken && other.gameObject.tag == "Player")
		{
			playerOn = false;
		}
	}

	private void BecomeObstacle()
	{
		NavMeshObstacle component = GetComponent<NavMeshObstacle>();
		if (wall)
		{
			component.carving = false;
			component.enabled = false;
		}
		else
		{
			component.enabled = true;
			Collider[] array = cols;
			foreach (Collider collider in array)
			{
				if (!collider.isTrigger)
				{
					collider.enabled = false;
				}
			}
		}
		if (kills >= 3)
		{
			StatsManager component2 = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			if (component2.maxGlassKills < kills)
			{
				component2.maxGlassKills = kills;
			}
		}
	}
}