﻿using System;
using System.Collections.Generic;
using Game.Json;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public enum BuildingType
	{
		None = 0,
		Pipe,
		Source,
		Destination,
		ItemMine,
		ItemConverter,
		Cross,
	}
 
	public abstract class BuildingInfoBase : MonoBehaviour
	{
		[SerializeField] Vector2Int[] cellsMask;

		public Vector2Int[] GetCellsMask()
		{
			return cellsMask;
		}

		public List<Vector2Int> CopyCellsMask(Vector2Int offset)
		{
			List<Vector2Int> newMask = new List<Vector2Int>(cellsMask.Length);

			for (int i = 0; i < cellsMask.Length; i++)
				newMask.Add(cellsMask[i] + offset);

			return newMask;
		}

		public abstract void Init();
		public abstract void ParseJson(MapJson mapJson, MapBuilder mapBuilder);
		public abstract void CastNonAllocate(Vector2Int cell, ref CellCast result);
		public abstract void PrintToJson(MapJson mapJson, MapBuilder mapBuilder, ref int actualId);
	}

	public abstract class BuildingInfo<T, T2> : BuildingInfoBase
		where T : MapElement
		where T2 : JsonEntity
	{
		[SerializeField, IsntNull] protected T itemPrefab;
		protected List<T> allItems;
  
		public Dictionary<int, T> idToItem;
		public Dictionary<T, int> itemToId;

		public sealed override void Init()
		{
			allItems = new List<T>();
			idToItem = new Dictionary<int, T>();
		}

		protected abstract List<T2> GetJsonList(MapJson mapJson, MapBuilder mapBuilder);

		public sealed override void ParseJson(MapJson mapJson, MapBuilder mapBuilder)
		{
			allItems = new List<T>();
			idToItem = new Dictionary<int, T>(); 
			List<T2> itemsJson = GetJsonList(mapJson, mapBuilder);
			foreach (var json in itemsJson)
				InstantiateFromSave(json, mapBuilder);
		} 

		public sealed override void PrintToJson(MapJson mapJson, MapBuilder mapBuilder, ref int actualId)
		{
			itemToId = new Dictionary<T, int>();

			List<T2> itemsJson = GetJsonList(mapJson, mapBuilder);
			foreach (var value in allItems)
			{
				int id = ++actualId;
				itemToId[value] = id;
				T2 json = PrintToJson(value);
				json.id = id;
				itemsJson.Add(json);
			}
		}

		protected abstract T2 PrintToJson(T item);

		public  sealed override void CastNonAllocate(Vector2Int cell, ref CellCast result)
		{
			allItems.ToStringMultilineAndLog($"CastNonAllocate {gameObject.name}",x=>x.name);
			foreach (var item in allItems)
			{
				foreach (var c in item.cells)
				{
					if (c == cell)
					{
						result.entities.Add(item);
						break;
					}
				}
			}
		}

		public void Remove(T value)
		{
			RemoveInternal(value);

			allItems.Remove(value);
			value.Delinking();
			Destroy(value.gameObject);
		}

		protected abstract void RemoveInternal(T value);
 
		public T InstantiateAsNew(Vector2Int cell, MapBuilder mapBuilder)
		{
			int id = mapBuilder.GetFreeId();
			var newItem = SpawnPrefab(cell, mapBuilder);
			idToItem[id] = newItem;

			return newItem;
		}

		private T InstantiateFromSave(T2 json, MapBuilder mapBuilder)
		{
			var newItem = SpawnPrefab(json.Cell, mapBuilder);
			idToItem[json.id] = newItem;
			InitAfterInstFormSave(newItem, json.Cell, json, mapBuilder);
			return newItem;
		}

		private T SpawnPrefab(Vector2Int cell, MapBuilder mapBuilder)
		{
			T newItem = mapBuilder.MapRoot.InstantiateAsChild(itemPrefab);
			allItems.Add(newItem);

			newItem.gameObject.transform.position = mapBuilder.Grid.CellToWorldPoint(cell);
			newItem.Cell = cell;
			newItem.cells = CopyCellsMask(cell);

		
			return newItem;
		}

		protected abstract void InitAfterInstFormSave(T item, Vector2Int cell, T2 json, MapBuilder mapBuilder);
	}
}