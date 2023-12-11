using System.Collections.Generic;
using System.Linq;
using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public sealed class CrossBuildingInfo : BuildingInfo<PipeCross, CrossJson>
	{
		[SerializeField, IsntNull] PipeBuildingInfo pipes;
		
		protected override List<CrossJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.cross;
		}
 
		protected override CrossJson PrintToJson(PipeCross item)
		{
			return new CrossJson()
			{ 
				cell = item.Cell,
				inPipes =  item.inPipes.Select(p => pipes.itemToId[p]).ToList(),
				outPipes = item.outPipes.Select(p => pipes.itemToId[p]).ToList(),
			};
		}
 
		protected override void RemoveInternal(PipeCross value)
		{ 
		}

		protected override void InitAfterInstFormSave(PipeCross item, Vector2Int cell, CrossJson json, MapBuilder mapBuilder)
		{
			item.Init();
			
			foreach (int pipeId in json.inPipes)
				item.AddInPipe(pipes.idToItem[pipeId]);

			foreach (int pipeId in json.outPipes)
				item.AddOutPipe(pipes.idToItem[pipeId]);
		} 
		
		public PipeCross InstantiateNew(Vector2Int cell, MapBuilder mapBuilder)
		{
			var newItem = InstantiateAsNew(cell, mapBuilder);
			newItem.Init();

			return newItem;
		}
	}
}