using System.Collections.Generic;
using UnityEngine;

public class EnemyScanner : MonoBehaviour
{
	private EnemyIdentifier owner;

	private EnemyType ownerType;

	private SwordsMachine sman;

	private Zombie zom;

	private Collider col;

	public bool constantUpdate;

	public List<GameObject> spottedEnemies = new List<GameObject>();

	private void Start()
	{
		owner = GetComponentInParent<EnemyIdentifier>();
		ownerType = owner.type;
		col = GetComponent<Collider>();
		if (ownerType == EnemyType.Machine)
		{
			sman = owner.GetComponent<SwordsMachine>();
		}
		else if (ownerType == EnemyType.Zombie)
		{
			zom = owner.GetComponent<Zombie>();
			if (!zom.friendly)
			{
				col.enabled = false;
			}
		}
	}

	private void Update()
	{
		if (ownerType == EnemyType.Zombie)
		{
			if (!zom.friendly)
			{
				col.enabled = false;
			}
			else
			{
				col.enabled = true;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (constantUpdate)
		{
			return;
		}
		if (owner == null)
		{
			owner = GetComponentInParent<EnemyIdentifier>();
		}
		if (!(other.gameObject.tag == "Enemy") || !(other.transform != owner.transform))
		{
			return;
		}
		EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
		if (component != null && component.type != ownerType && !component.dead && !component.ignoredByEnemies)
		{
			if (ownerType == EnemyType.Machine && sman != null && !sman.enemyTargets.Contains(component))
			{
				sman.enemyTargets.Add(component);
			}
			else if (ownerType == EnemyType.Zombie && zom != null && !zom.enemyTargets.Contains(component))
			{
				zom.enemyTargets.Add(component);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!constantUpdate)
		{
			return;
		}
		if (owner == null)
		{
			owner = GetComponentInParent<EnemyIdentifier>();
		}
		if (!(other.gameObject.tag == "Enemy") || !(other.transform != owner.transform) || spottedEnemies.Contains(other.gameObject))
		{
			return;
		}
		EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
		if (component != null && component.type != ownerType && !component.dead && !component.ignoredByEnemies)
		{
			if (ownerType == EnemyType.Machine && sman != null && !sman.enemyTargets.Contains(component))
			{
				sman.enemyTargets.Add(component);
			}
			else if (ownerType == EnemyType.Zombie && zom != null && !zom.enemyTargets.Contains(component))
			{
				zom.enemyTargets.Add(component);
			}
			spottedEnemies.Add(other.gameObject);
		}
	}
}
