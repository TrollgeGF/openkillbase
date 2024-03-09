using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CameraController : MonoBehaviour
{
	public bool invert;

	public float minimumX = -89f;

	public float maximumX = 89f;

	public float minimumY = -360f;

	public float maximumY = 360f;

	public OptionsManager opm;

	public float sensitivityX;

	public float sensitivityY;

	public float scroll;

	public Vector3 originalPos;

	public Vector3 defaultPos;

	private Vector3 targetPos;

	public GameObject player;

	public NewMovement pm;

	private Camera cam;

	public bool activated = false;

	public GameObject gun;

	public float rotationY;

	public float rotationX;

	public bool reverseX;

	public bool reverseY;

	public float cameraShaking;

	public float movementHor;

	public float movementVer;

	public int dodgeDirection;

	public float additionalRotationY;

	public float additionalRotationX;

	public float defaultFov;

	public AudioMixer[] audmix;

	private bool mouseUnlocked;

	public bool slide;

	private float slowDown = 1f;

	private Camera virtualCamera;

	private Camera hudCamera;

	public RenderTexture mainTargetMaterial;

	public RenderTexture ultraWideTargetMaterial;

	public RenderTexture hudTargetMaterial;

	public RenderTexture ultraWideHudTargetMaterial;

	private GameObject standardhud;

	private float aspectRatio;

	private bool pixeled;

	private bool tilt;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		pm = player.GetComponent<NewMovement>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Debug.Log(Cursor.lockState);
		cam = GetComponent<Camera>();
		Time.timeScale = 1f;
		Application.targetFrameRate = Screen.currentResolution.refreshRate * 2;
		gun = base.transform.GetChild(0).gameObject;
		originalPos = base.transform.localPosition;
		defaultPos = base.transform.localPosition;
		targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.2f, defaultPos.z);
		cam.fieldOfView = PlayerPrefs.GetFloat("FOV", 105f);
		defaultFov = cam.fieldOfView;
		opm = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
		virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
		hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
		standardhud = GetComponentInChildren<HudController>().gameObject;
		CheckPixelization();
		CheckTilt();
	}

	private void Update()
	{
		if ((!pixeled && cam.aspect != aspectRatio) || (pixeled && virtualCamera.aspect != aspectRatio))
		{
			CheckAspectRatio();
		}
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		scroll = Input.GetAxis("Mouse ScrollWheel");
		if (activated)
		{
			if (!reverseY)
			{
				rotationX += Input.GetAxis("Mouse Y") * (opm.mouseSensitivity / 10f);
			}
			else
			{
				rotationX -= Input.GetAxis("Mouse Y") * (opm.mouseSensitivity / 10f);
			}
			if (!reverseX)
			{
				rotationY += Input.GetAxis("Mouse X") * (opm.mouseSensitivity / 10f);
			}
			else
			{
				rotationY -= Input.GetAxis("Mouse X") * (opm.mouseSensitivity / 10f);
			}
		}
		if (rotationY > 180f)
		{
			rotationY -= 360f;
		}
		else if (rotationY < -180f)
		{
			rotationY += 360f;
		}
		rotationX = Mathf.Clamp(rotationX, minimumX + additionalRotationX, maximumX + additionalRotationX);
		if (pm.boost)
		{
			if (dodgeDirection == 0)
			{
				cam.fieldOfView = defaultFov - defaultFov / 20f;
			}
			else if (dodgeDirection == 1)
			{
				cam.fieldOfView = defaultFov + defaultFov / 10f;
			}
		}
		else if (cam.fieldOfView > defaultFov)
		{
			cam.fieldOfView -= Time.deltaTime * 100f;
		}
		else if (cam.fieldOfView < defaultFov)
		{
			cam.fieldOfView = defaultFov;
		}
		player.transform.localEulerAngles = new Vector3(0f, rotationY + additionalRotationY, 0f);
		if (tilt)
		{
			if (!pm.boost)
			{
				base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, movementHor * -1f);
			}
			else
			{
				base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, movementHor * -5f);
			}
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
		if (cameraShaking > 0f)
		{
			if (cameraShaking > 1f)
			{
				base.transform.localPosition = new Vector3(defaultPos.x + (float)Random.Range(-1, 2), defaultPos.y + (float)Random.Range(-1, 2), defaultPos.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(defaultPos.x + cameraShaking * Random.Range(-1f, 1f), defaultPos.y + cameraShaking * Random.Range(-1f, 1f), defaultPos.z);
			}
			cameraShaking -= Time.deltaTime * 3f;
		}
		else if (pm.walking)
		{
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPos, Time.deltaTime * 0.5f);
			if (base.transform.localPosition == targetPos && targetPos != defaultPos)
			{
				targetPos = defaultPos;
			}
			else if (base.transform.localPosition == targetPos && targetPos == defaultPos)
			{
				targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
			}
		}
		else
		{
			base.transform.localPosition = defaultPos;
			targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
		}
	}

	private void FixedUpdate()
	{
		if (slowDown < 1f)
		{
			slowDown = Mathf.MoveTowards(slowDown, 1f, 0.02f);
			Time.timeScale = slowDown;
			AudioMixer[] array = audmix;
			foreach (AudioMixer audioMixer in array)
			{
				audioMixer.SetFloat("allPitch", slowDown);
			}
		}
	}

	public void CameraShake(float shakeAmount)
	{
		float num = PlayerPrefs.GetFloat("ScrSha", 100f) / 100f;
		if (num != 0f && cameraShaking < shakeAmount * num)
		{
			cameraShaking = shakeAmount * num;
		}
	}

	public void StopShake()
	{
		cameraShaking = 0f;
	}

	public void HitStop(float length)
	{
		Time.timeScale = 0f;
		StartCoroutine(TimeIsStopped(length, false));
	}

	public void TrueStop(float length)
	{
		AudioMixer[] array = audmix;
		foreach (AudioMixer audioMixer in array)
		{
			audioMixer.SetFloat("allPitch", 0f);
		}
		Time.timeScale = 0f;
		StartCoroutine(TimeIsStopped(length, true));
	}

	private IEnumerator TimeIsStopped(float length, bool trueStop)
	{
		yield return new WaitForSecondsRealtime(length);
		ContinueTime(trueStop);
	}

	private void ContinueTime(bool trueStop)
	{
		Time.timeScale = slowDown;
		if (trueStop)
		{
			AudioMixer[] array = audmix;
			foreach (AudioMixer audioMixer in array)
			{
				audioMixer.SetFloat("allPitch", 1f);
			}
		}
	}

	public void ResetCamera(float degrees, bool instant)
	{
		if (instant)
		{
			rotationY = degrees;
			rotationX = 0f;
		}
	}

	public void ResetToDefaultPos()
	{
		base.transform.localPosition = defaultPos;
		targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
	}

	public void SlowDown(float amount)
	{
		if (amount <= 0f)
		{
			amount = 0.01f;
		}
		slowDown = amount;
	}

	public void CheckPixelization()
	{
		if (virtualCamera == null)
		{
			cam = GetComponent<Camera>();
			virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
			hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
		}
		if (PlayerPrefs.GetInt("Pix", 1) == 0)
		{
			pixeled = false;
			virtualCamera.enabled = false;
			cam.targetTexture = null;
			cam.allowHDR = true;
			hudCamera.targetTexture = null;
			hudCamera.clearFlags = CameraClearFlags.Depth;
			if (PlayerPrefs.GetInt("ColCom", 2) != 0)
			{
				cam.GetComponent<cPrecision>().enabled = true;
				hudCamera.GetComponent<cPrecision>().enabled = true;
			}
			virtualCamera.GetComponent<cPrecision>().enabled = false;
			return;
		}
		pixeled = true;
		virtualCamera.enabled = true;
		if (aspectRatio > 1.78f)
		{
			cam.targetTexture = ultraWideTargetMaterial;
			hudCamera.targetTexture = ultraWideHudTargetMaterial;
		}
		else
		{
			cam.targetTexture = mainTargetMaterial;
			hudCamera.targetTexture = hudTargetMaterial;
		}
		hudCamera.clearFlags = CameraClearFlags.Color;
		cam.GetComponent<cPrecision>().enabled = false;
		cam.allowHDR = false;
		hudCamera.GetComponent<cPrecision>().enabled = false;
		if (PlayerPrefs.GetInt("ColCom", 2) != 0)
		{
			virtualCamera.GetComponent<cPrecision>().enabled = true;
		}
	}

	public void CheckColorCompression()
	{
		int @int = PlayerPrefs.GetInt("ColCom", 2);
		int colorPrecision = 32;
		if (virtualCamera == null)
		{
			cam = GetComponent<Camera>();
			virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
			hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
		}
		switch (@int)
		{
		case 0:
		case 1:
			colorPrecision = 64;
			break;
		case 2:
			colorPrecision = 32;
			break;
		case 3:
			colorPrecision = 16;
			break;
		case 4:
			colorPrecision = 8;
			break;
		case 5:
			colorPrecision = 3;
			break;
		}
		cPrecision[] array = Object.FindObjectsOfType<cPrecision>();
		cPrecision[] array2 = array;
		foreach (cPrecision cPrecision2 in array2)
		{
			if (@int == 0)
			{
				cPrecision2.enabled = false;
			}
			else
			{
				cPrecision2.colorPrecision = colorPrecision;
			}
		}
		if (@int != 0)
		{
			if (!pixeled)
			{
				cam.GetComponent<cPrecision>().enabled = true;
				hudCamera.GetComponent<cPrecision>().enabled = true;
			}
			else if (pixeled)
			{
				virtualCamera.GetComponent<cPrecision>().enabled = true;
			}
		}
	}

	public void CheckAspectRatio()
	{
		if (virtualCamera == null)
		{
			cam = GetComponent<Camera>();
			virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
			hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
			standardhud = GetComponentInChildren<HudController>().gameObject;
		}
		if (!pixeled)
		{
			aspectRatio = cam.aspect;
		}
		else
		{
			aspectRatio = virtualCamera.aspect;
			if (aspectRatio > 1.78f)
			{
				cam.targetTexture = ultraWideTargetMaterial;
				hudCamera.targetTexture = ultraWideHudTargetMaterial;
				virtualCamera.transform.Find("Standard").gameObject.SetActive(false);
				virtualCamera.transform.Find("Ultrawide").gameObject.SetActive(true);
				virtualCamera.transform.Find("StandardHud").gameObject.SetActive(false);
				virtualCamera.transform.Find("UltrawideHud").gameObject.SetActive(true);
			}
			else
			{
				cam.targetTexture = mainTargetMaterial;
				hudCamera.targetTexture = hudTargetMaterial;
				virtualCamera.transform.Find("Standard").gameObject.SetActive(true);
				virtualCamera.transform.Find("Ultrawide").gameObject.SetActive(false);
				virtualCamera.transform.Find("StandardHud").gameObject.SetActive(true);
				virtualCamera.transform.Find("UltrawideHud").gameObject.SetActive(false);
			}
		}
		if (aspectRatio < 1.25f)
		{
			standardhud.transform.localScale = new Vector3(0.5f, 1f, 1f);
		}
		else if (aspectRatio < 1.45f)
		{
			standardhud.transform.localScale = new Vector3(0.75f, 1f, 1f);
		}
		else if (aspectRatio < 1.6f)
		{
			standardhud.transform.localScale = new Vector3(0.825f, 1f, 1f);
		}
		else if (aspectRatio < 1.7f)
		{
			standardhud.transform.localScale = new Vector3(0.9f, 1f, 1f);
		}
		else
		{
			standardhud.transform.localScale = Vector3.one;
		}
	}

	public void CheckTilt()
	{
		if (PlayerPrefs.GetInt("CamTil", 1) == 1)
		{
			tilt = true;
		}
		else
		{
			tilt = false;
		}
	}

	public void CheckMouseReverse()
	{
		if (PlayerPrefs.GetInt("RevMouX", 0) == 1)
		{
			reverseX = true;
		}
		else
		{
			reverseX = false;
		}
		if (PlayerPrefs.GetInt("RevMouY", 0) == 1)
		{
			reverseY = true;
		}
		else
		{
			reverseY = false;
		}
	}
}
