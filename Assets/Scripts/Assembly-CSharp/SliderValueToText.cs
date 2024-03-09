using UnityEngine;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
	private Slider targetSlider;

	private Text targetText;

	public string suffix;

	public string ifMax;

	public string ifMin;

	private void Start()
	{
		targetSlider = GetComponentInParent<Slider>();
		targetText = GetComponent<Text>();
	}

	private void Update()
	{
		if (ifMax != "" && targetSlider.value == targetSlider.maxValue)
		{
			targetText.text = ifMax;
		}
		else if (ifMin != "" && targetSlider.value == targetSlider.minValue)
		{
			targetText.text = ifMin;
		}
		else
		{
			targetText.text = targetSlider.value + suffix;
		}
	}
}
