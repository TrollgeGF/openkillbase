using UnityEngine;
using UnityEngine.AI;
using cakeslice;

public class Machine : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	public GameObject bodyBlood;

	public GameObject limbBlood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject jawHalf;

	public GameObject brainChunk;

	public GameObject[] giblet;

	private GameObject player;

	public bool limp;

	private EnemyIdentifier eid;

	public GameObject chest;

	private float chestHP;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	public StyleCalculator scalc;

	private GoreZone gz;

	public Material deadMaterial;

	private Material originalMaterial;

	public SkinnedMeshRenderer smr;

	private Outline oline;

	private NavMeshAgent nma;

	private Rigidbody rb;

	private Rigidbody[] rbs;

	private Animator anim;

	public AudioClip deathSound;

	private bool noheal;

	public bool bigKill;

	public bool parryable;

	private SwordsMachine sm;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		oline = GetComponentInChildren<Outline>();
		if (oline != null)
		{
			oline.enabled = false;
		}
		rbs = GetComponentsInChildren<Rigidbody>();
		anim = GetComponent<Animator>();
		if (smr != null)
		{
			originalMaterial = smr.material;
		}
		if (spawnIn)
		{
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
			Object.Instantiate(spawnEffect, position, base.transform.rotation);
		}
		sm = GetComponent<SwordsMachine>();
	}

	public void GetHurt(GameObject target, Vector3 force, float multiplier, float critMultiplier)
	{
		string hitLimb = "";
		bool dead = false;
		bool flag = false;
		if (target.gameObject.tag == "Head")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			float num = 1f * multiplier + multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject = null;
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity) : Object.Instantiate(headBlood, target.transform.position, Quaternion.identity));
				Bloodsplatter component = gameObject.GetComponent<Bloodsplatter>();
				ParticleSystem component2 = component.GetComponent<ParticleSystem>();
				ParticleSystem.CollisionModule collision = component2.collision;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					if (Random.Range(0f, 1f) > 0.5f)
					{
						collision.enabled = false;
					}
					component.hpAmount = Mathf.RoundToInt((float)component.hpAmount * 0.25f);
				}
				if (!noheal)
				{
					component.GetReady();
				}
				component2.Play();
			}
			Vector3 normalized = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "head";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				float num2 = 1f;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					num2 = 0.5f;
				}
				if (target.transform.parent != null && target.transform.parent.GetComponentInParent<Rigidbody>() != null)
				{
					target.transform.parent.GetComponentInParent<Rigidbody>().AddForce(force);
				}
				for (int i = 0; (float)i < 6f * num2; i++)
				{
					Object.Instantiate(skullFragment, target.transform.position, Random.rotation);
				}
				for (int j = 0; (float)j < 4f * num2; j++)
				{
					Object.Instantiate(brainChunk, target.transform.position, Random.rotation);
				}
				for (int k = 0; (float)k < 2f * num2; k++)
				{
					Object.Instantiate(eyeBall, target.transform.position, Random.rotation);
					Object.Instantiate(jawHalf, target.transform.position, Random.rotation);
				}
			}
		}
		else if (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			float num = 1f * multiplier + 0.5f * multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject2 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject2 = Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject2 = Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity);
				}
				if (gameObject2 != null)
				{
					Bloodsplatter component3 = gameObject2.GetComponent<Bloodsplatter>();
					ParticleSystem component4 = component3.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision2 = component4.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision2.enabled = false;
						}
						component3.hpAmount = Mathf.RoundToInt((float)component3.hpAmount * 0.25f);
					}
					if (!noheal)
					{
						component3.GetReady();
					}
					component4.Play();
				}
			}
			if (!limp)
			{
				flag = true;
				hitLimb = "limb";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.gameObject.tag == "Limb")
				{
					float num3 = 1f;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						num3 = 0.5f;
					}
					for (int l = 0; (float)l < 4f * num3; l++)
					{
						Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
					}
				}
				else
				{
					target.transform.localScale = Vector3.zero;
				}
			}
		}
		else if (target.gameObject.tag == "Body")
		{
			float num = 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (eid.hitter == "shotgunzone")
			{
				if (!parryable && (target.gameObject != chest || health - num > 0f))
				{
					num = 0f;
				}
				else if (parryable && (target.gameObject == chest || player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
				{
					num *= 1.5f;
					GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
					if (sm != null)
					{
						if (!sm.enraged)
						{
							sm.Knockdown(true);
						}
						else
						{
							sm.Enrage();
						}
					}
				}
			}
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject3 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject3 = Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject3 = Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity);
				}
				if (gameObject3 != null)
				{
					Bloodsplatter component5 = gameObject3.GetComponent<Bloodsplatter>();
					ParticleSystem component6 = component5.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision3 = component6.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision3.enabled = false;
						}
						component5.hpAmount = Mathf.RoundToInt((float)component5.hpAmount * 0.25f);
					}
					if (!noheal)
					{
						component5.GetReady();
					}
					component6.Play();
				}
			}
			if (health <= 0f && target.gameObject == chest)
			{
				chestHP -= num;
				if (chestHP <= 0f || eid.hitter == "shotgunzone")
				{
					CharacterJoint[] componentsInChildren = target.GetComponentsInChildren<CharacterJoint>();
					if (componentsInChildren.Length > 0)
					{
						CharacterJoint[] array = componentsInChildren;
						foreach (CharacterJoint characterJoint in array)
						{
							if (characterJoint.transform.parent.parent == chest.transform)
							{
								Object.Destroy(characterJoint);
								characterJoint.transform.parent = null;
							}
						}
					}
					if (!limp)
					{
					}
					for (int n = 0; n < 2; n++)
					{
						Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
					}
					Object.Instantiate(headBlood, target.transform.position, Quaternion.identity);
					target.transform.localScale = Vector3.zero;
				}
			}
			Vector3 normalized2 = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "body";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.GetComponentInParent<Rigidbody>() != null)
				{
					target.GetComponentInParent<Rigidbody>().AddForce(force);
				}
			}
		}
		if (eid.hitter == "punch" && parryable)
		{
			float num = 5f;
			health -= num;
			GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
			if (sm != null)
			{
				if (!sm.enraged)
				{
					sm.Knockdown(true);
				}
				else
				{
					sm.Enrage();
				}
			}
		}
		if (health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "Head"))
		{
			if (target.transform.childCount > 0)
			{
				Transform child = target.transform.GetChild(0);
				CharacterJoint[] componentsInChildren2 = target.GetComponentsInChildren<CharacterJoint>();
				if (componentsInChildren2.Length > 0)
				{
					CharacterJoint[] array2 = componentsInChildren2;
					foreach (CharacterJoint obj in array2)
					{
						Object.Destroy(obj);
					}
				}
				CharacterJoint component7 = target.GetComponent<CharacterJoint>();
				if (component7 != null)
				{
					component7.connectedBody = null;
					Object.Destroy(component7);
				}
				target.transform.DetachChildren();
				target.transform.position = child.position;
				target.transform.SetParent(child);
				Object.Destroy(target.GetComponent<Rigidbody>());
			}
			Object.Destroy(target.GetComponent<Collider>());
			target.transform.localScale = Vector3.zero;
		}
		else if (health <= 0f && target.gameObject.tag == "EndLimb")
		{
			target.transform.localScale = Vector3.zero;
		}
		if (health > 0f && hurtSounds.Length > 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = 0.5f;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 12;
			aud.Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (!flag || !(eid.hitter != "enemy"))
		{
			return;
		}
		if (scalc == null)
		{
			scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		}
		if (health <= 0f)
		{
			dead = true;
		}
		if (eid.hitter != "secret")
		{
			if (bigKill)
			{
				scalc.HitCalculator(eid.hitter, "spider", hitLimb, dead, base.gameObject);
			}
			else
			{
				scalc.HitCalculator(eid.hitter, "machine", hitLimb, dead, base.gameObject);
			}
		}
	}

	public void GoLimp()
	{
		gz = GetComponentInParent<GoreZone>();
		if (sm == null)
		{
			sm = GetComponent<SwordsMachine>();
		}
		SwingCheck[] componentsInChildren = GetComponentsInChildren<SwingCheck>();
		if (sm != null)
		{
			anim.StopPlayback();
			SwingCheck[] array = componentsInChildren;
			foreach (SwingCheck obj in array)
			{
				Object.Destroy(obj);
			}
			sm.CoolSword();
			if (sm.currentEnrageEffect != null)
			{
				Object.Destroy(sm.currentEnrageEffect);
			}
			Object.Destroy(sm);
		}
		if (gz != null && gz.checkpoint != null)
		{
			gz.AddDeath();
			gz.checkpoint.sm.kills++;
		}
		else
		{
			StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
			component.kills++;
		}
		if (deadMaterial != null)
		{
			smr.sharedMaterial = deadMaterial;
		}
		else if (smr != null)
		{
			smr.sharedMaterial = originalMaterial;
		}
		if (oline != null)
		{
			oline.enabled = false;
		}
		Object.Destroy(nma);
		nma = null;
		Object.Destroy(anim);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		Object.Destroy(rb);
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		if (deathSound != null)
		{
			aud.clip = deathSound;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
		if (!limp)
		{
			rbs = GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array2 = rbs;
			foreach (Rigidbody rigidbody in array2)
			{
				if (rigidbody != null)
				{
					rigidbody.isKinematic = false;
					rigidbody.useGravity = true;
				}
			}
		}
		if (!limp)
		{
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
		}
		MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		componentInChildren.PlayCleanMusic();
		limp = true;
		EnemyScanner componentInChildren2 = GetComponentInChildren<EnemyScanner>();
		if (componentInChildren2 != null)
		{
			Object.Destroy(componentInChildren2.gameObject);
		}
	}
}
