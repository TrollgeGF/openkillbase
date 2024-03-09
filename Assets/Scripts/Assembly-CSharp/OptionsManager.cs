using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
	public bool mainMenu;

	public bool paused;

	private float originalTimeScale;

	public GameObject pauseMenu;

	public GameObject optionsMenu;

	public GameObject progressChecker;

	public GameObject difficultySelect;

	public GameObject levelSelect;

	private CameraController cc;

	private NewMovement nm;

	private GunControl gc;

	public AudioMixer allAudio;

	public float mouseSensitivity;

	public float simplifiedDistance;

	public bool simplifyEnemies;

	public Dropdown resolutionDropdown;

	private int screenWidth;

	private int screenHeight;

	public Toggle fullScreen;

	public float bloodstainChance;

	public float maxGore;

	public GameObject playerPosInfo;

	public bool dontUnpause;

	private void Awake()
	{
		if (GameObject.FindWithTag("OptionsManager") == null)
		{
			Object.Instantiate(progressChecker);
		}
	}

	private void Start()
	{
		resolutionDropdown.captionText.text = Screen.width + "x" + Screen.height;
		fullScreen.isOn = Screen.fullScreen;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			if (mainMenu)
			{
				if (optionsMenu.activeSelf && !dontUnpause)
				{
					CloseOptions();
				}
				else if (levelSelect.activeSelf)
				{
					CloseLevelSelect();
				}
				else if (difficultySelect.activeSelf)
				{
					CloseDifficultySelect();
				}
			}
			else if (!paused)
			{
				Pause();
			}
			else if (!dontUnpause)
			{
				CloseOptions();
				UnPause();
			}
		}
		if (mainMenu && !paused)
		{
			Pause();
		}
		if (paused)
		{
			if (mainMenu)
			{
				Time.timeScale = 1f;
			}
			else
			{
				Time.timeScale = 0f;
			}
		}
	}

	public void Pause()
	{
		if (nm == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
			Debug.Log(cc);
			Debug.Log(cc.pm);
			nm = cc.pm;
			gc = nm.GetComponentInChildren<GunControl>();
		}
		if (!mainMenu)
		{
			nm.enabled = false;
			allAudio.SetFloat("allPitch", 0f);
		}
		cc.enabled = false;
		gc.activated = false;
		originalTimeScale = Time.timeScale;
		paused = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		pauseMenu.SetActive(true);
	}

	public void UnPause()
	{
		CloseOptions();
		Cursor.lockState = CursorLockMode.Locked;
		paused = false;
		Time.timeScale = originalTimeScale;
		allAudio.SetFloat("allPitch", 1f);
		Cursor.visible = false;
		if (!nm.dead)
		{
			nm.enabled = true;
			cc.enabled = true;
			gc.activated = true;
		}
		pauseMenu.SetActive(false);
	}

	public void RestartCheckpoint()
	{
		UnPause();
		StatsManager component = GetComponent<StatsManager>();
		if (!component.infoSent)
		{
			component.Restart();
		}
	}

	public void RestartMission()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Object.Destroy(base.gameObject);
	}

	public void OpenOptions()
	{
		pauseMenu.SetActive(false);
		optionsMenu.SetActive(true);
	}

	public void CloseOptions()
	{
		pauseMenu.SetActive(true);
		optionsMenu.SetActive(false);
	}

	public void QuitMission()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("Main Menu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void OpenDifficultySelect()
	{
		difficultySelect.SetActive(true);
		pauseMenu.SetActive(false);
	}

	public void CloseDifficultySelect()
	{
		difficultySelect.SetActive(false);
		pauseMenu.SetActive(true);
	}

	public void OpenLevelSelect()
	{
		levelSelect.SetActive(true);
		difficultySelect.SetActive(false);
	}

	public void CloseLevelSelect()
	{
		levelSelect.SetActive(false);
		difficultySelect.SetActive(true);
	}

	public void ResolutionChange()
	{
		Debug.Log("test " + resolutionDropdown.value);
		if (resolutionDropdown.value == 0)
		{
			screenWidth = 640;
			screenHeight = 480;
		}
		else if (resolutionDropdown.value == 1)
		{
			screenWidth = 1280;
			screenHeight = 720;
		}
		else if (resolutionDropdown.value == 2)
		{
			screenWidth = 1920;
			screenHeight = 1080;
		}
		Screen.SetResolution(screenWidth, screenHeight, fullScreen.isOn);
	}

	public void ChangeLevel(string levelname)
	{
		PlayerPosInfo component = Object.Instantiate(playerPosInfo).GetComponent<PlayerPosInfo>();
		component.velocity = nm.GetComponent<Rigidbody>().velocity;
		component.wooshTime = nm.GetComponentInChildren<WallCheck>().GetComponent<AudioSource>().time;
		component.noPosition = true;
		SceneManager.LoadScene(levelname);
	}
}
