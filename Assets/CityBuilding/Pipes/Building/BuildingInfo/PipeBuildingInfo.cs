using System.Collections.Generic;
using Game.Json;
using GamePackages.Core;
using UnityEngine;

namespace Game.Building
{
	public sealed class PipeBuildingInfo : BuildingInfo<PipeQueue, PipeJson>
	{
		protected override List<PipeJson> GetJsonList(MapJson mapJson, MapBuilder mapBuilder)
		{
			return mapJson.pipes;
		}

		protected override PipeJson PrintToJson(PipeQueue item)
		{
			return new PipeJson()
			{
				cells = item.cells
			};
		}

		protected override void RemoveInternal(PipeQueue value)
		{
			
		}

		protected override void InitAfterInstFormSave(PipeQueue item, Vector2Int cell, PipeJson json, MapBuilder mapBuilder)
		{
			item.Init(json.cells);
			
			var view = item.GetComponent<PipeQueueView>();
			view.Init(mapBuilder.Grid);
		}

		public PipeQueue InstantiateNew(List<Vector2Int> cells, MapBuilder mapBuilder)
		{
			var newItem = InstantiateAsNew(cells[0], mapBuilder);
			newItem.Init(cells);

			var view = newItem.GetComponent<PipeQueueView>();
			view.Init(mapBuilder.Grid);
			return newItem;
		}
	}
}