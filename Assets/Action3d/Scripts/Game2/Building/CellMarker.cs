using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game2.Building
{
    class CellMarker : MonoBehaviour
    {
        public CellMarkerValue Marker;

        public Vector3Int Cell => GameGrid.WorldPointToCell(transform.position);

#if UNITY_EDITOR
        [Button]
        public void Alignment()
        {
            Undo.RecordObject(transform, "Align");
            GameGrid.AlignToGrid(transform);
        }
#endif
    }
}