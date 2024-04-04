using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public class EmptyCityState : CityState
	{
		[SerializeField, IsntNull] FactoryCityState factoryPrefab;
		[SerializeField, IsntNull] BuildingState buildingPrefab;
		[SerializeField] Products buildingCost;
		
		public override void Frame()
		{
			 
		}

		public override Products GetProduction()
		{
			return new Products(0, 0, 0, 0);
		}

		public override float GetRequest(int productIndex)
		{
			return 0;
		}

		public override Products GetSpending()
		{
			return new Products(0, 0, 0, 0);
		}

		public void BuildFactory()
		{
			var building = city.CreateNewStateFromPrefab(buildingPrefab);
			building.Init(factoryPrefab, buildingCost);
		}
	}
}