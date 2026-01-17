using UnityEngine;

namespace Game2.Building
{
    class MiningCampBrush : PointBrush
    {
        readonly CellMarkerValue markerValue;

        public MiningCampBrush(
            BrushInject inject,
            BuildingListBase buildingList,
            CellMarkerValue reqieredMarker)
            : base(inject, buildingList)
        {
            markerValue = reqieredMarker;
        }

        protected override bool CanBuild(Vector3Int cell, BuildingBase buildingFantom)
        {
            if (!base.CanBuild(cell, buildingFantom))
                return false;

            return inject.gridContent.GetCellMarker(cell) == markerValue;
        }
    }
}