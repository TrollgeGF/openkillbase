using UnityEngine;

public class EnemyShotgun : MonoBehaviour
{
	private AudioSource gunAud;

	public AudioClip shootSound;

	public AudioClip clickSound;

	public AudioClip smackSound;

	public int variation;

	public GameObject bullet;

	public GameObject grenade;

	public float spread;

	private Animator anim;

	public bool gunReady = true;

	public Transform[] shootPoints;

	public GameObject muzzleFlash;

	private ParticleSystem[] parts;

	private void Start()
	{
		gunAud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		parts = GetComponentsInChildren<ParticleSystem>();
	}

	private void Update()
	{
	}

	public void Shoot()
	{
		gunReady = false;
		int num = 12;
		anim.SetTrigger("Shoot");
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(bullet, shootPoints[0].transform.position, shootPoints[0].transform.rotation);
			gameObject.transform.Rotate(Random.Range(0f - spread, spread), Random.Range(0f - spread, spread), Random.Range(0f - spread, spread));
		}
		gunAud.clip = shootSound;
		gunAud.volume = 0.35f;
		gunAud.panStereo = 0f;
		gunAud.pitch = Random.Range(0.95f, 1.05f);
		gunAud.Play();
		Transform[] array = shootPoints;
		foreach (Transform transform in array)
		{
			Object.Instantiate(muzzleFlash, transform.transform.position, transform.transform.rotation);
		}
	}

	private void ShootSinks()
	{
		gunReady = false;
		Transform[] array = shootPoints;
		foreach (Transform transform in array)
		{
			GameObject gameObject = Object.Instantiate(grenade, shootPoints[0].transform.position, Random.rotation);
			gameObject.GetComponent<Rigidbody>().AddForce(shootPoints[0].transform.forward * 70f, ForceMode.VelocityChange);
		}
		anim.SetTrigger("Secondary Fire");
		gunAud.clip = shootSound;
		gunAud.volume = 0.35f;
		gunAud.panStereo = 0f;
		gunAud.pitch = Random.Range(0.75f, 0.85f);
		gunAud.Play();
		Transform[] array2 = shootPoints;
		foreach (Transform transform2 in array2)
		{
			Object.Instantiate(muzzleFlash, transform2.transform.position, transform2.transform.rotation);
		}
	}

	public void ReleaseHeat()
	{
		ParticleSystem[] array = parts;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Play();
		}
	}

	public void ClickSound()
	{
		gunAud.clip = clickSound;
		gunAud.volume = 0.5f;
		gunAud.pitch = Random.Range(0.95f, 1.05f);
		gunAud.panStereo = 0.1f;
		gunAud.Play();
	}

	public void ReadyGun()
	{
		gunReady = true;
	}

	public void Smack()
	{
		gunAud.clip = smackSound;
		gunAud.volume = 0.75f;
		gunAud.pitch = Random.Range(2f, 2.2f);
		gunAud.panStereo = 0.1f;
		gunAud.Play();
	}
}
