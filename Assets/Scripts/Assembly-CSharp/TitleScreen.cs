using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
	private bool sequenceStarted;

	public MusicManager mman;

	public GameObject[] hud;

	public GameObject titleScreen;

	private Text title;

	public GameObject worldRevolver;

	public GameObject revolver;

	public GameObject arenaActivator;

	private void OnTriggerEnter(Collider other)
	{
		if (!sequenceStarted && other.gameObject.tag == "Player")
		{
			sequenceStarted = true;
			RevolverPickedUp();
			GameObject[] array = hud;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			mman.StartMusic();
			mman.ArenaMusicStart();
			titleScreen.SetActive(true);
			title = titleScreen.GetComponent<Text>();
			title.resizeTextMaxSize = 1000;
			Invoke("HideTitle", 4.5f);
		}
	}

	private void Update()
	{
		if (sequenceStarted && titleScreen.activeSelf)
		{
			base.transform.parent.position += Vector3.down * Time.deltaTime * 10f;
		}
	}

	private void HideTitle()
	{
		titleScreen.SetActive(false);
		arenaActivator.SetActive(true);
	}

	private void RevolverPickedUp()
	{
		worldRevolver.SetActive(false);
		revolver.SetActive(true);
	}
}
