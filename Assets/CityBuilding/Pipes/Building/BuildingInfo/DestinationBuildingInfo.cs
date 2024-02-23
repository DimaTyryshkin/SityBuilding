using System.Collections.Generic;
using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public sealed class DestinationBuildingInfo : BuildingInfo<PipeItemDestination, DestinationJson>
	{
		[SerializeField, IsntNull] PipeBuildingInfo pipes;
		
		protected override List<DestinationJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.destinations;
		}
 
		protected override DestinationJson PrintToJson(PipeItemDestination item)
		{
			return new DestinationJson()
			{ 
				cell = item.Cell,
				amount = item.itemAmount,
				maxAmount = item.maxItemAmount,
				pipe =item.pipeQueue? pipes.itemToId[item.pipeQueue] : 0,
				resourceName = item.itemName
			};
		}
 
		protected override void RemoveInternal(PipeItemDestination value)
		{ 
		}

		protected override void InitAfterInstFormSave(PipeItemDestination item, Vector2Int cell, DestinationJson json, MapBuilder mapBuilder)
		{
			item.Init(json.resourceName, json.amount, json.maxAmount);
				
			var pipe = pipes.idToItem.GetValueOrDefault(json.pipe);
			if(pipe)
				item.AddInPipe(pipe);
		}
 
		public PipeItemDestination InstantiateNewDestination(Vector2Int cell, string resourceName, MapBuilder mapBuilder)
		{
			var item = InstantiateAsNew(cell, mapBuilder);
			item.Init(resourceName, 0, 1);
			return item;
		}
	}
}