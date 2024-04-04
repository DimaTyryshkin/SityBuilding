using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GlobalStrategy.CoreLogic
{
	public class GroundRoad : Road
	{
		public static readonly float packageSize = 2;
		static readonly float packageDistance = 0.1f;
		static readonly float sideDeviation = packageDistance * 0.5f;
		[SerializeField] float throughput;
		[SerializeField, IsntNull] Delivery deliveryPrefab;
		
		float length;
		float timeNextSend; 
	  
		RoadSide side1;
		RoadSide side2;

		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(city1.transform.position, city2.transform.position);
		}
		
		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(city1.transform.position, city2.transform.position);
		}

		[Button()]
		public override void Init()
		{
			// D - дистанция между грузами
			// P - производительность трассы в секунду
			// P=a/T  a=T*P  T=a/P
			// D=T*V  V=D/T 
			
			float sendPeriod = packageSize / throughput;
			float deliverySpeed = packageDistance / sendPeriod;
			
			length = Vector3.Distance(city1.transform.position, city2.transform.position);

			side1 = new RoadSide(city1, city2, sendPeriod, sideDeviation, deliverySpeed, deliveryPrefab);
			side2 = new RoadSide(city2, city1, sendPeriod, sideDeviation, deliverySpeed, deliveryPrefab);
		}

		public override float GetFuelCost()
		{
			return 0;
			//return length;
		}

		public RoadSide GetSide(City toCity)
		{
			return toCity == side1.to ? side1 : side2;
		}
	}

	public class RoadSide
	{
		public readonly City to;

		readonly City from;
		readonly float deliverySpeed;
		readonly Delivery deliveryPrefab;
		readonly float sendPeriod;
		readonly Vector3 deviation;

		float timeNextSend;

		public RoadSide(City from, City to, float sendPeriod, float sideDeviation, float deliverySpeed, Delivery deliveryPrefab)
		{
			Assert.IsNotNull(from);
			Assert.IsNotNull(to);
			Assert.IsNotNull(deliveryPrefab);

			this.from = from;
			this.to = to;
			this.sendPeriod = sendPeriod;
			this.deliverySpeed = deliverySpeed;
			this.deliveryPrefab = deliveryPrefab;

			Vector3 dir = (to.transform.position - from.transform.position).normalized;
			deviation = Vector3.Cross(Vector3.up, dir) * sideDeviation;
		}

		public bool CanSend()
		{
			return Time.time > timeNextSend;
		}

		public void Send(Products products, UnityAction<City, Products> deliveryCallBack)
		{
			timeNextSend = Time.time + sendPeriod;

			Delivery delivery = Object.Instantiate(deliveryPrefab);
			delivery.transform.position = from.transform.position + deviation;
			delivery.Init(to, to.transform.position + deviation, deliverySpeed, products, deliveryCallBack);
			delivery.gameObject.SetActive(true);
		}
		
		
	}
}