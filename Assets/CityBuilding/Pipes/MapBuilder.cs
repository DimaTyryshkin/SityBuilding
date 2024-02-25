using System.Collections.Generic;
using System.IO;
using Game.Building;
using Game.Json;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine; 

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	public class CellCast
	{
		public bool IsFree => entities.Count == 0;
		public List<MapElement> entities = new List<MapElement>();
	}

	public class MapBuilder : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform mapRoot;
		[SerializeField, IsntNull] GameGrid grid;
		[SerializeField, IsntNull] TextAsset json1;
		[SerializeField, IsntNull] TextAsset jsonTemp;

		BuildingInfoBase[] buildingsInfo;
		MapJson map;
		int actualId;
		
		//Dictionary<int, PipeCross> idToCross = new Dictionary<int, PipeCross>();
		//Dictionary<int, PipeItemDestination> idToDestination = new Dictionary<int, PipeItemDestination>();
		 
		public MapJson Map => map;
		public Transform MapRoot => mapRoot;
		public GameGrid Grid => grid;

		public int GetFreeId()
		{
			return ++actualId;
		} 
		
		public void Init(BuildingInfoBase[] buildingsInfo)
		{
			AssertWrapper.IsAllNotNull(buildingsInfo);
			
			this.buildingsInfo = buildingsInfo;
			map = new MapJson();
		}
 
		void LoadFromJson()
		{   
			transform.DestroyChildren();

			foreach (var building in buildingsInfo)
			{
				building.ParseJson(map, this);
			} 
		}

		public void Remove(MapElement mapElement)
		{      
			//if(mapElement is ItemConvert converter)
			//	RemoveItemConverter(converter);
		}
  
		public void CastNonAllocate(Vector2Int cell, ref CellCast result)
		{
			result.entities.Clear();
			
			foreach (var info in buildingsInfo)
				info.CastNonAllocate(cell, ref result); 
		} 
 
		void LoadFromJson(TextAsset jsonFile)
		{
#if UNITY_EDITOR
			string fileName = AssetDatabase.GetAssetPath(jsonFile);
			string json = File.ReadAllText(fileName);
			map = JsonUtility.FromJson<MapJson>(json); 
			LoadFromJson();
#endif
		}
		   
		void PrintToJson(TextAsset jsonFile)
		{ 
#if UNITY_EDITOR
			map = new MapJson();

			actualId = 0;
			foreach (var info in buildingsInfo)
				info.PrintToJson(map, this, ref actualId);
			    
			string mapJson = JsonUtility.ToJson(map, true);
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