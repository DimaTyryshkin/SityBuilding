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

    public class GridContent : MonoBehaviour
    {
        [SerializeField, IsntNull] Transform mapRoot;
        [SerializeField, IsntNull] GameGrid grid;
        [SerializeField, IsntNull] TextAsset json1;
        [SerializeField, IsntNull] TextAsset jsonTemp;

        BuildingListBase[] buildingsInfo;
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

        public void Init(BuildingListBase[] buildingsInfo)
        {
            AssertWrapper.IsAllNotNull(buildingsInfo);

            this.buildingsInfo = buildingsInfo;
            mapJson = new MapJson();
        }

        void LoadFromJson()
        {
            transform.DestroyChildren();

            foreach (var building in buildingsInfo)
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

            foreach (BuildingListBase info in buildingsInfo)
                info.CastNonAllocate(cell, ref result);
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
            foreach (var info in buildingsInfo)
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