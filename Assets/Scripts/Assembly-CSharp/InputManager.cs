using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public Dictionary<string, KeyCode> inputs = new Dictionary<string, KeyCode>();

	public bool ScrOn;

	public bool ScrWep;

	public bool ScrVar;

	public bool ScrRev;

	private void Awake()
	{
		if (PlayerPrefs.GetInt("ScrOn", 1) == 1)
		{
			ScrOn = true;
		}
		else
		{
			ScrOn = false;
		}
		if (PlayerPrefs.GetInt("ScrWep", 1) == 1)
		{
			ScrWep = true;
		}
		else
		{
			ScrWep = false;
		}
		if (PlayerPrefs.GetInt("ScrVar", 0) == 1)
		{
			ScrVar = true;
		}
		else
		{
			ScrVar = false;
		}
		if (PlayerPrefs.GetInt("ScrRev", 0) == 1)
		{
			ScrRev = true;
		}
		else
		{
			ScrRev = false;
		}
		CheckInputs();
	}

	public void CheckInputs()
	{
		inputs.Add("W", (KeyCode)PlayerPrefs.GetInt("KeyW", 119));
		inputs.Add("S", (KeyCode)PlayerPrefs.GetInt("KeyS", 115));
		inputs.Add("A", (KeyCode)PlayerPrefs.GetInt("KeyA", 97));
		inputs.Add("D", (KeyCode)PlayerPrefs.GetInt("KeyD", 100));
		inputs.Add("Jump", (KeyCode)PlayerPrefs.GetInt("KeyJump", 32));
		inputs.Add("Dodge", (KeyCode)PlayerPrefs.GetInt("KeyDodge", 304));
		inputs.Add("Slide", (KeyCode)PlayerPrefs.GetInt("KeySlide", 306));
		inputs.Add("Fire1", (KeyCode)PlayerPrefs.GetInt("KeyFire1", 323));
		inputs.Add("Fire2", (KeyCode)PlayerPrefs.GetInt("KeyFire2", 324));
		inputs.Add("Punch", (KeyCode)PlayerPrefs.GetInt("KeyPunch", 102));
		inputs.Add("LastUsedWeapon", (KeyCode)PlayerPrefs.GetInt("KeyLastUsedWeapon", 113));
		inputs.Add("ChangeVariation", (KeyCode)PlayerPrefs.GetInt("KeyChangeVariation", 101));
	}
}
