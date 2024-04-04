using GamePackages.Core.Validation;
using GlobalStrategy.CoreLogic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GlobalStrategy
{
	public class EmptyCityMenu : MonoBehaviour
	{
		[SerializeField, IsntNull] TextMeshProUGUI nameText;
		[SerializeField, IsntNull] Button factoryButton;

		EmptyCityState emptyCityState;
		
		void Start()
		{
			factoryButton.onClick.AddListener(OnClickFactoryButton);
		}
		
		public void Draw(EmptyCityState emptyCityState)
		{
			Assert.IsNotNull(emptyCityState);
			this.emptyCityState = emptyCityState;
			
			nameText.text = emptyCityState.name;
		}
		
		void OnClickFactoryButton()
		{
			emptyCityState.BuildFactory();
			gameObject.SetActive(false);
		}
	}
}