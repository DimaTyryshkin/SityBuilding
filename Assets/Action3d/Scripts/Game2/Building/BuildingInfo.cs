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

    public abstract class BuildingListBase : MonoBehaviour
    {
        [SerializeField] Vector3Int[] cellsMask;

        public abstract BuildingBase Prefab { get; }

        public Vector3Int[] GetCellsMask()
        {
            return cellsMask;
        }

        public List<Vector3Int> CopyCellsMask(Vector3Int offset)
        {
            List<Vector3Int> newMask = new List<Vector3Int>(cellsMask.Length);

            for (int i = 0; i < cellsMask.Length; i++)
                newMask.Add(cellsMask[i] + offset);

            return newMask;
        }

        public abstract void Init();
        public abstract void ParseJson(MapJson mapJson, GridContent mapBuilder);
        public abstract void CastNonAllocate(Vector3Int cell, ref CellCast result);
        public abstract void PrintToJson(MapJson mapJson, GridContent mapBuilder, ref int actualId);
    }

    public abstract class BuildingList<T, T2> : BuildingListBase
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
            allBuildings = new List<T>();
            idToItem = new Dictionary<int, T>();
        }

        protected abstract List<T2> GetJsonList(MapJson mapJson, GridContent mapBuilder);

        public sealed override void ParseJson(MapJson mapJson, GridContent mapBuilder)
        {
            allBuildings = new List<T>();
            idToItem = new Dictionary<int, T>();
            List<T2> itemsJson = GetJsonList(mapJson, mapBuilder);
            foreach (var json in itemsJson)
                InstantiateFromSave(json, mapBuilder);
        }

        public sealed override void PrintToJson(MapJson mapJson, GridContent mapBuilder, ref int actualId)
        {
            itemToId = new Dictionary<T, int>();

            List<T2> itemsJson = GetJsonList(mapJson, mapBuilder);
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
                foreach (Vector3Int c in item.cells)
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
            T newItem = mapBuilder.MapRoot.InstantiateAsChild(itemPrefab);
            allBuildings.Add(newItem);

            newItem.gameObject.transform.position = mapBuilder.Grid.CellToWorldPoint(cell);
            newItem.Cell = cell;
            newItem.cells = CopyCellsMask(cell);


            return newItem;
        }

        protected abstract void InitAfterInstFormSave(T building, Vector3Int cell, T2 json, GridContent mapBuilder);
    }
}