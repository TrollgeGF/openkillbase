using UnityEngine;

namespace cakeslice
{
	public class OutlineAnimation : MonoBehaviour
	{
		private bool pingPong = false;

		private void Start()
		{
		}

		private void Update()
		{
			Color lineColor = GetComponent<OutlineEffect>().lineColor0;
			if (pingPong)
			{
				lineColor.a += Time.deltaTime;
				if (lineColor.a >= 1f)
				{
					pingPong = false;
				}
			}
			else
			{
				lineColor.a -= Time.deltaTime;
				if (lineColor.a <= 0f)
				{
					pingPong = true;
				}
			}
			lineColor.a = Mathf.Clamp01(lineColor.a);
			GetComponent<OutlineEffect>().lineColor0 = lineColor;
			GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
		}
	}
}
