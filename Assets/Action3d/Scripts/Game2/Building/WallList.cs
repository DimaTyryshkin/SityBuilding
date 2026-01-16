using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    public class WallList : BuildingList<WallBuilding, WallJson>
    {
        protected override WallJson PrintToJson(WallBuilding item)
        {
            return new WallJson()
            {
                cell = item.Cell,
                roration = 0
            };
        }

        protected override List<WallJson> GetJsonList(MapJson mapJson, GridContent mapBuilder) => mapJson.wallsA;

        protected override void RemoveInternal(WallBuilding building) { }

        protected override void InitAfterInstFormSave(WallBuilding building, Vector3Int cell, WallJson json, GridContent mapBuilder) { }
    }
}