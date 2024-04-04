using System;
using GamePackages.Core.Validation;
using GlobalStrategy.CoreLogic;
using TMPro;
using UnityEngine;

namespace GlobalStrategy
{
	public class FactoryMenu : MonoBehaviour
	{
		[SerializeField, IsntNull] TextMeshProUGUI nameText;
		
		public void Draw(FactoryCityState factory)
		{
			nameText.text = factory.name;
		}
	}
}