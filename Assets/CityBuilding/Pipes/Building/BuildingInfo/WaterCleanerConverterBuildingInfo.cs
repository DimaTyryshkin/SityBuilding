using System.Collections.Generic;
using Game.Json;

namespace Game.Building
{
	public sealed class WaterCleanerConverterBuildingInfo : ConverterBuildingInfo
	{
		protected override List<ItemConverterJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.waterCleanerConverters;
		}
	}
}