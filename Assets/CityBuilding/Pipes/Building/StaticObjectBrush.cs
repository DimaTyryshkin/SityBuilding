using Game.Json;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.Building
{  
	public class StaticObjectBrush : BuildingBrush
	{
		[SerializeField, IsntNull] Camera thisCamera;
		[SerializeField, IsntNull] PipeBrush pipeBrush;
		[SerializeField, IsntNull] MapBuilder mapBuilder;
		[SerializeField, IsntNull] GameGrid grid;
		[SerializeField, IsntNull] GameObject cellMarkerAvailable; 
		[SerializeField, IsntNull] GameObject cellMarkerLock; 
		[SerializeField, IsntNull] GuiHit guiHit;

		CellCast cast;
		Vector2Int[] cellsMask;
		Vector2Int[] cells;
		UnityAction<Vector2Int> buildAction;
  
		void LateUpdate()
		{
			if(guiHit.IsGuiUnderPointer)
				return;
			
			Vector3 worldPoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
			Vector2Int cell = grid.WorldPointToCell(worldPoint);
			for (int i = 0; i < cells.Length; i++)
				cells[i] = cellsMask[i] + cell;
			
			Vector3 pos = grid.CellToWorldPoint(cell);
			cellMarkerAvailable.transform.position = pos;
			cellMarkerLock.transform.position = pos;



			bool isFree = true;
			for (int i = 0; i < cells.Length; i++)
			{
				mapBuilder.CastNonAllocate(cells[i], ref cast);
				if (!cast.IsFree)
				{
					isFree = false;
					break;
				}
			}

			
			cellMarkerAvailable.SetActive(isFree);
			cellMarkerLock.SetActive(!isFree);
			
			if (isFree)
			{
				if (Input.GetMouseButtonDown(0))
				{
					buildAction(cell);
				}
			}
			 

			if (Input.GetMouseButton(1))
			{
				Remove(cells);
			}
		}

		public void Set(Vector2Int[] cellsMask, UnityAction<Vector2Int> build)
		{
			Assert.IsNotNull(cellsMask);
			Assert.IsNotNull(build);
			
			this.cellsMask = cellsMask;
			buildAction = build;
			cells = new Vector2Int[cellsMask.Length];
			cast = new CellCast();
		}
 
		void Remove(Vector2Int[] cells)
		{
			foreach (var cell in cells)
			{
				mapBuilder.CastNonAllocate(cell, ref cast);
				foreach (var entity in cast.entities)
				{
					if (entity is PipeItemSource)
						continue;
					
					if (entity is PipeItemDestination)
						continue;

					if (entity is PipeBaseElement)
						pipeBrush.RemoveCell(cell);
					
					mapBuilder.Remove(entity);
				}
			}
		}

		void Build(Vector2Int cell)
		{
			//mapBuilder.BuildItemConverterWithSources(cell, "water-cleaner","dirt-water", "clean-water");
		}
	}
}