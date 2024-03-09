using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
	public GameObject[] checkPoints;

	private GameObject player;

	private NewMovement nm;

	public Vector3 spawnPos;

	public CheckPoint currentCheckPoint;

	public GameObject debugCheckPoint;

	public int levelNumber;

	public int kills;

	public int stylePoints;

	public int restarts;

	public int secrets;

	public int maxSecrets;

	public float seconds;

	private bool timer;

	public FinalRank fr;

	private StyleHUD shud;

	private GunControl gunc;

	public bool infoSent;

	public bool casualFR;

	public int[] timeRanks;

	public int[] killRanks;

	public int[] styleRanks;

	public int rankScore;

	public GameObject[] secretObjects;

	public List<int> prevSecrets;

	public bool challengeComplete;

	public AudioClip[] rankSounds;

	public int maxGlassKills;

	public GameObject crosshair;

	public bool tookDamage;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		nm = player.GetComponent<NewMovement>();
		spawnPos = player.transform.position;
		fr = player.GetComponentInChildren<FinalRank>();
		fr.gameObject.SetActive(false);
		spawnPos = player.transform.position;
		GameObject gameObject = GameObject.FindWithTag("PlayerPosInfo");
		if (gameObject != null)
		{
			PlayerPosInfo component = gameObject.GetComponent<PlayerPosInfo>();
			if (!component.noPosition)
			{
				player.transform.position = component.position;
			}
			player.GetComponent<Rigidbody>().velocity = component.velocity;
			player.GetComponentInChildren<WallCheck>().GetComponent<AudioSource>().time = component.wooshTime;
			Object.Destroy(gameObject);
		}
		else
		{
			player.GetComponent<Rigidbody>().velocity = Vector3.down * 100f;
		}
		string path = Application.persistentDataPath + "/lvl" + levelNumber + "progress.bepis";
		if (!File.Exists(path))
		{
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(path, FileMode.Open);
		RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
		fileStream.Close();
		if (rankData.levelNumber != levelNumber)
		{
			return;
		}
		for (int i = 0; i < maxSecrets; i++)
		{
			if (rankData.secretsFound[i])
			{
				Object.Destroy(secretObjects[i]);
				secretObjects[i] = null;
				prevSecrets.Add(i);
			}
		}
		if (rankData.challenge)
		{
			challengeComplete = true;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R) && nm.hp <= 0)
		{
			Restart();
		}
		if (timer)
		{
			seconds += Time.deltaTime;
		}
		if (stylePoints < 0)
		{
			stylePoints = 0;
		}
	}

	public void GetCheckPoint(Vector3 position)
	{
		spawnPos = position;
	}

	public void Restart()
	{
		GetComponentInChildren<MusicManager>().ArenaMusicEnd();
		timer = true;
		if (currentCheckPoint == null)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
			return;
		}
		currentCheckPoint.OnRespawn();
		restarts++;
	}

	public void StartTimer()
	{
		timer = true;
	}

	public void StopTimer()
	{
		timer = false;
	}

	public void HideShit()
	{
		if (shud == null)
		{
			shud = player.GetComponentInChildren<StyleHUD>();
		}
		shud.ComboOver();
		if (gunc == null)
		{
			gunc = player.GetComponentInChildren<GunControl>();
		}
		gunc.NoWeapon();
		crosshair.transform.parent.gameObject.SetActive(false);
		fr.transform.parent.parent.GetChild(0).gameObject.SetActive(false);
	}

	public void SendInfo()
	{
		if (!infoSent)
		{
			infoSent = true;
			string text = "";
			fr.gameObject.SetActive(true);
			if (!casualFR)
			{
				text = GetRanks(timeRanks, seconds, true);
				SetRankSound(text, fr.timeRank.gameObject);
				fr.SetTime(seconds, text);
				text = GetRanks(killRanks, kills, false);
				SetRankSound(text, fr.killsRank.gameObject);
				fr.SetKills(kills, text);
				text = GetRanks(styleRanks, stylePoints, false);
				SetRankSound(text, fr.styleRank.gameObject);
				fr.SetStyle(stylePoints, text);
				fr.SetInfo(restarts, tookDamage);
				GetFinalRank();
				RankSaveSystem.SaveRank(this);
				fr.SetSecrets(secrets, maxSecrets);
				fr.levelSecrets = secretObjects;
				fr.prevSecrets = prevSecrets;
			}
			else
			{
				fr.StartLevelLoad();
			}
			fr.Appear();
		}
	}

	private string GetRanks(int[] ranksToCheck, float value, bool reverse)
	{
		int num = 0;
		bool flag = true;
		while (flag)
		{
			if (num < ranksToCheck.Length)
			{
				if ((reverse && value <= (float)ranksToCheck[num]) || (!reverse && value >= (float)ranksToCheck[num]))
				{
					num++;
					continue;
				}
				rankScore += num;
				switch (num)
				{
				case 0:
					return "<color=#0094FF>D</color>";
				case 1:
					return "<color=#4CFF00>C</color>";
				case 2:
					return "<color=#FFD800>B</color>";
				case 3:
					return "<color=#FF6A00>A</color>";
				}
				continue;
			}
			rankScore += 4;
			return "<color=#FF0000>S</color>";
		}
		return "X";
	}

	private void GetFinalRank()
	{
		string text = "";
		if (restarts != 0)
		{
			rankScore -= restarts;
		}
		if (rankScore == 12)
		{
			text = "<color=#FFFFFF>P</color>";
			fr.totalRank.transform.parent.GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 1f);
		}
		else
		{
			float num = (float)rankScore / 3f;
			Debug.Log("Float: " + num);
			Debug.Log("PreInt: " + rankScore);
			rankScore = Mathf.RoundToInt(num);
			Debug.Log("PostInt: " + rankScore);
			switch (rankScore)
			{
			case 1:
				text = "<color=#4CFF00>C</color>";
				break;
			case 2:
				text = "<color=#FFD800>B</color>";
				break;
			case 3:
				text = "<color=#FF6A00>A</color>";
				break;
			case 4:
			case 5:
			case 6:
				text = "<color=#FF0000>S</color>";
				break;
			default:
				text = "<color=#0094FF>D</color>";
				break;
			}
		}
		fr.SetRank(text);
	}

	private void SetRankSound(string rank, GameObject target)
	{
		switch (rank)
		{
		case "<color=#FFD800>B</color>":
			target.GetComponent<AudioSource>().clip = rankSounds[0];
			break;
		case "<color=#FF6A00>A</color>":
			target.GetComponent<AudioSource>().clip = rankSounds[1];
			break;
		case "<color=#FF0000>S</color>":
			target.GetComponent<AudioSource>().clip = rankSounds[2];
			break;
		}
	}
}
