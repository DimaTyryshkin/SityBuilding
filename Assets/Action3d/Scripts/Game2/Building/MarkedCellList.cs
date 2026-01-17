using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
#endif

namespace Game2.Building
{
    enum CellMarkerValue
    {
        None = 0,
        MetallMiningCamp
    }

    class CellMarkerList
    {
        public readonly CellMarkerValue marker;
        public readonly IReadOnlyList<Vector3Int> cells;

        public CellMarkerList(CellMarkerValue marker, Vector3Int[] cells)
        {
            Assert.IsNotNull(cells);

            this.marker = marker;
            this.cells = cells;
        }
    }
}