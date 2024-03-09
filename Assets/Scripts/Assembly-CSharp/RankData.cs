using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class RankData
{
	public int[] ranks;

	public int secretsAmount;

	public bool[] secretsFound;

	public bool challenge;

	public int levelNumber;

	public RankData(StatsManager sman)
	{
		int @int = PlayerPrefs.GetInt("Diff", 2);
		string path = Application.persistentDataPath + "/lvl" + sman.levelNumber + "progress.bepis";
		levelNumber = sman.levelNumber;
		if (File.Exists(path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			RankData rankData = binaryFormatter.Deserialize(fileStream) as RankData;
			fileStream.Close();
			ranks = rankData.ranks;
			if (sman.rankScore > rankData.ranks[@int] || rankData.levelNumber != levelNumber)
			{
				ranks[@int] = sman.rankScore;
			}
			secretsAmount = sman.maxSecrets;
			secretsFound = new bool[secretsAmount];
			for (int i = 0; i < secretsAmount; i++)
			{
				if (sman.secretObjects[i] == null || rankData.secretsFound[i])
				{
					secretsFound[i] = true;
				}
			}
			challenge = rankData.challenge;
			return;
		}
		ranks = new int[5];
		for (int j = 0; j < ranks.Length; j++)
		{
			ranks[j] = -1;
		}
		ranks[@int] = sman.rankScore;
		secretsAmount = sman.maxSecrets;
		secretsFound = new bool[secretsAmount];
		for (int k = 0; k < secretsAmount; k++)
		{
			if (sman.secretObjects[k] == null)
			{
				secretsFound[k] = true;
			}
		}
	}
}
