using System.Collections.Generic;
using System.Linq;
using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public sealed class SourceBuildingInfo : BuildingInfo<PipeItemSource, SourceJson>
	{
		[SerializeField, IsntNull] PipeBuildingInfo pipes;
		
		protected override List<SourceJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.sources;
		}
 
		protected override SourceJson PrintToJson(PipeItemSource item)
		{
			return new SourceJson()
			{ 
				cell = item.Cell,
				amount = item.itemAmount,
				maxAmount = item.maxItemAmount,
				pipe = pipes.itemToId[item.pipeQueue],
				resourceName = item.itemName
			};
		}
 
		protected override void RemoveInternal(PipeItemSource value)
		{ 
		}

		protected override void InitAfterInstFormSave(PipeItemSource item, Vector2Int cell, SourceJson json, MapBuilder mapBuilder)
		{
			item.Init(cell, json.resourceName, json.amount, json.maxAmount);
				
			var pipe = pipes.idToItem.GetValueOrDefault(json.pipe);
			if(pipe)
				item.AddOutPipe(pipe);
		}

		public PipeItemSource InstantiateNewSource(Vector2Int cell, string resourceName, int amount, int maxAmount, MapBuilder mapBuilder)
		{
			var item = InstantiateAsNew(cell, mapBuilder);
			item.Init(cell, resourceName, amount, maxAmount);
			return item;
		}
	}
}