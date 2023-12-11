using System.Collections.Generic;
using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public sealed class MinerBuildingInfo : BuildingInfo<ItemMiner, ItemMinerJson>
	{ 
		[SerializeField, IsntNull] SourceBuildingInfo sources;
		
		protected override List<ItemMinerJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.miners;
		}
 
		protected override ItemMinerJson PrintToJson(ItemMiner item)
		{
			return new ItemMinerJson()
			{ 
				cell = item.Cell,
				resourceName = item.itemName,
				pipeSourceId = sources.itemToId[item.pipeItemSource]
			};
		}
 
		protected override void RemoveInternal(ItemMiner value)
		{
			sources.Remove(value.pipeItemSource);
		}

		protected override void InitAfterInstFormSave(ItemMiner item, Vector2Int cell, ItemMinerJson json, MapBuilder mapBuilder)
		{
			item.Init(cell, json.resourceName, sources.idToItem[json.pipeSourceId]);
		}

		public void AfterInstAsNew(ItemMiner item, Vector2Int cell, MapBuilder mapBuilder)
		{
			string resourceName = "clean-water";
			Vector2Int sourceOffset = new Vector2Int(2, 0);
			var outSourceCell = cell + sourceOffset;

			PipeItemSource source = sources.InstantiateNewSource(outSourceCell, resourceName, 0, 1, mapBuilder);
			item.Init(cell, resourceName, source);
		}
	}
}