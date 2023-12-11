using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	public class Car : MonoBehaviour
	{
		[SerializeField] RoadLayer road;
		[SerializeField] GameGrid gameGrid;
		[SerializeField] Transform target;
		[SerializeField] GameObject cargoMarker;
		[SerializeField] MeshRenderer cargoMaterial;
		[SerializeField] float  speed;

		Vector2Int lastCell1;
		Vector2Int lastCell2;


		Vector2Int[] cells = new Vector2Int[4]; 
		int[] distances = new int[4];
		bool isMove;

		public bool IsMove => isMove;

		public Vector2Int TargetCell { get; private set; }


		void Start()
		{
			//SetDebugTarget();
		}

		void Update()
		{
			if(!isMove)
				return;
			
			var cell = gameGrid.WorldPointToCell(transform.position);
			bool isOnRoad = road.GetValue(cell);
			
			if(!isOnRoad)
				return;
 
			for (int i = 0; i < 4; i++)
			{
				var c = cell + GameGrid.nearOffsets[i];
				cells[i] = c;

				distances[i] = int.MaxValue;

				if (road.GetValue(c))
				{
					if (c == lastCell2 || c == lastCell1)
						distances[i] = Int32.MaxValue - 1;
					else
						distances[i] = (TargetCell - c).sqrMagnitude;
				}
			} 
			
			
			
			int minValue = Int32.MaxValue;
			int minIndex = -1;

			for (int i = 0; i < 4; i++)
			{
				if (distances[i] < minValue)
				{
					minValue = distances[i];
					minIndex = i;
				}
			}

			if (minIndex >= 0)
			{
				Move(cells[minIndex]);
				if (Vector2Int.Distance(TargetCell, cell) < 2)
					isMove = false;
			}

			if (cell != lastCell1)
			{
				lastCell2 = lastCell1;
				lastCell1 = cell;
			}
		}
		 
		void Move(Vector2Int cell)
		{
			Vector3 cellPosition = gameGrid.CellToWorldPoint(cell);
			transform.position = Vector3.MoveTowards(transform.position, cellPosition, Time.deltaTime * speed);
		}

		public void SetTarget(Vector2Int target)
		{
			TargetCell = target;
			isMove = true;
		}

		public void SetCargoEnable(bool isCargoEnable, Color color)
		{
			cargoMarker.SetActive(isCargoEnable);

			cargoMaterial.material.color = color;
			//var mat = cargoMaterial.material;
			//mat.color = color;
			//cargoMaterial.material = mat;
		}

		Vector2Int Normalize(Vector2Int value)
		{
			return new Vector2Int(IntSign(value.x), IntSign(value.y));
		}

		int IntSign(int value)
		{
			if (value>0) return 1;
			if (value < 0) return -1;
			return 0;
		}

		[Button()]
		void SetDebugTarget()
		{
			SetTarget(gameGrid.WorldPointToCell(target.position));
		}
	}
}