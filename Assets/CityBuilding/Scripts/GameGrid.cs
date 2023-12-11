using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	public class GameGrid : MonoBehaviour
	{
		[SerializeField] bool is2d;
		
		public static Vector2Int[] nearOffsets = new[]
		{
			new Vector2Int(0, 1),
			new Vector2Int(1, 0),
			new Vector2Int(0, -1),
			new Vector2Int(-1, 0)
		};
		
		public static int [,] dirToAngle = new int[3,3]
		{
			{225,270,315},
			{180,000,000},
			{135,090,045},
		};

		
		[SerializeField] Vector2Int cellSize;

		public Vector2Int WorldPointToCell(Vector3 worldPoint)
		{
			int x = (int)Mathf.Floor(worldPoint.x);
			int y =is2d ? 
			(int)Mathf.Floor(worldPoint.y) :
			(int)Mathf.Floor(worldPoint.z);

			return new Vector2Int(x, y);
		}


		public Vector3 CellToWorldPoint(Vector2 cell)
		{
			float x = cell.x;
			float z = cell.y;
			if (is2d)
				return new Vector3(x + cellSize.x * 0.5f, z + cellSize.y * 0.5f, 0);
			else
				return new Vector3(x + cellSize.x * 0.5f, 0, z + cellSize.y * 0.5f);
		}


		public int GetAngle(Vector2Int dir)
		{
			return dirToAngle[dir.x + 1, dir.y + 1];
		}

		[Button()]
		void Print()
		{
			Debug.Log(GetAngle(new Vector2Int(0,1)));
			Debug.Log(GetAngle(new Vector2Int(1,0)));
		}
	}
}