using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Json
{
	[Serializable]
	public class MapJson
	{
		public List<PipeJson> pipes = new List<PipeJson>();
		public List<CrossJson> cross = new List<CrossJson>();
		public List<SourceJson> sources = new List<SourceJson>();
		public List<DestinationJson> destinations = new List<DestinationJson>(); 
		public List<ItemMinerJson> dirtWaterMiners = new List<ItemMinerJson>(); 
		public List<ItemMinerJson> coalMiners = new List<ItemMinerJson>(); 
		public List<ItemConverterJson> waterCleanerConverters = new List<ItemConverterJson>(); 
	}
 
	public abstract class JsonEntity
	{
		public int id;

		public abstract Vector2Int Cell { get; }
	}

	[Serializable]
	public class PipeJson:JsonEntity
	{
		public List<Vector2Int> cells = new List<Vector2Int>();

		public Vector2Int Start => cells[0];
		public Vector2Int End => cells[^1];
		
		public Vector2 GetCenter()
		{
			Vector2 center = cells[0];
			for (int i = 1; i < cells.Count; i++)
				center += cells[i];

			return center / cells.Count;
		}

		public override Vector2Int Cell => cells[0];
	} 
	
	[Serializable]
	public class CrossJson:JsonEntity
	{
		public Vector2Int cell;
		public List<int> inPipes = new List<int>();
		public List<int> outPipes = new List<int>();
		
		public override Vector2Int Cell =>cell;
	}
	
	[Serializable]
	public class SourceJson:JsonEntity
	{
		public Vector2Int cell;
		public int pipe;
		
		public string resourceName;
		
		public override Vector2Int Cell =>cell;
	}
	
	[Serializable]
	public class DestinationJson:JsonEntity
	{
		public Vector2Int cell;
		public int pipe;
		
		public string resourceName;
		public int amount;
		public int maxAmount;
		
		public override Vector2Int Cell =>cell;
	}
	
	[Serializable]
	public class ItemMinerJson:JsonEntity
	{
		public Vector2Int cell;
		public string resourceName;
		public int pipeSourceId;
		
		public override Vector2Int Cell =>cell;
	}
	
	[Serializable]
	public class ItemConverterJson:JsonEntity
	{
		public Vector2Int cell; 
		public int inDestinationId;
		public int outSourceId;
		
		public override Vector2Int Cell =>cell;
	}
}