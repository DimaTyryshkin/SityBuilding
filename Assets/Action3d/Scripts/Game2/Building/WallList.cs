using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    class WallList : BuildingList<WallBuilding, WallJson>
    {
        protected override WallJson PrintToJson(WallBuilding building)
        {
            return new WallJson()
            {
                cell = building.actualCell,
                roration = 0
            };
        }

        protected override List<WallJson> GetJsonList(MapJson mapJson) => mapJson.wallsA;

        protected override void RemoveInternal(WallBuilding building) { }

        protected override void InitAfterInstFormSave(WallBuilding building, Vector3Int cell, WallJson json, GridContent mapBuilder) { }
    }
}