using UnityEngine;

public class BigDoor : MonoBehaviour
{
	public bool open;

	public Vector3 openRotation;

	private Quaternion targetRotation;

	private Quaternion origRotation;

	public float speed;

	private CameraController cc;

	public bool screenShake;

	private AudioSource aud;

	public AudioClip openSound;

	public AudioClip closeSound;

	private float origPitch;

	public Light openLight;

	private void Start()
	{
		targetRotation.eulerAngles = base.transform.rotation.eulerAngles + openRotation;
		origRotation = base.transform.rotation;
		cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		aud = GetComponent<AudioSource>();
		origPitch = aud.pitch;
	}

	private void Update()
	{
		if (open && base.transform.rotation != targetRotation)
		{
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetRotation, Time.deltaTime * speed);
			if (screenShake)
			{
				cc.CameraShake(0.05f);
			}
			if (base.transform.rotation == targetRotation)
			{
				aud.clip = closeSound;
				aud.loop = false;
				aud.pitch = Random.Range(origPitch - 0.1f, origPitch + 0.1f);
				aud.Play();
			}
		}
		else
		{
			if (open || !(base.transform.rotation != origRotation))
			{
				return;
			}
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, origRotation, Time.deltaTime * speed);
			if (screenShake)
			{
				cc.CameraShake(0.05f);
			}
			if (base.transform.rotation == origRotation)
			{
				aud.clip = closeSound;
				aud.loop = false;
				aud.pitch = Random.Range(origPitch - 0.1f, origPitch + 0.1f);
				aud.Play();
				if (openLight != null)
				{
					openLight.enabled = false;
				}
			}
		}
	}

	public void Open()
	{
		open = true;
		aud.clip = openSound;
		aud.loop = true;
		aud.pitch = Random.Range(origPitch - 0.1f, origPitch + 0.1f);
		aud.Play();
	}

	public void Close()
	{
		open = false;
		aud.clip = openSound;
		aud.loop = true;
		aud.pitch = Random.Range(origPitch / 2f - 0.1f, origPitch / 2f + 0.1f);
		aud.Play();
	}
}
