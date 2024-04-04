using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalStrategy.DebugGui
{
	public class TimeScalePanel : MonoBehaviour
	{
		[SerializeField] Slider timeScaleSlider;
		[SerializeField] Button resetTimeScale;
		[SerializeField] TextMeshProUGUI scaleText;

		void Start()
		{
			DrawText();
			timeScaleSlider.value = Time.timeScale;
			timeScaleSlider.onValueChanged.AddListener(TimeScaleSlider_OnValueChanged);

			resetTimeScale.onClick.AddListener(() => timeScaleSlider.value = 1);
		}

		void TimeScaleSlider_OnValueChanged(float scale)
		{
			Time.timeScale = scale;
			DrawText();
		}

		void DrawText()
		{
			scaleText.text = $"{Time.timeScale:0.0}";
		}
	}
}