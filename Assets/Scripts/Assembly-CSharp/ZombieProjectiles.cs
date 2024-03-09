using UnityEngine;
using UnityEngine.AI;

public class ZombieProjectiles : MonoBehaviour
{
	public bool stationary;

	public bool smallRay;

	public bool wanderer;

	public bool afraid;

	private Zombie zmb;

	private GameObject player;

	private GameObject camObj;

	private NavMeshAgent nma;

	private NavMeshHit hit;

	private Animator anim;

	public Vector3 targetPosition;

	public float coolDown;

	private AudioSource aud;

	public TrailRenderer tr;

	public GameObject projectile;

	private GameObject currentProjectile;

	public Transform shootPos;

	public GameObject head;

	public bool playerSpotted;

	private RaycastHit rhit;

	private RaycastHit bhit;

	public LayerMask lookForPlayerMask;

	public bool seekingPlayer = true;

	private float raySize = 1f;

	private bool musicRequested;

	public GameObject decProjectileSpawner;

	public GameObject decProjectile;

	private GameObject currentDecProjectile;

	public bool swinging;

	private int difficulty;

	private float coolDownReduce = 0f;

	private EnemyIdentifier eid;

	private GameObject origWP;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		player = GameObject.FindWithTag("Player");
		camObj = GameObject.FindWithTag("MainCamera");
		nma = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		if (stationary || smallRay)
		{
			raySize = 0.25f;
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			coolDownReduce = 1f;
		}
		eid = GetComponent<EnemyIdentifier>();
		origWP = eid.weakPoint;
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (!nma.enabled || zmb.limp || !(zmb.target != null))
		{
			return;
		}
		if (coolDown > 0f)
		{
			coolDown = Mathf.MoveTowards(coolDown, 0f, Time.deltaTime);
		}
		Vector3 normalized = (zmb.target.position - head.transform.position).normalized;
		if (playerSpotted && !Physics.Raycast(head.transform.position, normalized, out bhit, Vector3.Distance(zmb.target.position, head.transform.position), lookForPlayerMask) && (Vector3.Distance(base.transform.position, zmb.target.position) < 30f || stationary))
		{
			seekingPlayer = false;
			if (!wanderer)
			{
				nma.SetDestination(base.transform.position);
			}
			else if (wanderer && coolDown <= 0f)
			{
				nma.SetDestination(base.transform.position);
			}
			if (coolDown <= 0f && nma.velocity == Vector3.zero)
			{
				Swing();
			}
		}
		else if (!playerSpotted && !Physics.Raycast(head.transform.position, normalized, out rhit, Vector3.Distance(zmb.target.position, head.transform.position), lookForPlayerMask))
		{
			seekingPlayer = false;
			playerSpotted = true;
			coolDown = (float)Random.Range(1, 2) - coolDownReduce / 2f;
			if (zmb.target == zmb.player.transform && !musicRequested)
			{
				musicRequested = true;
				MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
				componentInChildren.PlayBattleMusic();
			}
		}
		else if (!stationary)
		{
			seekingPlayer = true;
			nma.updateRotation = true;
			nma.SetDestination(zmb.target.position);
		}
		Vector3 vector = zmb.target.position - base.transform.position;
		if (afraid && !swinging && Vector3.Distance(zmb.target.position, base.transform.position) < 15f)
		{
			seekingPlayer = true;
			nma.updateRotation = true;
			targetPosition = new Vector3(base.transform.position.x + vector.normalized.x * -10f, base.transform.position.y, base.transform.position.z + vector.normalized.z * -10f);
			if (NavMesh.SamplePosition(targetPosition, out hit, 1f, nma.areaMask))
			{
				nma.SetDestination(targetPosition);
			}
			else
			{
				NavMesh.FindClosestEdge(targetPosition, out hit, nma.areaMask);
				targetPosition = hit.position;
				nma.SetDestination(targetPosition);
			}
		}
		if (nma.velocity == Vector3.zero && playerSpotted && !seekingPlayer && (!wanderer || !swinging))
		{
			anim.SetBool("Running", false);
			nma.updateRotation = false;
			base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		}
		else if (nma.velocity != Vector3.zero)
		{
			anim.SetBool("Running", true);
		}
		else
		{
			if (!(nma.velocity == Vector3.zero) || !playerSpotted || seekingPlayer || !wanderer || !swinging)
			{
				return;
			}
			anim.SetBool("Running", false);
			nma.updateRotation = false;
			if (difficulty >= 2)
			{
				Vector3 vector2 = new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z);
				Vector3 normalized2 = (vector2 - base.transform.position).normalized;
				Quaternion b = Quaternion.LookRotation(normalized2);
				if (difficulty == 2)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 3.5f);
				}
				else if (difficulty == 3)
				{
					base.transform.LookAt(vector2);
				}
				else if (difficulty > 3)
				{
					base.transform.LookAt(vector2);
				}
			}
		}
	}

	public void Swing()
	{
		swinging = true;
		seekingPlayer = false;
		nma.updateRotation = false;
		base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		if (nma.enabled)
		{
			nma.isStopped = true;
		}
		if (zmb.target.position.y - 5f > base.transform.position.y || zmb.target.position.y + 5f < base.transform.position.y)
		{
			anim.SetFloat("AttackType", 1f);
		}
		else
		{
			anim.SetFloat("AttackType", Random.Range(0, 2));
		}
		anim.SetTrigger("Swing");
		coolDown = 99f;
	}

	public void SwingEnd()
	{
		swinging = false;
		if (nma.enabled)
		{
			nma.isStopped = false;
		}
		coolDown = Random.Range(1f, 2.5f) - coolDownReduce;
		if (wanderer)
		{
			Vector3 sourcePosition = Random.onUnitSphere * 10f + base.transform.position;
			NavMeshHit navMeshHit;
			NavMesh.SamplePosition(sourcePosition, out navMeshHit, 10f, 1);
			nma.SetDestination(navMeshHit.position);
			coolDown = Random.Range(0f, 2f) - coolDownReduce;
		}
	}

	public void SpawnProjectile()
	{
		currentDecProjectile = Object.Instantiate(decProjectile, decProjectileSpawner.transform);
		eid.weakPoint = currentDecProjectile;
	}

	public void DamageStart()
	{
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = true;
		zmb.attacking = true;
	}

	public void ThrowProjectile()
	{
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		currentProjectile = Object.Instantiate(projectile, shootPos.position, base.transform.rotation);
		if (zmb.target == player.transform)
		{
			currentProjectile.transform.LookAt(camObj.transform);
		}
		else
		{
			currentProjectile.transform.LookAt(zmb.target.GetComponentInChildren<EnemyIdentifierIdentifier>().transform);
		}
		currentProjectile.GetComponent<Projectile>().safeEnemyType = EnemyType.Zombie;
		if (difficulty > 2)
		{
			currentProjectile.GetComponent<Projectile>().speed *= 1.35f;
		}
	}

	public void ShootProjectile()
	{
		swinging = true;
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		currentProjectile = Object.Instantiate(projectile, decProjectileSpawner.transform.position, decProjectileSpawner.transform.rotation);
		currentProjectile.GetComponent<Projectile>().safeEnemyType = EnemyType.Zombie;
		if (difficulty > 2)
		{
			currentProjectile.GetComponent<Projectile>().speed *= 1.25f;
		}
	}

	public void StopTracking()
	{
	}

	public void DamageEnd()
	{
		if (tr != null)
		{
			tr.enabled = false;
		}
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		zmb.attacking = false;
	}

	public void CancelAttack()
	{
		Debug.Log("Cancelled Attack");
		Debug.Log("Cancelled Attack");
		swinging = false;
		coolDown = 0f;
		if (currentDecProjectile != null)
		{
			Object.Destroy(currentDecProjectile);
			eid.weakPoint = origWP;
		}
		if (tr != null)
		{
			tr.enabled = false;
		}
		zmb.attacking = false;
	}
}
