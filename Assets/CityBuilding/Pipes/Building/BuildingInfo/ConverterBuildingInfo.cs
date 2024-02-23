using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public abstract class ConverterBuildingInfo : BuildingInfo<ItemConvert, ItemConverterJson>
	{
		[SerializeField] string inItemName;
		[SerializeField] string outItemName;
		[SerializeField] Vector2Int inOffset;
		[SerializeField] Vector2Int outOffset;
		[SerializeField, IsntNull] SourceBuildingInfo sources;
		[SerializeField, IsntNull] DestinationBuildingInfo destinations;
		
		  
		protected override ItemConverterJson PrintToJson(ItemConvert item)
		{
			return new ItemConverterJson()
			{ 
				cell = item.Cell, 
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
				destinations.idToItem[json.inDestinationId],
				sources.idToItem[json.outSourceId]);
		}
 
		public ItemConvert InstantiateNew(Vector2Int cell, MapBuilder mapBuilder)
		{
			//"water-cleaner","dirt-water", "clean-water"
			var outSourceCell = cell + outOffset;
			PipeItemSource outSource = sources.InstantiateNewSource(outSourceCell, outItemName, mapBuilder);

			var inDestinationCell = cell + inOffset;
			PipeItemDestination inDestination = destinations.InstantiateNewDestination(inDestinationCell, inItemName, mapBuilder);
			
			
			var item = InstantiateAsNew(cell, mapBuilder);
			item.Init(inDestination, outSource);
			return item;
		}
	}
}