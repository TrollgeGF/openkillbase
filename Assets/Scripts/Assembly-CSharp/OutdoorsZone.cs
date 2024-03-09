using UnityEngine;

public class OutdoorsZone : MonoBehaviour
{
	public Light[] sunLights;

	private LayerMask inside;

	public LayerMask outside;

	private void Start()
	{
		if (sunLights.Length > 0)
		{
			inside = sunLights[0].cullingMask;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player" && sunLights[0].cullingMask != (int)outside)
		{
			Light[] array = sunLights;
			foreach (Light light in array)
			{
				light.cullingMask = outside;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Light[] array = sunLights;
			foreach (Light light in array)
			{
				light.cullingMask = inside;
			}
		}
	}
}
