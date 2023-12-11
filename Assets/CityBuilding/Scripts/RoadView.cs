using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;

namespace Game
{
	public class RoadView:MonoBehaviour
	{
		[SerializeField] RoadLayer roadLayer;
		[SerializeField] GameGrid gameGrid;
		[SerializeField] RoadViewStore roadViewMap;
		
		Dictionary<Vector2Int, GameObject> cellToView;
		Dictionary<bool[], GameObject> viewMap;
		
		bool[] xNearValues = new []{false,false,false,false};
		
		public void StartIt()
		{
			cellToView = new Dictionary<Vector2Int, GameObject>();

			UpdateNewCells();
			roadLayer.Update += UpdateNewCells;
		}

		void UpdateNewCells()
		{
			foreach (var cell in roadLayer.CellsToUpdate)
				UpdateCellAndNear(cell);
		}

		void UpdateCellAndNear(Vector2Int cell)
		{
			UpdateCell(cell);

			for (int i = 0; i < 4; i++)
			{
				Vector2Int nearCell = GameGrid.nearOffsets[i] + cell;
				UpdateCell(nearCell);
			}
		}

		void UpdateCell(Vector2Int cell)
		{
			RemoveView(cell);
			if (!roadLayer.GetValue(cell))
				return;

			for (int i = 0; i < 4; i++)
			{
				xNearValues[i] = roadLayer.GetValue(GameGrid.nearOffsets[i] + cell);
			}

			var viewInfo = roadViewMap.GetPrefab(xNearValues);

			AddView(cell, viewInfo.prefab, viewInfo.spawnAngle);
		}

		void AddView(Vector2Int cell, GameObject prefab, float angle)
		{
			RemoveView(cell);
			
			GameObject newCellView = Instantiate(prefab, gameGrid.CellToWorldPoint(cell), Quaternion.Euler(0, angle, 0), transform);
			newCellView.SetActive(true);
			cellToView[cell] = newCellView; 
		}

		void RemoveView(Vector2Int cell)
		{
			var view = cellToView.GetOrDefault(cell, null);
			if (view)
			{
				Destroy(view);
				cellToView.Remove(cell);
			}
		}


		// public void Add(Vector2 cell)
		// {
		// 	cells[cell] = true;
		//
		// 	GameObject cellView = view.GetOrDefault(cell, null);
		// 	if (!cellView)
		// 	{
		// 		GameObject newCellView = Instantiate(prefab, grid.CellToWorldPoint(cell), Quaternion.identity, transform);
		// 		newCellView.SetActive(true);
		// 		view[cell] = newCellView;
		// 	}
		// }
		//
		// public void Remove(Vector2 cell)
		// {
		// 	cells[cell] = false;
		// 	GameObject cellView =  view.GetOrDefault(cell, null);
		// 	if (cellView)
		// 	{
		// 		Destroy(cellView);
		// 		view.Remove(cell);
		// 	}
		// }
	}
}