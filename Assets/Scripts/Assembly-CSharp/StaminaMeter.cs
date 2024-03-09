using UnityEngine;
using UnityEngine.UI;

public class StaminaMeter : MonoBehaviour
{
	private NewMovement nmov;

	private float stamina;

	private Slider stm;

	private Text stmText;

	public bool changeTextColor;

	public Color normalTextColor;

	private Image staminaFlash;

	private Color flashColor;

	private Image staminaBar;

	private bool full = true;

	private AudioSource aud;

	public Color emptyColor;

	private Color origColor;

	private void Start()
	{
		stm = GetComponent<Slider>();
		if (stm != null)
		{
			staminaBar = base.transform.GetChild(1).GetChild(0).GetComponent<Image>();
			staminaFlash = staminaBar.transform.GetChild(0).GetComponent<Image>();
			flashColor = staminaFlash.color;
			origColor = staminaBar.color;
		}
		stmText = GetComponent<Text>();
		nmov = GameObject.FindWithTag("Player").GetComponent<NewMovement>();
	}

	private void Update()
	{
		if (stamina < nmov.boostCharge)
		{
			stamina = nmov.boostCharge;
		}
		else if (stamina > nmov.boostCharge)
		{
			stamina = Mathf.MoveTowards(stamina, nmov.boostCharge, Time.deltaTime * ((stamina - nmov.boostCharge) * 25f + 25f));
		}
		if (stm != null)
		{
			stm.value = stamina;
			if (stm.value >= stm.maxValue && !full)
			{
				full = true;
				staminaBar.color = origColor;
				Flash();
			}
			if (flashColor.a > 0f)
			{
				if (flashColor.a - Time.deltaTime > 0f)
				{
					flashColor.a -= Time.deltaTime;
				}
				else
				{
					flashColor.a = 0f;
				}
				staminaFlash.color = flashColor;
			}
			if (stm.value < stm.maxValue)
			{
				full = false;
				flashColor.a = 0f;
				staminaFlash.color = flashColor;
				staminaBar.color = emptyColor;
			}
		}
		if (!(stmText != null))
		{
			return;
		}
		stmText.text = stamina.ToString("F0");
		if (changeTextColor)
		{
			if (stamina < 100f)
			{
				stmText.color = Color.red;
			}
			else
			{
				stmText.color = normalTextColor;
			}
		}
	}

	private void Flash()
	{
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		aud.Play();
		flashColor.a = 1f;
		staminaFlash.color = flashColor;
	}
}
