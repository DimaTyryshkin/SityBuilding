using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public class FactoryCityState : CityState
	{
		public Products liveCost;
		public Products productionCost;
		//static readonly float productionPeriod = 0.2f;
		 
		[SerializeField] FactoryType factoryType;
		[SerializeField] int productivity;
   
		public override void Frame()
		{
			// a=T*P пакет = период * производительность  
			
			Products frameLiveCost = liveCost * Time.deltaTime;
			bool isLive = city.CanRemove(frameLiveCost);
			city.Remove(frameLiveCost);
			
			if(!isLive)
				return;
			
			if (city.balance[(int)factoryType] >= City.normalCapacityValue)
				return;

			var productionFrameCost = productionCost * Time.deltaTime;
			if (!city.CanRemove(productionFrameCost))
				return;

			city.Remove(productionFrameCost);
			city.Add((int)factoryType, productivity * Time.deltaTime);
		}

		public override Products GetProduction()
		{
			return factoryType == FactoryType.None ? 
				new Products(0, 0, 0, 0) : 
				new Products((int)factoryType, productivity);
		}

		public override float GetRequest(int productIndex)
		{
			return City.normalCapacityValue - city.balance[productIndex];
		}

		public override Products GetSpending()
		{
			return liveCost + productionCost;
		}
	}
}