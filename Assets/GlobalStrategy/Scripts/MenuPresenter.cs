using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using GlobalStrategy.CoreLogic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GlobalStrategy
{
	public class MenuPresenter : MonoBehaviour
	{
		[SerializeField, IsntNull] FactoryMenu factoryMenu;
		[SerializeField, IsntNull] EmptyCityMenu emptyCityMenu;
		[SerializeField, IsntNull] BuildingMenu buildingMenu;
		[SerializeField, IsntNull] TouchInput touchInput;

		void Start()
		{
			touchInput.ClickOnClickHandler += OnClick;
		}

		void OnClick(IClickHandler clickHandler)
		{
			if (clickHandler is City city)
			{
				if (city.State is FactoryCityState factory)
					ShowFactoryMenu(factory);
				
				if (city.State is EmptyCityState emptyCity)
					ShowEmptyCityMenu(emptyCity);
				
				if (city.State is BuildingState buildingState)
					ShowBuildingMenu(buildingState);
			}
		}

		void HideAll()
		{
			factoryMenu.gameObject.SetActive(false);
			emptyCityMenu.gameObject.SetActive(false);
			buildingMenu.gameObject.SetActive(false);
		}

		void ShowFactoryMenu(FactoryCityState factory)
		{
			HideAll();
			factoryMenu.Draw(factory);
			factoryMenu.gameObject.SetActive(true);
		}
		
		void ShowEmptyCityMenu(EmptyCityState emptyCity)
		{
			HideAll();
			emptyCityMenu.Draw(emptyCity);
			emptyCityMenu.gameObject.SetActive(true);
		}
		
		void ShowBuildingMenu(BuildingState state)
		{
			HideAll();
			buildingMenu.Draw(state);
			buildingMenu.gameObject.SetActive(true);
		}
	}
}