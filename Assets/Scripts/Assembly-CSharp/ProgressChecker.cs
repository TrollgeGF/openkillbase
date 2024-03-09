using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressChecker : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!GameProgressSaver.GetTutorial())
		{
			SceneManager.LoadScene("Tutorial");
		}
	}
}
