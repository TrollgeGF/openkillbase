using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectButton : MonoBehaviour
{
	public int difficulty;

	private int stage;

	private int level;

	private void Start()
	{
		level = GameProgressSaver.GetProgress(difficulty);
		while (level > 5)
		{
			level -= 5;
			stage++;
		}
		base.transform.Find("Progress").GetComponent<Text>().text = "" + stage + "-" + level;
	}

	public void SetDifficulty()
	{
		PlayerPrefs.SetInt("Diff", difficulty);
		Debug.Log("Set Difficulty to: " + difficulty);
	}
}
