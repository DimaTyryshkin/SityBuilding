using UnityEngine;
using UnityEngine.Assertions;

namespace GlobalStrategy.CoreLogic
{
	public abstract class CityState : MonoBehaviour
	{
		protected City city;

		public void StartState(City city)
		{
			Assert.IsNotNull(city);
			this.city = city;
		}
 
		public abstract void Frame();

		public abstract Products GetProduction();
		public abstract Products GetSpending();
	}
}