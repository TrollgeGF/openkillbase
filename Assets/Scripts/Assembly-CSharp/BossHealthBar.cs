using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	public GameObject bossBar;

	public Slider[] hpSlider;

	public Slider[] hpAfterImage;

	private EnemyIdentifier eid;

	private SpiderBody spb;

	private Machine mac;

	private Statue stat;

	public string bossName;

	private float introCharge;

	private GameObject filler;

	private float shakeTime;

	private Vector3 originalPosition;

	private bool done;

	public FinalDoor finalDoor;

	private int currentHpSlider;

	private int currentAfterImageSlider;

	private MusicManager mman;

	private void Awake()
	{
		eid = GetComponent<EnemyIdentifier>();
		hpSlider[0].transform.parent.gameObject.GetComponentInChildren<Text>().text = bossName;
		if (eid.type == EnemyType.Spider)
		{
			spb = GetComponent<SpiderBody>();
		}
		if (eid.type == EnemyType.Machine)
		{
			mac = GetComponent<Machine>();
		}
		if (eid.type == EnemyType.Statue)
		{
			stat = GetComponent<Statue>();
		}
		filler = bossBar.transform.GetChild(0).gameObject;
		originalPosition = filler.transform.position;
		bossBar.SetActive(true);
		currentHpSlider = hpSlider.Length - 1;
		currentAfterImageSlider = currentHpSlider;
		Slider[] array = hpSlider;
		foreach (Slider slider in array)
		{
			slider.value = 0f;
		}
		Slider[] array2 = hpAfterImage;
		foreach (Slider slider2 in array2)
		{
			slider2.value = 0f;
		}
		if (mman == null)
		{
			mman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		}
		mman.PlayBossMusic();
	}

	private void OnEnable()
	{
		if (mman == null)
		{
			mman = GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>();
		}
		mman.PlayBossMusic();
	}

	private void OnDisable()
	{
		DisappearBar();
	}

	private void Update()
	{
		if (eid.type == EnemyType.Spider && hpSlider[currentHpSlider].value != spb.health)
		{
			if (introCharge < spb.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, spb.health, (spb.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array = hpSlider;
				foreach (Slider slider in array)
				{
					slider.value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - spb.health);
				hpSlider[currentHpSlider].value = spb.health;
				if (hpSlider[currentHpSlider].minValue > spb.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = spb.health;
				}
			}
		}
		else if (eid.type == EnemyType.Machine && hpSlider[currentHpSlider].value != mac.health)
		{
			if (introCharge < mac.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, mac.health, (mac.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array2 = hpSlider;
				foreach (Slider slider2 in array2)
				{
					slider2.value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - mac.health);
				hpSlider[currentHpSlider].value = mac.health;
				if (hpSlider[currentHpSlider].minValue > mac.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = mac.health;
				}
			}
		}
		else if (eid.type == EnemyType.Statue && hpSlider[currentHpSlider].value != stat.health)
		{
			if (introCharge < stat.health)
			{
				introCharge = Mathf.MoveTowards(introCharge, stat.health, (stat.health - introCharge) * Time.deltaTime * 3f);
				Slider[] array3 = hpSlider;
				foreach (Slider slider3 in array3)
				{
					slider3.value = introCharge;
				}
			}
			else
			{
				shakeTime = 5f * (hpSlider[currentHpSlider].value - stat.health);
				hpSlider[currentHpSlider].value = stat.health;
				if (hpSlider[currentHpSlider].minValue > stat.health && currentHpSlider > 0)
				{
					currentHpSlider--;
					hpSlider[currentHpSlider].value = stat.health;
				}
			}
		}
		if (hpAfterImage[currentAfterImageSlider].value != hpSlider[currentHpSlider].value)
		{
			if (hpAfterImage[currentAfterImageSlider].value > hpSlider[currentHpSlider].value)
			{
				hpAfterImage[currentAfterImageSlider].value = Mathf.MoveTowards(hpAfterImage[currentAfterImageSlider].value, hpSlider[currentHpSlider].value, Time.deltaTime * (Mathf.Abs(hpAfterImage[currentAfterImageSlider].value - hpSlider[currentHpSlider].value) + 0.5f));
			}
			else
			{
				hpAfterImage[currentAfterImageSlider].value = hpSlider[currentHpSlider].value;
			}
			if (hpAfterImage[currentAfterImageSlider].value <= hpAfterImage[currentAfterImageSlider].minValue && currentAfterImageSlider > 0)
			{
				currentAfterImageSlider--;
			}
		}
		if (shakeTime != 0f)
		{
			if (shakeTime > 10f)
			{
				shakeTime = 10f;
			}
			shakeTime -= Time.deltaTime * 10f;
			filler.transform.position = new Vector3(originalPosition.x + Random.Range(0f - shakeTime, shakeTime), originalPosition.y + Random.Range(0f - shakeTime, shakeTime), originalPosition.z);
			if (shakeTime < 0f)
			{
				shakeTime = 0f;
				filler.transform.position = originalPosition;
			}
		}
		if (!done && hpSlider[0].value <= 0f)
		{
			done = true;
			if (finalDoor != null)
			{
				Invoke("OpenDoors", 1f);
			}
			Invoke("DisappearBar", 3f);
			Debug.Log("Invoking");
		}
	}

	private void OpenDoors()
	{
		finalDoor.Open();
	}

	public void DisappearBar()
	{
		if (bossBar != null)
		{
			bossBar.SetActive(false);
		}
	}
}
