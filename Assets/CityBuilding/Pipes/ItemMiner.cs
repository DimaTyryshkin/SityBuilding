using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class ItemMiner : MapElement
	{
		[SerializeField] StatusBar statusBar;
		[SerializeField] float spawnPeriod = 1;
		
		public string itemName;
		public PipeItemSource pipeItemSource;

	

		float timeNextSpawn;

		public void Init(Vector2Int cell, string resourceName, PipeItemSource pipeItemSource)
		{
			Assert.IsNotNull(pipeItemSource);
			
			Cell = cell;
			cells = new List<Vector2Int>()
			{
				cell + new Vector2Int(1, 0),
				cell + new Vector2Int(0, 1),
				cell + new Vector2Int(1, 1),
			};
			
			itemName = resourceName;
			this.pipeItemSource = pipeItemSource;
		}
		
		void Update()
		{
			if (Time.time > timeNextSpawn)
			{
				if (pipeItemSource.IncrementAmount())
				{
					timeNextSpawn = Time.time + spawnPeriod;
				}
				else
				{
					statusBar.SetLock();
				}
			}
			else
			{
				statusBar.SetProgress((timeNextSpawn - Time.time) / spawnPeriod);
			}
		}
		 
		public override void Delinking()
		{
			 
		}

		public static Vector2Int GetOutSourceCell(Vector2Int mainCell)
		{
			return mainCell + new Vector2Int(2, 0);
		}

		public static void GetAllCellsNotAllocate(Vector2Int mainCell, ref Vector2Int[] array)
		{
			array[0] = mainCell;
			array[1] = mainCell + new Vector2Int(1, 0);
			array[2] = mainCell + new Vector2Int(0, 1);
			array[3] = mainCell + new Vector2Int(1, 1);
			array[4] = GetOutSourceCell(mainCell);
		}
	}
}