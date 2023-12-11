using System;
using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public class RoadLayer : MonoBehaviour
	{  
		Dictionary<Vector2Int, bool> cells;
		List<Vector2Int> cellsToUpdate;
		
		public List<Vector2Int> CellsToUpdate => cellsToUpdate;
		public event UnityAction Update;
 
		public void StartIt()
		{
			cellsToUpdate = new List<Vector2Int>(32);
			cells = new Dictionary<Vector2Int, bool>();

			// for (int i = 0; i < 3; i++)
			// {
			// 	for (int j = i % 2; j < 10; j += 2)
			// 	{
			// 		SetValue(new Vector2Int(i, j), true);
			// 	}
			// }
		}

		void LateUpdate()
		{
			if (cellsToUpdate.Count > 0)
			{
				Update?.Invoke();
				cellsToUpdate.Clear();
			}
		}

		public bool GetValue(Vector2Int cell)
		{
			return cells.GetOrDefault(cell, false);
		}


		public void SetValue(Vector2Int cell, bool value)
		{
			bool oldValue = cells.GetOrDefault(cell, false);
			cells[cell] = value;

			if (oldValue != value)
				cellsToUpdate.Add(cell);
		}
	}
}