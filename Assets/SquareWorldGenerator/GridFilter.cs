using NaughtyAttributes;
using UnityEngine;

namespace SquareWorldGenerator
{
	public class GridFilter : MonoBehaviour
	{
		[SerializeField] Grid grid;  
		[SerializeField] SquareWorldGenerator  squareWorldGenerator;  
		[SerializeField] int r = 3;


		[Button()]
		void Apply()
		{
			squareWorldGenerator.StopAutoTick();
			
			float[,] values =  FilterGrid();
            
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				float a = 1f - values[x, y];
				Color color = new Color(a, a, a);
				grid.Get(x, y).SetColor(color);
			} 
		}
		
		[Button()]
		void Revert()
		{ 
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				var square = grid.Get(x, y);
				square.Type = square.Type;
			} 
		}

		float[,] FilterGrid()
		{
			float[,] values = new float[grid.width, grid.height];

			float min = float.MaxValue;
			float max = 0;
            
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				float filterValue = Filter(new Vector2Int(x, y), r);
				values[x, y] = filterValue;
				max = Mathf.Max(max, filterValue);
				min = Mathf.Min(min, filterValue);
			}
            
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				values[x, y] = (values[x, y] - min) / (max - min);
			}

			return values;
		}

		float Filter(Vector2Int cell, int r)
		{
			float sum = 0;
            
			for (int x = -r; x <= r; x++)
			for (int y = -r; y <= r; y++)
			{
				var offset = new Vector2Int(x, y);
				float dist = offset.magnitude;
				if (dist <= r)
				{
					float value = grid.GetLoop(cell + offset).Type;
					//sum += value * (Mathf.Cos((-dist / r) * Mathf.PI * 0.5f)); // График смотри тут https://yotx.ru/#!1/3_h/sH@zv7Rgzhf23/aP9gf2vfiCH8r@1v/P79b2zugbcgZ6eI3d39g30SDbuxc8p4PN1iPG5dXuzub@0DBg==
					sum += value * (1 - dist / r);
				}
			}

			return sum;
		}
	}
}