using UnityEngine;

public class PlayerActivator : MonoBehaviour
{
	public GameObject[] toActivate;

	public float delay;

	private NewMovement nm;

	private bool activated;

	private int i;

	public bool startTimer;

	private GunControl gc;

	public GameObject impactDust;

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.tag == "Player")
		{
			nm = other.gameObject.GetComponentInParent<NewMovement>();
			gc = other.gameObject.GetComponentInChildren<GunControl>();
			if (!nm.activated)
			{
				nm.activated = true;
				nm.cc.activated = true;
				nm.cc.CameraShake(1f);
				AudioSource component = GetComponent<AudioSource>();
				component.Play();
			}
			activated = true;
			if (toActivate.Length != 0)
			{
				gc.YesWeapon();
				nm.EmptyStamina();
				ActivateObjects();
			}
			if (startTimer)
			{
				GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().StartTimer();
			}
		}
	}

	private void ActivateObjects()
	{
		toActivate[i].SetActive(true);
		i++;
		if (i == 4 && GameObject.FindWithTag("Player").GetComponentInChildren<GunControl>().noWeapons)
		{
			Debug.Log("No Weapons");
		}
		else if (i < toActivate.Length)
		{
			Invoke("ActivateObjects", delay);
		}
	}
}
