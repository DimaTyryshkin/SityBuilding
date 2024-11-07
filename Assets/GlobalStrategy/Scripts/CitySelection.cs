using System;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using GlobalStrategy.CoreLogic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GlobalStrategy
{
	public class CitySelection : MonoBehaviour
	{
		[SerializeField, IsntNull] FactoryMenu factoryMenu;
		[SerializeField, IsntNull] EmptyCityMenu emptyCityMenu;
		[SerializeField, IsntNull] BuildingMenu buildingMenu;
		[SerializeField, IsntNull] TouchInput touchInput;


		[SerializeField, IsntNull] Animator cityCanvasAnimator;
		[SerializeField, IsntNull] GameObject cityCanvas;
		[SerializeField, IsntNull] GameObject cityGo;

		City selectedCity;
		CityState selectedCityState;

		void Start()
		{
			cityGo.SetActive(false);
			touchInput.ClickOnClickHandler += OnClick;
			cityCanvas.transform.localRotation = Quaternion.Euler(90, 0, 0);
		}

		void OnClick(IClickHandler clickHandler)
		{
			selectedCity = null;
			selectedCityState = null;
			
			if (clickHandler is City city)
			{
				selectedCity = city;
				selectedCityState = city.State;

				cityCanvas.transform.position = city.transform.position;
				cityCanvas.SetActive(true);
				cityCanvasAnimator.SetTrigger("select");
				
				
				if (city.State is FactoryCityState factory)
					ShowFactoryMenu(factory);
				
				if (city.State is EmptyCityState emptyCity)
					ShowEmptyCityMenu(emptyCity);
				
				if (city.State is BuildingState buildingState)
					ShowBuildingMenu(buildingState);
			}
		}

		void Update()
		{
			if (selectedCity)
			{
				if (selectedCityState != selectedCity.State)
					OnClick(selectedCity);
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