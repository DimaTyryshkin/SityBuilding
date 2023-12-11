using System;
using GamePackages.Core;
using UnityEngine;

namespace Game
{
	public class RoadBuilderInputMode : MonoBehaviour
	{
		[SerializeField] Camera thisCamera;
		[SerializeField] RoadLayer roadLayer;
		[SerializeField] GameGrid grid;
		[SerializeField] GameObject pointMarker;
		[SerializeField] GameObject cellMarker;


		Vector2Int lastCell;
		
		void LateUpdate()
		{
			Vector3 worldPoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
			Vector2Int cell = grid.WorldPointToCell(worldPoint);
			pointMarker.transform.position = worldPoint;
			cellMarker.transform.position = grid.CellToWorldPoint(cell);

			lastCell = cell;
			
			if (Input.GetMouseButton(0))
			{ 
				roadLayer.SetValue(cell, true);
			}
		    
			if (Input.GetMouseButton(1))
			{ 
				roadLayer.SetValue(cell, false);
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(10,10,20,40), $"{lastCell.x} {lastCell.y}");
		}
	}
}