using System;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public abstract class Road : MonoBehaviour
	{
		public City city1;
		public City city2;

		public abstract void Init();
		public abstract float GetFuelCost();

		public City GetOtherCity(City city)
		{
			if (city1 == city)
				return city2;
			
			if (city2 == city)
				return city1;

			throw new ArgumentException();
		}
	}
}