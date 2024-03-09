using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	private NewMovement nmov;

	public Slider hpSlider;

	public Slider afterImageSlider;

	public Text hpText;

	private float hp;

	public bool changeTextColor;

	public Color normalTextColor;

	public bool yellowColor;

	private void Start()
	{
		nmov = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
	}

	private void Update()
	{
		if (hp < (float)nmov.hp)
		{
			hp = Mathf.MoveTowards(hp, nmov.hp, Time.deltaTime * (((float)nmov.hp - hp) * 5f + 5f));
		}
		else if (hp > (float)nmov.hp)
		{
			hp = nmov.hp;
		}
		if (hpSlider != null && hpSlider.value != hp)
		{
			hpSlider.value = hp;
		}
		if (afterImageSlider != null)
		{
			if (afterImageSlider.value < hp)
			{
				afterImageSlider.value = hp;
			}
			else if (afterImageSlider.value > hp)
			{
				afterImageSlider.value = Mathf.MoveTowards(afterImageSlider.value, hp, Time.deltaTime * ((afterImageSlider.value - hp) * 5f + 5f));
			}
		}
		if (!(hpText != null))
		{
			return;
		}
		hpText.text = hp.ToString("F0");
		if (changeTextColor)
		{
			if (hp <= 30f)
			{
				hpText.color = Color.red;
			}
			else if (hp <= 50f && yellowColor)
			{
				hpText.color = Color.yellow;
			}
			else
			{
				hpText.color = normalTextColor;
			}
		}
	}
}
