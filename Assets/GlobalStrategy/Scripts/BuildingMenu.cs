using System;
using GamePackages.Core.Validation;
using GlobalStrategy.CoreLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace GlobalStrategy
{
	public class BuildingMenu : MonoBehaviour
	{
		[SerializeField, IsntNull] TextMeshProUGUI nameText;
		[SerializeField, IsntNull] TextMeshProUGUI progressText;

		BuildingState state;

		public void Draw(BuildingState state)
		{
			Assert.IsNotNull(state);
			this.state = state;

			nameText.text = state.NextStatePrefab.name;
		}

		void Update()
		{
			if (!state)
			{
				gameObject.SetActive(false);
				return;
			}

			float progress = 1f - state.NeedToBuild.ComponentsSum / state.BuildingCost.ComponentsSum;
			progressText.text = $"{progress * 100:0}%";
		}
	}
}