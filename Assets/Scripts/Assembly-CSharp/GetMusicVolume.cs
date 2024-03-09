using System.Collections.Generic;
using UnityEngine;

public class GetMusicVolume : MonoBehaviour
{
	private MusicManager mman;

	private AudioSource[] auds;

	private List<float> origVol = new List<float>();

	private void Start()
	{
		mman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		auds = GetComponentsInChildren<AudioSource>();
		AudioSource[] array = auds;
		foreach (AudioSource audioSource in array)
		{
			origVol.Add(audioSource.volume);
		}
	}

	private void Update()
	{
		for (int i = 0; i < auds.Length; i++)
		{
			auds[i].volume = origVol[i] * mman.volume;
		}
	}
}
