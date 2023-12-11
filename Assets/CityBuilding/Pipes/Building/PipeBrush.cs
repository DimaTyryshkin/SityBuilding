using System;
using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.Building
{
	public class PipeBrush : BuildingBrush
	{
		[SerializeField, IsntNull] PipeBuildingInfo pipes;
		[SerializeField, IsntNull] CrossBuildingInfo cross;
		[SerializeField, IsntNull] Camera thisCamera;
		[SerializeField, IsntNull] MapBuilder mapBuilder;
		[SerializeField, IsntNull] GameGrid grid;
		//[SerializeField] GameObject pointMarker;
		[SerializeField, IsntNull] GameObject cellMarker;
		[SerializeField, IsntNull] GuiHit guiHit;

		Vector2Int startCell;
		Vector2Int endCell; 

		CellCast cast;
		List<Vector2Int> inputLine;
		List<Vector2Int> line;
 
		void Start()
		{  
			cast = new CellCast();
			inputLine = new List<Vector2Int>();
			line = new List<Vector2Int>();
		}

		void LateUpdate()
		{
			if(guiHit.IsGuiUnderPointer)
				return;
			
			Vector3 worldPoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
			Vector2Int cell = grid.WorldPointToCell(worldPoint);
			//pointMarker.transform.position = worldPoint;
			cellMarker.transform.position = grid.CellToWorldPoint(cell);
 
			
			if (Input.GetMouseButtonDown(0))
			{
				startCell = cell;
			}
			
			if (Input.GetMouseButton(0))
			{  
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				endCell = cell;
				ProcessBuildCommand(startCell, endCell);
			}
		    
			if (Input.GetMouseButton(1))
			{
				RemoveCell(cell);
			}
		}

		public void RemoveCell(Vector2Int cell)
		{
			mapBuilder.CastNonAllocate(cell, ref cast);
			if(cast.IsFree)
				return;
			
			MapElement entity = cast.entities[^1];
			if (entity)
			{
				if (entity is PipeQueue pipe)
				{
					RemoveCellFromPipe(pipe, cell);
				}
				else
				{ 
					mapBuilder.Remove(entity);
				}
			}
		}

		void ProcessBuildCommand(Vector2Int from, Vector2Int to)
		{ 
			if(from == to)
				return;

			line.Clear();
			GetCellLine(from, to, ref inputLine);

			Vector2Int cell = from;
			
			MapElement fromEntity = null;
			MapElement entity = null;
			MapElement lastEntity = null;
			for (int i = 0; i < inputLine.Count; i++)
			{
				bool isLastCell = i == inputLine.Count - 1;
				mapBuilder.CastNonAllocate(inputLine[i], ref cast);
				entity = cast.IsFree ? null : cast.entities[0];
				
				line.Add(inputLine[i]);
				if (line.Count == 1)
				{
					fromEntity = entity;
				}
				else
				{
					if (lastEntity == entity)
					{
						if (entity is PipeQueue oldPipe)
						{ 
							// Если ввод игрока прошел по двум подряд идущим клеткам старой трубы, то новая получается поверх вдоль старой. Так нельзя.
							if(IsPineGoThruPoints(inputLine[i - 1],inputLine[i - 0], oldPipe))
								break;
						}
					} 

					bool isPipe = entity is PipeQueue;
					if (isLastCell && isPipe)
					{
						BuildSegment(line, fromEntity, entity);
						break;
					}

					bool notFreeAndNotPipe = entity && !isPipe; 
					if (notFreeAndNotPipe)
					{ 
						bool success = BuildSegment(line, fromEntity, entity);
						if(!success)
							break;
						
						line.Clear();
						lastEntity = null;
						entity = null;
						i--;
					} 
				}

				if (isLastCell && line.Count > 1)
				{
					BuildSegment(line, fromEntity, entity);
					break;
				}

				lastEntity = entity;
			}  
		}

		bool BuildSegment(List<Vector2Int> cells, MapElement from, MapElement to, bool isInverse = false)
		{
			// Варианты ввода 
			// 1 из пустой клетки в пустую = создание
			// - из пустой в начало трубы = создание + объединений
			// - из пустой в конец трубы = создание + объединение
			// - из пустой в середину = создание + разрыв + создание пееркрестка (+ наоборот)
			// - из пустой в перекресток = создание + подсоединение к перекрестку (+ наоборот)
			// - из занятой в занятую = создание + создание двух перекрестков + два разрыва
 
			EditorAction fromAction = new EditorAction();
			PipeQueue pipe = null;
			// --- FROM
			if (!from)
			{
				Debug.Log("From Empty");
				fromAction.isAvailable = true;
				fromAction.action = () => pipe = pipes.InstantiateNew(cells.ToList(), mapBuilder);
			}

			if (from is PipeQueue oldPipe)
			{  
				bool addInTail = oldPipe.cells[^1] == cells[0]; 
				bool addInHead = oldPipe.cells[0] == cells[0];
				bool isMiddle = !addInHead && !addInTail;
				 
				if (addInTail)
				{
					Debug.Log("From Tail");
					fromAction.isAvailable = true;
					fromAction.action = () =>
					{
						pipe = oldPipe;

						for (int i = 1; i < cells.Count; i++)
						{
							oldPipe.cells.Add(cells[i]);
							oldPipe.OnChanged();
						}
					};
				} 
				
				if(isMiddle)
				{
					Debug.Log("From Middle");
					fromAction.isAvailable = true;
					fromAction.action = () =>
					{
						var newPipe = SplitPipe(oldPipe, cells[0]);
						var newCross = cross.InstantiateNew(cells[0], mapBuilder);
						newCross.AddInPipe(oldPipe);
						newCross.AddOutPipe(newPipe);
						pipe = pipes.InstantiateNew(cells.ToList(), mapBuilder);
						newCross.AddOutPipe(pipe);
						
						oldPipe.OnChanged();
						newPipe.OnChanged();
						pipe.OnChanged();
					};
				}
			}
			
			if (from is IPipeSource pipeSource)
			{
				Debug.Log("From Source"); 
				fromAction.isAvailable = true;
				fromAction.action = () =>
				{
					pipe = pipes.InstantiateNew(cells.ToList(), mapBuilder);
					pipeSource.AddOutPipe(pipe);
					pipe.OnChanged();
				};
			}

			if (!fromAction.isAvailable)
			{
				// Если начинаем строить с места, которой должно быть концом, то новую часть(ввод игрока) переварачиваем наоборот

				if (isInverse)
				{
					return false;
				}
				else
				{
					cells.Reverse();
					return BuildSegment(cells, to, from, isInverse = true);
				}
			}


			//--- TO
			EditorAction toAction = new EditorAction();
			
			if (!to)
			{
				Debug.Log("To Empty");
				toAction.isAvailable = true;
				toAction.action = () => { };
			}
			
			if (to is IPipeDestination destination)
			{
				Debug.Log("To Destination");
				toAction.isAvailable = true;
				toAction.action = () =>
				{
					destination.AddInPipe(pipe);
				};
			}

			if (to is PipeQueue oldPipe2)
			{
				bool addInTail = oldPipe2.cells[^1] == cells[^1]; 
				bool addInHead = oldPipe2.cells[0] == cells[^1];
				bool isMiddle = !addInHead && !addInTail;
				
				if (addInHead)
				{
					Debug.Log("To Head");
					toAction.isAvailable = true;
					toAction.action = () =>
					{
						for (int i = 1; i < oldPipe2.cells.Count; i++)
							pipe.cells.Add(oldPipe2.cells[i]);

						var outElement = oldPipe2.outElement;
						pipes.Remove(oldPipe2);
						outElement?.AddInPipe(pipe);
						pipe.OnChanged();
					};
				}
				
				if(isMiddle)
				{
					Debug.Log("To Middle");
					toAction.isAvailable = true;
					toAction.action = () =>
					{
						var newPipe = SplitPipe(oldPipe2, cells[^1]);
						var newCross = cross.InstantiateNew(cells[^1], mapBuilder);
						newCross.AddInPipe(oldPipe2);
						newCross.AddOutPipe(newPipe); 
						newCross.AddInPipe(pipe);
					};
				}
			}

			if (toAction.isAvailable)
			{
				fromAction.action.Invoke();
				toAction.action.Invoke();
				return true;
			}
			else
			{
				if (isInverse)
				{
					return false;
				}
				else
				{
					cells.Reverse();
					return BuildSegment(cells, to, from, isInverse = true);
				}
			} 
		}
 
		PipeQueue SplitPipe(PipeQueue pipe, Vector2Int cell)
		{
			Assert.IsTrue(cell != pipe.EndCell);
			Assert.IsTrue(cell != pipe.StartCell);
			
			List<Vector2Int> first = new List<Vector2Int>(pipe.cells.Count);
			List<Vector2Int> second = new List<Vector2Int>(pipe.cells.Count);


			List<Vector2Int> actual = first;
			foreach (var c in pipe.cells)
			{
				actual.Add(c);

				if (c == cell && actual == first)
				{
					actual = second;
					actual.Add(c);
				}
			}

			Assert.IsTrue(first.Count > 1 && second.Count > 1);
			pipe.cells = first;
			var newPipe = pipes.InstantiateNew(second, mapBuilder);

			var outElement = pipe.outElement;
			if (outElement != null)
			{
				outElement.RemovePipe(pipe);
				outElement.AddInPipe(newPipe);
			}

			pipe.OnChanged();
			return newPipe;
		}
		
		void RemoveCellFromPipe(PipeQueue pipe, Vector2Int cell)
		{
			List<Vector2Int> first = new List<Vector2Int>(pipe.cells.Count);
			List<Vector2Int> second = new List<Vector2Int>(pipe.cells.Count);

			List<Vector2Int> actual = first;
			foreach (var c in pipe.cells)
			{

				if (c != cell)
				{
					actual.Add(c);
				}
				else
				{
					if(actual == first)
						actual = second;
				}
			}

			var inElement = pipe.inElement;
			var outElement = pipe.outElement;
			if (first.Count < 2)
			{
				pipes.Remove(pipe);
			}
			else
			{
				pipe.cells = first;
				outElement?.RemovePipe(pipe);
				pipe.OnChanged();
			}

			if (second.Count >= 2)
			{
				PipeQueue newPipe = pipes.InstantiateNew(second, mapBuilder);
				outElement?.AddInPipe(newPipe);
				newPipe.OnChanged();
			}
		}

		void GetCellLine(Vector2Int from, Vector2Int to, ref List<Vector2Int> inputLine)
		{
			inputLine.Clear();

			inputLine.Add(from);
			if(from == to)
				return;
			
			Vector2Int cell = from;
			while (cell != to)
			{
				int deltaX = to.x - cell.x;
				int deltaY = to.y - cell.y;

				if (Math.Abs(deltaX) > Math.Abs(deltaY))
				{
					cell.x += Math.Sign(deltaX);
				}
				else
				{
					cell.y += Math.Sign(deltaY);
				} 
				
				inputLine.Add(cell);
			} 
		}

		/// <summary>
		/// Содержит ли Pipe две подряд идущие клетки
		/// </summary>
		bool IsPineGoThruPoints(Vector2Int cell1, Vector2Int cell2, PipeQueue pipe)
		{
			for (int k = 1; k < pipe.cells.Count; k++)
			{
				var c1 = pipe.cells[k - 1];
				var c2 = pipe.cells[k - 0];

				if (c1 == cell1)
					if (c2 == cell2)
						return true;

				if (c1 == cell2)
					if (c2 == cell1)
						return true;
			}

			return false;
		}


		
		
		struct EditorAction
		{
			public bool isAvailable;  
			public UnityAction action;
		}

	}
	
	
}