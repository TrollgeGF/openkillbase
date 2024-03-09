using System.Collections.Generic;
using UnityEngine;

public class SwingCheck : MonoBehaviour
{
	private EnemyIdentifier eid;

	private NewMovement nmo;

	public EnemyType type;

	public bool playerOnly;

	private ZombieMelee zombie;

	private SwordsMachine swm;

	private StatueBoss sb;

	public int damage;

	public LayerMask lmask;

	private List<EnemyIdentifier> hitEnemies = new List<EnemyIdentifier>();

	public bool strong;

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
		type = eid.type;
		swm = GetComponentInParent<SwordsMachine>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (eid == null)
		{
			eid = GetComponentInParent<EnemyIdentifier>();
			if (eid != null)
			{
				type = eid.type;
			}
		}
		if (!Physics.Raycast(eid.transform.position, other.transform.position - eid.transform.position, Vector3.Distance(other.transform.position, eid.transform.position), lmask))
		{
			if (other.gameObject.tag == "Player")
			{
				if (type == EnemyType.Zombie)
				{
					if (zombie == null)
					{
						zombie = GetComponentInParent<ZombieMelee>();
					}
					if (zombie.coolDown == 0f)
					{
						zombie.Swing();
					}
					if (zombie.damaging && zombie.zmb.player.gameObject.layer != 15)
					{
						zombie.damaging = false;
						if (nmo == null)
						{
							nmo = zombie.zmb.player.GetComponent<NewMovement>();
						}
						nmo.GetHurt(30, true);
					}
				}
				if (type == EnemyType.Machine)
				{
					if (swm == null)
					{
						swm = GetComponentInParent<SwordsMachine>();
					}
					else if (swm.damaging && swm.player.gameObject.layer != 15)
					{
						swm.damaging = false;
						if (nmo == null)
						{
							nmo = swm.player.GetComponent<NewMovement>();
						}
						damage = swm.damage;
						if (damage > 0)
						{
							nmo.GetHurt(damage, true);
						}
					}
				}
				if (type == EnemyType.Statue)
				{
					if (sb == null)
					{
						sb = GetComponentInParent<StatueBoss>();
					}
					else if (sb.damaging && sb.player.gameObject.layer != 15)
					{
						sb.damaging = false;
						if (nmo == null)
						{
							nmo = sb.player.GetComponent<NewMovement>();
						}
						damage = sb.damage;
						if (damage > 0)
						{
							nmo.GetHurt(damage, true);
						}
						if (sb.launching)
						{
							nmo.Launch(other.transform.position + sb.transform.forward * -1f, 100f, 100f);
						}
					}
				}
			}
			else if (other.gameObject.layer == 10 && !playerOnly)
			{
				EnemyIdentifierIdentifier component = other.GetComponent<EnemyIdentifierIdentifier>();
				if (component != null && component.eid != null && component.eid.type != type)
				{
					if (type == EnemyType.Machine)
					{
						if (swm == null)
						{
							swm = GetComponentInParent<SwordsMachine>();
						}
						if (swm.damaging)
						{
							EnemyIdentifier enemyIdentifier = component.eid;
							if ((!hitEnemies.Contains(enemyIdentifier) || (enemyIdentifier.dead && other.gameObject.tag == "Head")) && (!enemyIdentifier.dead || (enemyIdentifier.dead && other.gameObject.tag != "Body")))
							{
								damage = swm.damage;
								enemyIdentifier.hitter = "enemy";
								enemyIdentifier.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 10000f, other.transform.position, damage / 10, false, 0f);
								hitEnemies.Add(enemyIdentifier);
							}
						}
						else if (hitEnemies.Count > 0)
						{
							hitEnemies.Clear();
						}
					}
					if (type == EnemyType.Zombie)
					{
						if (zombie == null)
						{
							zombie = GetComponentInParent<ZombieMelee>();
						}
						else
						{
							if (zombie.coolDown == 0f)
							{
								zombie.Swing();
							}
							if (zombie.damaging)
							{
								zombie.damaging = false;
								EnemyIdentifier enemyIdentifier2 = component.eid;
								enemyIdentifier2.hitter = "enemy";
								enemyIdentifier2.DeliverDamage(other.gameObject, (base.transform.position - other.transform.position).normalized * 10000f, other.transform.position, 3f, false, 0f);
							}
						}
					}
				}
			}
		}
		if (!(other.gameObject.tag == "Breakable"))
		{
			return;
		}
		Breakable component2 = other.gameObject.GetComponent<Breakable>();
		if (type == EnemyType.Machine)
		{
			if (swm == null)
			{
				swm = GetComponentInParent<SwordsMachine>();
			}
			if (swm.damaging && !component2.playerOnly && (component2.weak || strong))
			{
				component2.Break();
			}
			else if (!swm.damaging && !swm.inAction && !swm.inSemiAction && !component2.playerOnly && (component2.weak || strong))
			{
				swm.inAction = true;
				swm.RunningSwing();
			}
		}
		else if (type == EnemyType.Statue)
		{
			if (sb == null)
			{
				sb = GetComponentInParent<StatueBoss>();
			}
			if (sb != null && sb.damaging && !component2.playerOnly && (component2.weak || strong))
			{
				component2.Break();
			}
		}
		else if (type == EnemyType.Zombie)
		{
			if (zombie == null)
			{
				zombie = GetComponentInParent<ZombieMelee>();
			}
			if (zombie.damaging && !component2.playerOnly && (component2.weak || strong))
			{
				component2.Break();
			}
		}
	}
}
