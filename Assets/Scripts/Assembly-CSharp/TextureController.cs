using UnityEngine;

public class TextureController : MonoBehaviour
{
	private bool textureWarping = true;

	private int vertexWarping = 2;

	private void Awake()
	{
		CheckTextureWarping();
		if (PlayerPrefs.GetInt("VerWar", 2) != 2)
		{
			CheckVertexWarping();
		}
	}

	public void CheckTextureWarping()
	{
		float value = PlayerPrefs.GetFloat("TexWar", 100f) / 100f;
		Material[] array = Resources.FindObjectsOfTypeAll<Material>();
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			material.SetFloat("_TextureWarping", value);
		}
	}

	public void CheckVertexWarping()
	{
		int @int = PlayerPrefs.GetInt("VerWar", 2);
		if (@int != vertexWarping)
		{
			vertexWarping = @int;
			float value = 0f;
			switch (vertexWarping)
			{
			case 0:
				value = 9999f;
				break;
			case 1:
				value = 400f;
				break;
			case 2:
				value = 160f;
				break;
			case 3:
				value = 80f;
				break;
			case 4:
				value = 40f;
				break;
			case 5:
				value = 16f;
				break;
			}
			Material[] array = Resources.FindObjectsOfTypeAll<Material>();
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				material.SetFloat("_VertexWarping", value);
			}
		}
	}
}
