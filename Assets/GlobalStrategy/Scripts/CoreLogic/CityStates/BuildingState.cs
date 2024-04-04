using UnityEngine.Assertions;

namespace GlobalStrategy.CoreLogic
{
	public class BuildingState : CityState
	{ 
		CityState nextStatePrefab;
		Products needToBuild; 
		Products buildingCost;

		public CityState NextStatePrefab => nextStatePrefab;
		
		public Products NeedToBuild => needToBuild;
		public Products BuildingCost => buildingCost;
		
		
		public void Init(CityState nextStatePrefab, Products buildingCost)
		{
			Assert.IsNotNull(nextStatePrefab);
			this.nextStatePrefab = nextStatePrefab;
			needToBuild  = buildingCost;
			this.buildingCost = buildingCost;
		}

		public override void Frame()
		{
			Products add = Products.Min(city.balance, needToBuild); 
			needToBuild -= add;
			city.Remove(add);

			if (needToBuild <= new Products(0, 0, 0, 0))
				BuildComplete();

		}

		void BuildComplete()
		{
			city.CreateNewStateFromPrefab(nextStatePrefab);
		}

		public override Products GetProduction()
		{
			return new Products(0, 0, 0, 0);
		}

		public override float GetRequest(int productIndex)
		{
			return needToBuild[productIndex];
		}

		public override Products GetSpending()
		{
			return new Products(0, 0, 0, 0);
		}
	}
}