using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoEnd : MonoBehaviour
{
	private Image img;

	private bool fade;

	public GameObject credits;

	public int timeToFade;

	public float fadeTime;

	private void Start()
	{
		img = GetComponent<Image>();
		Invoke("StartFade", timeToFade);
	}

	private void Update()
	{
		if (fade)
		{
			img.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(img.color.a, 1f, Time.deltaTime * fadeTime));
		}
		if (img.color.a == 1f)
		{
			if (credits != null)
			{
				credits.SetActive(true);
				GameObject.FindWithTag("Player").GetComponent<NewMovement>().activated = false;
				GameObject.FindWithTag("Player").GetComponentInChildren<GunControl>().NoWeapon();
				GameObject.FindWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
				GameObject.FindWithTag("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
			else
			{
				SceneManager.LoadScene("Main Menu");
			}
		}
	}

	private void StartFade()
	{
		fade = true;
	}
}
