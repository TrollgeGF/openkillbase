using UnityEngine;

public class test : MonoBehaviour
{
	private float FixedUpdatesPerFrame = 0f;

	private void FixedUpdate()
	{
		FixedUpdatesPerFrame += 1f;
	}

	private void Update()
	{
		Debug.Log("FixedUpdatesPerFrame: " + FixedUpdatesPerFrame);
		FixedUpdatesPerFrame = 0f;
	}
}
