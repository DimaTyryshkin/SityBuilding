using System;
using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Events;

namespace SquareWorldGenerator
{
	public class Grid : MonoBehaviour
	{
		[SerializeField] Square squarePrefab;
		[SerializeField] Square squareSelection;
		[SerializeField] Transform root;
		[SerializeField] Camera thisCamera;

		public int width;
		public int height;

		public int Count => width * height;
		public bool IsInit { get; private set; }
		public Vector2Int SelectedCell { get; private set; }

		public event UnityAction Click;

		Square[] squares;

		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			GizmosExtension.DrawRect(new Rect(0,0,width,height));
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPoint = thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XY);
				int x = (int)worldPoint.x;
				int y = (int)worldPoint.y;
				SelectedCell = new Vector2Int(x, y);
				squareSelection.transform.position = CellToPos(SelectedCell);
				squareSelection.gameObject.SetActive(true);
				Click?.Invoke();
			}

			if (Input.GetMouseButtonDown(1))
				HideSelection();
		}

		public void HideSelection()
		{
			squareSelection.gameObject.SetActive(false);
		}

		int GetIndex(Vector2Int a)
		{
			return a.y * width + a.x;
		}
        
		int GetIndex(int x, int y)
		{
			return y * width + x;
		}

		public Vector2Int GetCell(int index)
		{
			int x = index % width;
			int y = index / width;

			return new Vector2Int(x, y);
		}

		Vector2 CellToPos(Vector2Int cell)
		{
			return cell + new Vector2(0.5f, 0.5f);
		}

		public Square GetLoop(Vector2Int cell)
		{
			cell.x = MathExtension.Residual(cell.x, width);
			cell.y = MathExtension.Residual(cell.y, height);
			return squares[GetIndex(cell)];
		}
		
		public Vector2Int LoopCell(Vector2Int cell)
		{
			cell.x = MathExtension.Residual(cell.x, width);
			cell.y = MathExtension.Residual(cell.y, height);
			return cell;
		}
		
		public (int, int) LoopCell(int x, int y)
		{
			int outX = MathExtension.Residual(x, width);
			int outY = MathExtension.Residual(y, height);
			return (outX, outY);
		}
		
		public Square Get(int index)
		{
			return squares[index];
		}
        
		public Square Get(int x, int y)
		{
			return squares[GetIndex(x,y)];
		}


		public IEnumerable<MaskOverlay> Mask(int maskRank, int cellX, int cellY)
		{ 
			int r = (maskRank - 1) / 2; 
			for (int maskX = 0; maskX < maskRank; maskX++)
			for (int maskY = 0; maskY < maskRank; maskY++)
			{
				(int x, int y) = LoopCell(cellX - r + maskX, cellY - r + maskY);
				yield return new MaskOverlay(x, y, maskX, maskY);
			}
		}

		public void Init()
		{
			if (IsInit)
			{
				Clear();
				return;
			}
			else
			{
				IsInit = true;
			}


			squares = new Square[width * height];
			for (int i = 0; i < width * height; i++)
			{
				Vector2Int cell = GetCell(i);
				squares[i] = Instantiate(squarePrefab, CellToPos(cell), Quaternion.identity, root);
				squares[i].Type = 0;
				squares[i].GrayValue = 0;
				squares[i].gameObject.SetActive(true);
			}
		}

		public void Clear()
		{ 
			for (int i = 0; i < width * height; i++)
			{ 
				squares[i].Type = 0; 
				squares[i].GrayValue = 0; 
			}
		}

		public void RemoveObjects()
		{
			IsInit = false;
			root.DestroyChildren();
		}
	}

	public struct MaskOverlay
	{
		public int x;
		public int y;
		public int maskX;
		public int maskY;

		public MaskOverlay(int x, int y, int maskX, int maskY)
		{
			this.x = x;
			this.y = y;
			this.maskX = maskX;
			this.maskY = maskY;
		}
	}
}