using UnityEngine;

public class ScriptActivator : MonoBehaviour
{
	public Piston[] pistons;

	public LightPillar[] lightpillars;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject.tag == "Player"))
		{
			return;
		}
		if (pistons.Length != 0)
		{
			Piston[] array = pistons;
			foreach (Piston piston in array)
			{
				piston.off = false;
			}
		}
		if (lightpillars.Length != 0)
		{
			LightPillar[] array2 = lightpillars;
			foreach (LightPillar lightPillar in array2)
			{
				lightPillar.ActivatePillar();
			}
		}
		Object.Destroy(this);
	}
}
