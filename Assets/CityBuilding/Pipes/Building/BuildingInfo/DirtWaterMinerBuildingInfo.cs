using System.Collections.Generic;
using Game.Json;

namespace Game.Building
{
	public sealed class DirtWaterMinerBuildingInfo : MinerBuildingInfo
	{
		protected override List<ItemMinerJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.dirtWaterMiners;
		}
	}
}