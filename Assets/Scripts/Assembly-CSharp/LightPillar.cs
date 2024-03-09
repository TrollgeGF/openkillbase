using UnityEngine;

public class LightPillar : MonoBehaviour
{
	public bool activated;

	public bool completed;

	private Vector3 origScale;

	private Light[] lights;

	private float lightRange;

	public float speed;

	private AudioSource aud;

	private float origPitch;

	private float origVol;

	private void Start()
	{
		if (activated || completed)
		{
			return;
		}
		origScale = base.transform.localScale;
		lights = GetComponentsInChildren<Light>();
		aud = GetComponentInChildren<AudioSource>();
		origPitch = aud.pitch + Random.Range(-0.1f, 0.1f);
		aud.pitch = 0f;
		base.transform.localScale = new Vector3(0f, origScale.y, 0f);
		if (lights.Length > 0)
		{
			lightRange = lights[0].range;
			Light[] array = lights;
			foreach (Light light in array)
			{
				light.range = 0f;
			}
		}
	}

	private void Update()
	{
		if (!activated)
		{
			return;
		}
		if (base.transform.localScale != origScale)
		{
			base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, origScale, speed * Time.deltaTime);
		}
		if (lights[0].range != lightRange)
		{
			Light[] array = lights;
			foreach (Light light in array)
			{
				light.range = Mathf.MoveTowards(light.range, lightRange, speed * 4f * Time.deltaTime);
			}
		}
		if (aud.pitch != origPitch)
		{
			aud.pitch = Mathf.MoveTowards(aud.pitch, origPitch, speed / 3f * origPitch * Time.deltaTime);
		}
		else if (base.transform.localScale == origScale && lights[0].range == lightRange)
		{
			activated = false;
			completed = true;
		}
	}

	public void ActivatePillar()
	{
		activated = true;
	}
}
