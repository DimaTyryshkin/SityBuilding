using GamePackages.Core;
using GamePackages.Core.Validation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    public enum BuildingType
    {
        None = 0,
        Pipe,
    }

    abstract class BuildingListBase : MonoBehaviour
    {
        public abstract BuildingBase Prefab { get; }

        public virtual void Init()
        {
            //Prefab.Init();
        }

        public abstract void ParseJson(MapJson mapJson, GridContent mapBuilder);
        public abstract void CastNonAllocate(Vector3Int cell, ref CellCast result);
        public abstract void PrintToJson(MapJson mapJson, GridContent mapBuilder, ref int actualId);
    }

    abstract class BuildingList<T, T2> : BuildingListBase
        where T : BuildingBase
        where T2 : BuildingJson
    {
        [SerializeField, IsntNull] protected T itemPrefab;
        [NonSerialized] protected List<T> allBuildings;

        public Dictionary<int, T> idToItem;
        public Dictionary<T, int> itemToId;
        public override BuildingBase Prefab => itemPrefab;

        public sealed override void Init()
        {
            base.Init();
            allBuildings = new List<T>();
            idToItem = new Dictionary<int, T>();
        }

        protected abstract List<T2> GetJsonList(MapJson mapJson);

        public sealed override void ParseJson(MapJson mapJson, GridContent mapBuilder)
        {
            allBuildings = new List<T>();
            idToItem = new Dictionary<int, T>();
            List<T2> itemsJson = GetJsonList(mapJson);
            foreach (var json in itemsJson)
                InstantiateFromSave(json, mapBuilder);
        }

        public sealed override void PrintToJson(MapJson mapJson, GridContent mapBuilder, ref int actualId)
        {
            itemToId = new Dictionary<T, int>();

            List<T2> itemsJson = GetJsonList(mapJson);
            foreach (var value in allBuildings)
            {
                int id = ++actualId;
                itemToId[value] = id;
                T2 json = PrintToJson(value);
                json.id = id;
                itemsJson.Add(json);
            }
        }

        protected abstract T2 PrintToJson(T building);

        public sealed override void CastNonAllocate(Vector3Int cell, ref CellCast result)
        {
            allBuildings.ToStringMultilineAndLog($"CastNonAllocate {gameObject.name}", x => x.name);
            foreach (T item in allBuildings)
            {
                foreach (Vector3Int c in item.actualCells)
                {
                    if (c == cell)
                    {
                        result.buildings.Add(item);
                        break;
                    }
                }
            }
        }

        public void Remove(T building)
        {
            RemoveInternal(building);

            allBuildings.Remove(building);
            building.Delinking();
            Destroy(building.gameObject);
        }

        protected abstract void RemoveInternal(T building);

        public T InstantiateAsNew(Vector3Int cell, GridContent gridContent)
        {
            int id = gridContent.GetFreeId();
            T newItem = SpawnPrefab(cell, gridContent);
            idToItem[id] = newItem;

            return newItem;
        }

        private T InstantiateFromSave(T2 json, GridContent mapBuilder)
        {
            var newItem = SpawnPrefab(json.Cell, mapBuilder);
            idToItem[json.id] = newItem;
            InitAfterInstFormSave(newItem, json.Cell, json, mapBuilder);
            return newItem;
        }

        private T SpawnPrefab(Vector3Int cell, GridContent mapBuilder)
        {
            T newBuilding = mapBuilder.MapRoot.InstantiateAsChild(itemPrefab, localScaleToOne: false);
            newBuilding.gameObject.SetActive(true);
            allBuildings.Add(newBuilding);
            newBuilding.Init();
            newBuilding.MoveToCell(cell);
            return newBuilding;
        }

        protected abstract void InitAfterInstFormSave(T building, Vector3Int cell, T2 json, GridContent mapBuilder);
    }
}