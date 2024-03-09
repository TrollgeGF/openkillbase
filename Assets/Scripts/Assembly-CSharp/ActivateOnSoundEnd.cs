using UnityEngine;

public class ActivateOnSoundEnd : MonoBehaviour
{
	private AudioSource aud;

	private bool hasStarted;

	public GameObject[] toActivate;

	public GameObject[] toDisactivate;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (aud.isPlaying)
		{
			hasStarted = true;
		}
		if (hasStarted && !aud.isPlaying)
		{
			hasStarted = false;
			GameObject[] array = toActivate;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			GameObject[] array2 = toDisactivate;
			foreach (GameObject gameObject2 in array2)
			{
				gameObject2.SetActive(false);
			}
		}
	}
}
