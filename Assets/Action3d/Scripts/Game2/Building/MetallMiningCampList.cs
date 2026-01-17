using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    class MetallMiningCampList : BuildingList<MetallMiningCampBuilding, MetallMiningCampJson>
    {
        protected override MetallMiningCampJson PrintToJson(MetallMiningCampBuilding building)
        {
            return new MetallMiningCampJson()
            {
                cell = building.actualCell,
                roration = 0
            };
        }

        protected override List<MetallMiningCampJson> GetJsonList(MapJson mapJson) => mapJson.metallMiningCamp;

        protected override void RemoveInternal(MetallMiningCampBuilding building) { }

        protected override void InitAfterInstFormSave(MetallMiningCampBuilding building, Vector3Int cell, MetallMiningCampJson json, GridContent mapBuilder) { }
    }
}