using System.Collections.Generic;
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
				pipe = item.pipeQueue? pipes.itemToId[item.pipeQueue] : 0,
				resourceName = item.itemName
			};
		}
 
		protected override void RemoveInternal(PipeItemSource value)
		{ 
		}

		protected override void InitAfterInstFormSave(PipeItemSource item, Vector2Int cell, SourceJson json, MapBuilder mapBuilder)
		{
			item.Init(json.resourceName);
		
			var pipe = pipes.idToItem.GetValueOrDefault(json.pipe);
			if(pipe)
				item.AddOutPipe(pipe);
		}

		public PipeItemSource InstantiateNewSource(Vector2Int cell, string resourceName, MapBuilder mapBuilder)
		{
			var item = InstantiateAsNew(cell, mapBuilder);
			item.Init(resourceName);
			return item;
		}
	}
}