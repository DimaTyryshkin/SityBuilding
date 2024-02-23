using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public abstract class MapElement : MonoBehaviour
	{
		[NonSerialized] public List<Vector2Int> cells;
		public Vector2Int Cell { get; set; }
		public abstract void Delinking();
	}
	
	public abstract class PipeBaseElement : MapElement
	{
	}

	public interface IPipeSource
	{
		void RemovePipe(PipeQueue pipe); 
		void AddOutPipe(PipeQueue pipe);
	}
	
	public interface IPipeDestination
	{
		void RemovePipe(PipeQueue pipe); 
		void AddInPipe(PipeQueue pipe);
	}

	public static class PipeExtension
	{
		 
	}
}