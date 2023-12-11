using System.Collections.Generic;
using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public sealed class ConverterBuildingInfo : BuildingInfo<ItemConvert, ItemConverterJson>
	{
		[SerializeField, IsntNull] SourceBuildingInfo sources;
		[SerializeField, IsntNull] DestinationBuildingInfo destinations;
		
		protected override List<ItemConverterJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.converters;
		}
 
		protected override ItemConverterJson PrintToJson(ItemConvert item)
		{
			return new ItemConverterJson()
			{ 
				cell = item.Cell,
				converterType = item.converterType,
				inDestinationId = destinations.itemToId[item.inDestination],
				outSourceId = sources.itemToId[item.outSource]
			};
		}
 
		protected override void RemoveInternal(ItemConvert value)
		{
			sources.Remove(value.outSource);
			destinations.Remove(value.inDestination);
		}

		protected override void InitAfterInstFormSave(ItemConvert item, Vector2Int cell, ItemConverterJson json, MapBuilder mapBuilder)
		{
			item.Init(
				cell,
				json.converterType,
				destinations.idToItem[json.inDestinationId],
				sources.idToItem[json.outSourceId]);
		}
 
		public ItemConvert InstantiateNewConverter(Vector2Int cell, string converterType, string inItemName, string outItemName, MapBuilder mapBuilder)
		{
			//"water-cleaner","dirt-water", "clean-water"
			var outSourceCell = cell + new Vector2Int(2, 0);
			PipeItemSource outSource = sources.InstantiateNewSource(outSourceCell, outItemName, 0, 1, mapBuilder);

			var inDestinationCell = cell +  new Vector2Int(-1, 0);
			PipeItemDestination inDestination = destinations.InstantiateNewDestination(inDestinationCell, inItemName,0, 1, mapBuilder);
			
			
			var item = InstantiateAsNew(cell, mapBuilder);
			item.Init(cell, converterType,inDestination, outSource);
			return item;
		}
	}
}