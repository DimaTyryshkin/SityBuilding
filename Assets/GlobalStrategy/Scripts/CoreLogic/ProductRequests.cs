using System;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine; 


namespace GlobalStrategy.CoreLogic
{
	public class ProductRequests : MonoBehaviour
	{
		[SerializeField, IsntNull] RoadList roadList;
 
		City[] cities;

		public void Init()
		{
			roadList.Init();
			cities = FindObjectsOfType<City>();
			foreach (var c in cities)
				c.Init();
		}

		int resourceIndex;
		void Update()
		{
			resourceIndex = (resourceIndex + 1) % 4;
			
			foreach (var city in cities)
			{
				//TryRequestProduct(city, 0);
				//TryRequestProduct(city, 1);
				//TryRequestProduct(city, 2);
				//TryRequestProduct(city, 3);
			
				
				TryRequestProduct(city, resourceIndex); 
			}
		}
 
		void TryRequestProduct(City city, int productIndex, bool priority = false)
		{
			float needAmount = city.State.GetRequest(productIndex);
			//Debug.Log($"[d][log] 1 {city.name} needAmount={needAmount} {(FactoryCity.FactoryType)productIndex}");
			if (needAmount >= GroundRoad.packageSize)
			{
				GroundRoad[] roads = roadList.GetRoads(city);
				//Debug.Log($"[d][log] 2 {city.name} roads.count:{roads.Length}");

				foreach (GroundRoad r in roads)
				{
					//Debug.Log($"[d][log] 3 {city.name} roads.count:{roads.Length}");
					City otherCity = r.GetOtherCity(city);
					if (otherCity.country == city.country)
					{
						if(r.GetSide(city).CanSend())
						{ 
							//Debug.Log($"[d][log] 4 {city.name} roads.count:{roads.Length}");
							float availableAmount = CalculateSendAmount(otherCity, r, city, productIndex, priority);
							//Debug.Log($"[d] {otherCity.name}->{city.name} availableAmount={availableAmount}");
							if (availableAmount >= GroundRoad.packageSize)
							{
								SendProduct(otherCity, r, city, productIndex, GroundRoad.packageSize);
								//Debug.Log($"[d][SendProduct] {otherCity.name}->{city.name} availableAmount={availableAmount} {(FactoryCity.FactoryType)productIndex}");
							}
						} 
					}
				}
			}
		}

		float CalculateSendAmount(City from, GroundRoad road, City to, int productIndex, bool priority)
		{
			float fromFuelAmount = from.balance[0];
			float deliveryCost = road.GetFuelCost();
			if (fromFuelAmount - deliveryCost < 0)
				return 0;

			float fromAmount = from.balance[productIndex];
			float toAmount = to.balance[productIndex] + to.credit[productIndex];
			//bool isMax = fromAmount > 99.9f;

			float reminder = productIndex == 0 ? fromAmount - deliveryCost : fromAmount; // Сколько останется продукта, после траты топлива на пеервозку
			float canSendMid = (reminder - toAmount) * 0.5f; // Сколько нужно отправить, чтобы выровнять баланс между городами
			float canSendMax = reminder; // Отправить все, что есть
			return priority ? canSendMax : canSendMid;
		}

		void SendProduct(City from, GroundRoad road, City to, int productIndex, float amount)
		{
			Products deliveryProducts = new Products(productIndex, amount);

			Products removeProducts = deliveryProducts;
			removeProducts.Add(0, road.GetFuelCost());
			from.Remove(removeProducts);

			to.credit += deliveryProducts;
			road.GetSide(to).Send(deliveryProducts, OnDeliveryComplete);
		}

		void OnDeliveryComplete(City to, Products p)
		{
			to.credit -= p;
			to.credit.Clamp(0, City.maxCapacityValue);
			to.Add(p);
		}

		[Button()]
		void PrintStats()
		{
			var allCities = FindObjectsOfType<City>();

			Products productsSpend = new Products(0, 0, 0, 0);
			Products productsProduction = new Products(0, 0, 0, 0);
			foreach (var city in allCities)
			{ 
				productsProduction += city.State.GetProduction();
				productsSpend += city.State.GetSpending();
			}


			string s = "";
			s += $"r {productsProduction - productsSpend}"+ Environment.NewLine;
			s += $"p {productsProduction}"+ Environment.NewLine;
			s += $"s {productsSpend}"+ Environment.NewLine;
			
			Debug.Log(s);
		} 
	}
}