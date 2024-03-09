using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
	public Toggle pixelization;

	public Slider textureWarping;

	public Dropdown vertexWarping;

	public Dropdown colorCompression;

	private CameraController cc;

	private TextureController texcon;

	private void Start()
	{
		if (PlayerPrefs.GetInt("Pix", 1) == 0)
		{
			pixelization.isOn = false;
		}
		else
		{
			pixelization.isOn = true;
		}
		textureWarping.value = PlayerPrefs.GetFloat("TexWar", 100f);
		vertexWarping.value = PlayerPrefs.GetInt("VerWar", 2);
		vertexWarping.RefreshShownValue();
		colorCompression.value = PlayerPrefs.GetInt("ColCom", 2);
		colorCompression.RefreshShownValue();
	}

	public void Pixelization(bool stuff)
	{
		if (stuff)
		{
			PlayerPrefs.SetInt("Pix", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Pix", 0);
		}
		if (cc == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		}
		cc.CheckPixelization();
	}

	public void TextureWarping(float stuff)
	{
		PlayerPrefs.SetFloat("TexWar", stuff);
		if (texcon == null)
		{
			texcon = Object.FindObjectOfType<TextureController>();
		}
		texcon.CheckTextureWarping();
	}

	public void VertexWarping(int stuff)
	{
		PlayerPrefs.SetInt("VerWar", stuff);
		if (texcon == null)
		{
			texcon = Object.FindObjectOfType<TextureController>();
		}
		texcon.CheckVertexWarping();
	}

	public void ColorCompression(int stuff)
	{
		PlayerPrefs.SetInt("ColCom", stuff);
		if (cc == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		}
		cc.CheckColorCompression();
	}
}
