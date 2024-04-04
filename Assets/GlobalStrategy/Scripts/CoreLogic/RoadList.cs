using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public class RoadList : MonoBehaviour
	{
		GroundRoad[] groundRoads;
		Dictionary<City, GroundRoad[]> cityToRoad;

		public void Init()
		{
			groundRoads = FindObjectsOfType<GroundRoad>(false);
			foreach (var r in groundRoads)
				r.Init();
			
			cityToRoad = new Dictionary<City, GroundRoad[]>();
		}

		public GroundRoad[] GetRoads(City city)
		{
			if (cityToRoad.TryGetValue(city, out var roads))
			{
				return roads;
			}
			else
			{
				GroundRoad[] newRoads = groundRoads.Where(r => r.city1 == city || r.city2 == city).ToArray();
				cityToRoad[city] = newRoads;
				return newRoads;
			}
		}
	}
}