using UnityEngine;
using UnityEngine.UI;

public class SliderToFillAmount : MonoBehaviour
{
	public Slider targetSlider;

	public float maxFill;

	public bool copyColor;

	private Image img;

	private void Update()
	{
		if (img == null)
		{
			img = GetComponent<Image>();
		}
		img.fillAmount = (targetSlider.value - targetSlider.minValue) / (targetSlider.maxValue - targetSlider.minValue) * maxFill;
		if (copyColor)
		{
			img.color = targetSlider.targetGraphic.color;
		}
	}
}
