using System;
using UnityEngine;

namespace Game
{
	public class PipeItemSource : MapElement, IPipeSource
	{ 
		[NonSerialized] public string itemName; 
		[NonSerialized] public PipeQueue pipeQueue;
		

		public void Init(string resourceName)
		{
			itemName = resourceName; 
 
		}
 
		public bool IncrementAmount()
		{
			if (pipeQueue && pipeQueue.CanAdd)
			{
				pipeQueue.Queue(itemName);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemovePipe(PipeQueue pipe)
		{
			if (pipeQueue && pipe == pipeQueue)
			{
				pipeQueue.inElement = null;
				pipeQueue = null;
			}
		}

		public void AddOutPipe(PipeQueue pipe)
		{
			if (pipe != pipeQueue)
			{
				if (pipeQueue)
					pipeQueue.inElement = null;

				pipeQueue = pipe;
				pipeQueue.inElement = this;
			}
		}

		public override void Delinking()
		{
			RemovePipe(pipeQueue);
		}
	}
}