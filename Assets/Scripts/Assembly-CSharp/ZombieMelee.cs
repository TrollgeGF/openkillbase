using UnityEngine;
using UnityEngine.AI;

public class ZombieMelee : MonoBehaviour
{
	public bool damaging;

	public TrailRenderer tr;

	public bool track;

	private AudioSource aud;

	public float coolDown;

	public Zombie zmb;

	private NavMeshAgent nma;

	private GameObject player;

	private Animator anim;

	private bool customStart;

	private bool musicRequested;

	private int difficulty;

	private float defaultCoolDown = 0.5f;

	public GameObject swingSound;

	public bool attacking;

	public LayerMask lmask;

	private void Start()
	{
		zmb = GetComponent<Zombie>();
		nma = zmb.nma;
		anim = zmb.anim;
		player = zmb.player;
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			defaultCoolDown = 0.25f;
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		int num = 40;
		if (difficulty > 2)
		{
			num = 80;
		}
		if (damaging)
		{
			base.transform.Translate(Vector3.forward * 80f * Time.deltaTime / 2.5f, Space.Self);
		}
		if (track && zmb.target != null)
		{
			if (player == null)
			{
				player = zmb.player;
			}
			base.transform.LookAt(new Vector3(zmb.target.position.x, base.transform.position.y, zmb.target.position.z));
		}
		if (coolDown != 0f)
		{
			if (coolDown - Time.deltaTime > 0f)
			{
				coolDown -= Time.deltaTime / 2.5f;
			}
			else
			{
				coolDown = 0f;
			}
		}
	}

	private void FixedUpdate()
	{
		if (anim == null)
		{
			anim = zmb.anim;
		}
		if (nma == null)
		{
			nma = zmb.nma;
		}
		if (zmb.grounded && nma != null && nma.enabled && zmb.target != null)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(zmb.target.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, float.PositiveInfinity, lmask))
			{
				nma.SetDestination(hitInfo.point);
			}
			else
			{
				nma.SetDestination(zmb.target.position);
			}
			if (!musicRequested && !zmb.friendly)
			{
				musicRequested = true;
				MusicManager componentInChildren = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
				componentInChildren.PlayBattleMusic();
			}
			if (nma.isStopped || nma.velocity == Vector3.zero)
			{
				anim.SetBool("Running", false);
			}
			else
			{
				anim.SetBool("Running", true);
			}
		}
		else if (nma == null)
		{
			nma = zmb.nma;
		}
	}

	public void Swing()
	{
		if (aud == null)
		{
			aud = GetComponentInChildren<SwingCheck>().GetComponent<AudioSource>();
		}
		if (nma == null)
		{
			nma = zmb.nma;
		}
		zmb.stopped = true;
		anim.speed = 1f;
		track = true;
		coolDown = defaultCoolDown;
		if (nma.enabled)
		{
			nma.isStopped = true;
		}
		anim.SetTrigger("Swing");
		Object.Instantiate(swingSound, base.transform);
	}

	public void SwingEnd()
	{
		nma.isStopped = false;
		tr.enabled = false;
		zmb.stopped = false;
	}

	public void DamageStart()
	{
		damaging = true;
		aud.Play();
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = true;
	}

	public void DamageEnd()
	{
		damaging = false;
		zmb.attacking = false;
	}

	public void StopTracking()
	{
		track = false;
		zmb.attacking = true;
	}

	public void CancelAttack()
	{
		damaging = false;
		zmb.attacking = false;
		if (tr == null)
		{
			tr = GetComponentInChildren<TrailRenderer>();
		}
		tr.enabled = false;
		tr.enabled = false;
		zmb.stopped = false;
		track = false;
		coolDown = defaultCoolDown;
	}
}
