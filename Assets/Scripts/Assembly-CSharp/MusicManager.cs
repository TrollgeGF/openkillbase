using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public bool off;

	public AudioSource battleTheme;

	public AudioSource cleanTheme;

	public AudioSource bossTheme;

	public AudioSource targetTheme;

	private AudioSource[] allThemes;

	public float volume;

	public float requestedThemes;

	private bool arenaMode;

	private float defaultVolume;

	private void Start()
	{
		allThemes = GetComponentsInChildren<AudioSource>();
		defaultVolume = volume;
		if (!off)
		{
			AudioSource[] array = allThemes;
			foreach (AudioSource audioSource in array)
			{
				audioSource.Play();
			}
			cleanTheme.volume = volume;
			targetTheme = cleanTheme;
		}
		else
		{
			targetTheme = GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (!off && targetTheme.volume != volume)
		{
			AudioSource[] array = allThemes;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource == targetTheme)
				{
					if (audioSource.volume > volume)
					{
						audioSource.volume = volume;
					}
					audioSource.volume = Mathf.MoveTowards(audioSource.volume, volume, Time.deltaTime);
				}
				else
				{
					audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, Time.deltaTime);
				}
			}
			if (targetTheme.volume == volume)
			{
				AudioSource[] array2 = allThemes;
				foreach (AudioSource audioSource2 in array2)
				{
					if (audioSource2 != targetTheme)
					{
						audioSource2.volume = 0f;
					}
				}
			}
		}
		if (volume != 0f && (!off || !(targetTheme.volume > 0f)))
		{
			return;
		}
		AudioSource[] array3 = allThemes;
		for (int k = 0; k < array3.Length; k++)
		{
			array3[k].volume -= Time.deltaTime / 5f;
		}
		if (targetTheme.volume <= 0f)
		{
			AudioSource[] array4 = allThemes;
			foreach (AudioSource audioSource3 in array4)
			{
				audioSource3.volume = 0f;
			}
		}
	}

	public void StartMusic()
	{
		off = false;
		AudioSource[] array = allThemes;
		foreach (AudioSource audioSource in array)
		{
			audioSource.Play();
		}
		cleanTheme.volume = volume;
		targetTheme = cleanTheme;
	}

	public void PlayBattleMusic()
	{
		if (targetTheme != battleTheme)
		{
			battleTheme.time = cleanTheme.time;
		}
		if (targetTheme != bossTheme)
		{
			targetTheme = battleTheme;
		}
		requestedThemes += 1f;
	}

	public void PlayCleanMusic()
	{
		requestedThemes -= 1f;
		if (requestedThemes <= 0f && !arenaMode)
		{
			requestedThemes = 0f;
			targetTheme = cleanTheme;
		}
	}

	public void PlayBossMusic()
	{
		Debug.Log("PlayBossMusic");
		if (targetTheme != bossTheme)
		{
			bossTheme.time = cleanTheme.time;
		}
		targetTheme = bossTheme;
	}

	public void ArenaMusicStart()
	{
		if (off)
		{
			off = false;
		}
		if (!battleTheme.isPlaying)
		{
			AudioSource[] array = allThemes;
			foreach (AudioSource audioSource in array)
			{
				audioSource.Play();
			}
			battleTheme.volume = volume;
		}
		if (targetTheme != bossTheme)
		{
			targetTheme = battleTheme;
		}
		arenaMode = true;
	}

	public void ArenaMusicEnd()
	{
		requestedThemes = 0f;
		targetTheme = cleanTheme;
		arenaMode = false;
	}
}
