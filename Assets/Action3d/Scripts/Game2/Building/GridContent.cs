using System.Collections.Generic;
using System.IO;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game2.Building
{
    public class CellCast
    {
        public bool IsFree => buildings.Count == 0;
        public List<BuildingBase> buildings = new List<BuildingBase>();
    }

    class GridContent : MonoBehaviour
    {
        [SerializeField, IsntNull] Transform mapRoot;
        [SerializeField, IsntNull] GameGrid grid;
        [SerializeField, IsntNull] TextAsset json1;
        [SerializeField, IsntNull] TextAsset jsonTemp;

        BuildingListBase[] buildingsLists;
        CellMarkerList[] cellMarkerLists;
        MapJson mapJson;
        int actualId;

        //Dictionary<int, PipeCross> idToCross = new Dictionary<int, PipeCross>();
        //Dictionary<int, PipeItemDestination> idToDestination = new Dictionary<int, PipeItemDestination>();

        public MapJson MapJson => mapJson;
        public Transform MapRoot => mapRoot;
        public GameGrid Grid => grid;

        public int GetFreeId()
        {
            return ++actualId;
        }

        public void Init(BuildingListBase[] buildingsLists, CellMarkerList[] cellMarkerLists)
        {
            AssertWrapper.IsAllNotNull(buildingsLists);
            AssertWrapper.IsAllNotNull(cellMarkerLists);

            this.cellMarkerLists = cellMarkerLists;
            this.buildingsLists = buildingsLists;
            mapJson = new MapJson();
        }

        void LoadFromJson()
        {
            transform.DestroyChildren();

            foreach (var building in buildingsLists)
            {
                building.ParseJson(mapJson, this);
            }
        }

        public void Remove(BuildingBase building)
        {
            //if(mapElement is ItemConvert converter)
            //	RemoveItemConverter(converter);
        }

        public void CastNonAllocate(Vector3Int cell, ref CellCast result)
        {
            result.buildings.Clear();

            foreach (BuildingListBase list in buildingsLists)
                list.CastNonAllocate(cell, ref result);
        }

        public CellMarkerValue GetCellMarker(Vector3Int cell)
        {
            foreach (CellMarkerList cellMarkerList in cellMarkerLists)
            {
                foreach (var c in cellMarkerList.cells)
                    if (cell == c)
                        return cellMarkerList.marker;
            }

            return CellMarkerValue.None;
        }

        void LoadFromJson(TextAsset jsonFile)
        {
#if UNITY_EDITOR
            string fileName = AssetDatabase.GetAssetPath(jsonFile);
            string json = File.ReadAllText(fileName);
            mapJson = JsonUtility.FromJson<MapJson>(json);
            LoadFromJson();
#endif
        }

        void PrintToJson(TextAsset jsonFile)
        {
#if UNITY_EDITOR
            this.mapJson = new MapJson();

            actualId = 0;
            foreach (var info in buildingsLists)
                info.PrintToJson(this.mapJson, this, ref actualId);

            string mapJson = JsonUtility.ToJson(this.mapJson, true);
            string fileName = AssetDatabase.GetAssetPath(jsonFile);
            Debug.Log(fileName);
            File.WriteAllText(fileName, mapJson);
#endif
        }


        [Button()]
        void LoadFromJson1()
        {
            LoadFromJson(json1);
        }


        [Button()]
        void SaveToJson1()
        {
            PrintToJson(json1);
        }

        [Button()]
        void LoadFromJsonTemp()
        {
            LoadFromJson(jsonTemp);
        }


        [Button()]
        void SaveToJsonTemp()
        {
            PrintToJson(jsonTemp);
        }
    }
}