using UnityEngine;

public class StatueFake : MonoBehaviour
{
	private Animator anim;

	private AudioSource aud;

	private ParticleSystem part;

	public GameObject[] toDeactivate;

	public GameObject enemyObject;

	public bool spawn;

	public GameObject[] toActivate;

	public bool quickSpawn;

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();
		aud = GetComponentInChildren<AudioSource>();
		part = GetComponentInChildren<ParticleSystem>();
		StatueIntroChecker statueIntroChecker = Object.FindObjectOfType<StatueIntroChecker>();
		if (statueIntroChecker != null && statueIntroChecker.beenSeen)
		{
			quickSpawn = true;
		}
		else
		{
			statueIntroChecker.beenSeen = true;
		}
		if (quickSpawn)
		{
			anim.speed = 1.5f;
		}
	}

	public void Activate()
	{
		if (quickSpawn)
		{
			anim.Play("Awaken", -1, 0.33f);
		}
		else
		{
			Invoke("SlowStart", 3f);
		}
	}

	public void Crack()
	{
		aud.Play();
		part.Play();
	}

	public void Done()
	{
		GameObject[] array = toDeactivate;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		GameObject[] array2 = toActivate;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(true);
		}
		if (spawn)
		{
			Object.Instantiate(enemyObject, base.transform.position + base.transform.forward * 4f, base.transform.rotation);
		}
	}

	private void SlowStart()
	{
		anim.SetTrigger("Awaken");
	}
}
