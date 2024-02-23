using Game.Json;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.Building
{
	public class BrushBuilder
	{
		public readonly Camera thisCamera;
		public readonly MapBuilder mapBuilder;
		public readonly GameGrid grid;
		public readonly GameObject cellMarkerAvailable; 
		public readonly GameObject cellMarkerLock; 
		public readonly GuiHit guiHit; 
		
		public BrushBuilder( 
			Camera thisCamera,
			MapBuilder mapBuilder,
			GameGrid grid,
			GameObject cellMarkerAvailable,
			GameObject cellMarkerLock,
			GuiHit guiHit
		)
		{
			Assert.IsNotNull(thisCamera);
			Assert.IsNotNull(mapBuilder);
			Assert.IsNotNull(grid);
			Assert.IsNotNull(cellMarkerAvailable);
			Assert.IsNotNull(cellMarkerLock);
			Assert.IsNotNull(guiHit);
 
			this.thisCamera = thisCamera;
			this.mapBuilder = mapBuilder;
			this.grid = grid;
			this.cellMarkerAvailable = cellMarkerAvailable;
			this.cellMarkerLock = cellMarkerLock;
			this.guiHit = guiHit; 
		}
	}

	public class PointBrush : BuildingBrush
	{ 
		readonly BrushBuilder data;
		readonly PipeBrush pipeBrush;

		CellCast cast;
		readonly Vector2Int[] cellsMask;
		readonly Vector2Int[] cells;
		 
		public event UnityAction<Vector2Int> Build;
 

		public PointBrush( 
			PipeBrush pipeBrush,
			BrushBuilder brushBuilder,
			Vector2Int[] cellsMask)
		{
			Assert.IsNotNull(cellsMask);
			Assert.IsNotNull(pipeBrush);
			this.cellsMask = cellsMask;
			this.pipeBrush = pipeBrush;
			cells = new Vector2Int[cellsMask.Length];
			
			data = brushBuilder; 
			cast = new CellCast();
		}

		public override void LateUpdate()
		{
			if(data.guiHit.IsGuiUnderPointer)
				return;
			
			Vector3 worldPoint = data.thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
			Vector2Int cell = data.grid.WorldPointToCell(worldPoint);
			for (int i = 0; i < cells.Length; i++)
				cells[i] = cellsMask[i] + cell;
			
			Vector3 pos = data.grid.CellToWorldPoint(cell);
			data.cellMarkerAvailable.transform.position = pos;
			data.cellMarkerLock.transform.position = pos;



			bool isFree = true;
			foreach (var c in cells)
			{
				data.mapBuilder.CastNonAllocate(c, ref cast);
				if (!cast.IsFree)
				{
					isFree = false;
					break;
				}
			}

			
			//data.cellMarkerAvailable.SetActive(isFree);
			//data.cellMarkerLock.SetActive(!isFree);
			
			if (isFree)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Build(cell);
				}
			}
			 

			if (Input.GetMouseButton(1))
				Remove(cells);
		}
 
		void Remove(Vector2Int[] cells)
		{
			foreach (var cell in cells)
			{
				data.mapBuilder.CastNonAllocate(cell, ref cast);
				foreach (var entity in cast.entities)
				{
					if (entity is PipeItemSource)
						continue;
					
					if (entity is PipeItemDestination)
						continue;

					if (entity is PipeBaseElement)
						pipeBrush.RemoveCell(cell);
					
					data.mapBuilder.Remove(entity);
				}
			}
		} 
	}
}