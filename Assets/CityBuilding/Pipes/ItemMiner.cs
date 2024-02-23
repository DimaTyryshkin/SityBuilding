using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class ItemMiner : MapElement
	{ 
		[SerializeField] float spawnPeriod = 1;
		
		[NonSerialized] public string itemName;
		[NonSerialized] public PipeItemSource pipeItemSource;

	

		float timeNextSpawn;

		public void Init(string resourceName, PipeItemSource pipeItemSource)
		{
			Assert.IsNotNull(pipeItemSource); 
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
					//statusBar.SetLock();
				}
			}
			else
			{
				//statusBar.SetProgress((timeNextSpawn - Time.time) / spawnPeriod);
			}
		}
		 
		public override void Delinking()
		{
			 
		}

		public static Vector2Int GetOutSourceCell(Vector2Int mainCell)
		{
			return mainCell + new Vector2Int(2, 0);
		} 
	}
}