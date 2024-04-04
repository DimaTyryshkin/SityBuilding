using UnityEngine;
using UnityEngine.Assertions;

namespace GlobalStrategy.CoreLogic
{
	public abstract class CityState : MonoBehaviour
	{
		protected City city;

		public void SetCity(City city)
		{
			Assert.IsNotNull(city);
			this.city = city;
			//StartState();
		}
 
		public abstract void Frame();

		public abstract float GetRequest(int productIndex);
		public abstract Products GetProduction();
		public abstract Products GetSpending();
		//protected virtual void StartState() { }
	}
}