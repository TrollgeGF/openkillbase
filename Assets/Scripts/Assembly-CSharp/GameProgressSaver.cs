using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameProgressSaver
{
	public static void SaveProgress(int levelnum)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/difficulty" + PlayerPrefs.GetInt("Diff", 2) + "progress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressData gameProgressData = binaryFormatter.Deserialize(fileStream) as GameProgressData;
			fileStream.Close();
			if (gameProgressData.levelNum < levelnum || gameProgressData.difficulty != PlayerPrefs.GetInt("Diff", 2))
			{
				GameProgressData graph = new GameProgressData(levelnum);
				fileStream = new FileStream(path, FileMode.Create);
				binaryFormatter.Serialize(fileStream, graph);
				fileStream.Close();
			}
		}
		else
		{
			GameProgressData graph2 = new GameProgressData(levelnum);
			FileStream fileStream = new FileStream(path, FileMode.Create);
			binaryFormatter.Serialize(fileStream, graph2);
			fileStream.Close();
		}
	}

	public static int GetProgress(int difficulty)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/difficulty" + difficulty + "progress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressData gameProgressData = binaryFormatter.Deserialize(fileStream) as GameProgressData;
			fileStream.Close();
			if (gameProgressData.difficulty == difficulty)
			{
				return gameProgressData.levelNum;
			}
			return 1;
		}
		return 1;
	}

	public static void AddGear(string gear)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		GameProgressMoneyAndGear gameProgressMoneyAndGear;
		FileStream fileStream;
		if (File.Exists(path))
		{
			fileStream = new FileStream(path, FileMode.Open);
			gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
		}
		else
		{
			gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
			gameProgressMoneyAndGear.secretMissions = new bool[10];
		}
		switch (gear)
		{
		case "rev0":
			gameProgressMoneyAndGear.rev0 = 1;
			break;
		case "rev1":
			gameProgressMoneyAndGear.rev1 = 1;
			break;
		case "rev2":
			gameProgressMoneyAndGear.rev2 = 1;
			break;
		case "rev3":
			gameProgressMoneyAndGear.rev3 = 1;
			break;
		case "sho0":
			gameProgressMoneyAndGear.sho0 = 1;
			break;
		case "sho1":
			gameProgressMoneyAndGear.sho1 = 1;
			break;
		case "sho2":
			gameProgressMoneyAndGear.sho2 = 1;
			break;
		case "sho3":
			gameProgressMoneyAndGear.sho3 = 1;
			break;
		case "nai0":
			gameProgressMoneyAndGear.nai0 = 1;
			break;
		case "nai1":
			gameProgressMoneyAndGear.nai1 = 1;
			break;
		case "nai2":
			gameProgressMoneyAndGear.nai2 = 1;
			break;
		case "nai3":
			gameProgressMoneyAndGear.nai3 = 1;
			break;
		}
		fileStream = new FileStream(path, FileMode.Create);
		binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
		fileStream.Close();
	}

	public static int CheckGear(string gear)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
			switch (gear)
			{
			case "rev0":
				return gameProgressMoneyAndGear.rev0;
			case "rev1":
				return gameProgressMoneyAndGear.rev1;
			case "rev2":
				return gameProgressMoneyAndGear.rev2;
			case "rev3":
				return gameProgressMoneyAndGear.rev3;
			case "sho0":
				return gameProgressMoneyAndGear.sho0;
			case "sho1":
				return gameProgressMoneyAndGear.sho1;
			case "sho2":
				return gameProgressMoneyAndGear.sho2;
			case "sho3":
				return gameProgressMoneyAndGear.sho3;
			case "nai0":
				return gameProgressMoneyAndGear.nai0;
			case "nai1":
				return gameProgressMoneyAndGear.nai1;
			case "nai2":
				return gameProgressMoneyAndGear.nai2;
			case "nai3":
				return gameProgressMoneyAndGear.nai3;
			default:
				return 0;
			}
		}
		return 0;
	}

	public static void LoadGear()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
			if (gameProgressMoneyAndGear.rev0 > 0)
			{
				PlayerPrefs.SetInt("rev0", 1);
			}
			if (gameProgressMoneyAndGear.rev1 > 0)
			{
				PlayerPrefs.SetInt("rev1", 1);
			}
			if (gameProgressMoneyAndGear.rev2 > 0)
			{
				PlayerPrefs.SetInt("rev2", 1);
			}
			if (gameProgressMoneyAndGear.rev3 > 0)
			{
				PlayerPrefs.SetInt("rev3", 1);
			}
			if (gameProgressMoneyAndGear.sho0 > 0)
			{
				PlayerPrefs.SetInt("sho0", 1);
			}
			if (gameProgressMoneyAndGear.sho1 > 0)
			{
				PlayerPrefs.SetInt("sho1", 1);
			}
			if (gameProgressMoneyAndGear.sho2 > 0)
			{
				PlayerPrefs.SetInt("sho2", 1);
			}
			if (gameProgressMoneyAndGear.sho3 > 0)
			{
				PlayerPrefs.SetInt("sho3", 1);
			}
			if (gameProgressMoneyAndGear.nai0 > 0)
			{
				PlayerPrefs.SetInt("nai0", 1);
			}
			if (gameProgressMoneyAndGear.nai1 > 0)
			{
				PlayerPrefs.SetInt("nai1", 1);
			}
			if (gameProgressMoneyAndGear.nai2 > 0)
			{
				PlayerPrefs.SetInt("nai2", 1);
			}
			if (gameProgressMoneyAndGear.nai3 > 0)
			{
				PlayerPrefs.SetInt("nai3", 1);
			}
		}
	}

	public static void AddMoney(int money)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		GameProgressMoneyAndGear gameProgressMoneyAndGear;
		FileStream fileStream;
		if (File.Exists(path))
		{
			fileStream = new FileStream(path, FileMode.Open);
			gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
		}
		else
		{
			gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
			gameProgressMoneyAndGear.secretMissions = new bool[10];
		}
		gameProgressMoneyAndGear.money += money;
		fileStream = new FileStream(path, FileMode.Create);
		binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
		fileStream.Close();
	}

	public static int GetMoney()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
			return gameProgressMoneyAndGear.money;
		}
		return 0;
	}

	public static bool GetTutorial()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
			return gameProgressMoneyAndGear.tutorialBeat;
		}
		PlayerPrefs.SetInt("rev0", 0);
		PlayerPrefs.SetInt("rev1", 0);
		PlayerPrefs.SetInt("rev2", 0);
		PlayerPrefs.SetInt("rev3", 0);
		PlayerPrefs.SetInt("sho0", 0);
		PlayerPrefs.SetInt("sho1", 0);
		PlayerPrefs.SetInt("sho2", 0);
		PlayerPrefs.SetInt("sho3", 0);
		PlayerPrefs.SetInt("nai0", 0);
		PlayerPrefs.SetInt("nai1", 0);
		PlayerPrefs.SetInt("nai2", 0);
		PlayerPrefs.SetInt("nai3", 0);
		PlayerPrefs.SetInt("Diff", 2);
		return false;
	}

	public static void SetTutorial(bool beat)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		GameProgressMoneyAndGear gameProgressMoneyAndGear;
		FileStream fileStream;
		if (File.Exists(path))
		{
			fileStream = new FileStream(path, FileMode.Open);
			gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
		}
		else
		{
			gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
			gameProgressMoneyAndGear.secretMissions = new bool[10];
		}
		gameProgressMoneyAndGear.tutorialBeat = beat;
		fileStream = new FileStream(path, FileMode.Create);
		binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
		fileStream.Close();
	}

	public static bool GetSecretMission(int missionNumber)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			GameProgressMoneyAndGear gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
			if (gameProgressMoneyAndGear.secretMissions.Length == 0)
			{
				gameProgressMoneyAndGear.secretMissions = new bool[10];
			}
			return gameProgressMoneyAndGear.secretMissions[missionNumber];
		}
		return false;
	}

	public static void SetSecretMission(int missionNumber)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/generalprogress.bepis";
		GameProgressMoneyAndGear gameProgressMoneyAndGear;
		FileStream fileStream;
		if (File.Exists(path))
		{
			fileStream = new FileStream(path, FileMode.Open);
			gameProgressMoneyAndGear = binaryFormatter.Deserialize(fileStream) as GameProgressMoneyAndGear;
			fileStream.Close();
		}
		else
		{
			gameProgressMoneyAndGear = new GameProgressMoneyAndGear();
			gameProgressMoneyAndGear.secretMissions = new bool[10];
		}
		gameProgressMoneyAndGear.secretMissions[missionNumber] = true;
		fileStream = new FileStream(path, FileMode.Create);
		binaryFormatter.Serialize(fileStream, gameProgressMoneyAndGear);
		fileStream.Close();
	}
}
