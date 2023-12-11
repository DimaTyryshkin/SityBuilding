using UnityEngine;

namespace Game
{
	public class PipeItemSource : MapElement, IPipeSource
	{
		[SerializeField] StatusBar statusBar;
		public string itemName;
		public int itemAmount;
		public int maxItemAmount;
		public PipeQueue pipeQueue;
		

		public void Init(Vector2Int cell, string resourceName, int amount, int maxAmount)
		{
			Cell = cell;
			itemName = resourceName;
			itemAmount = amount;
			maxItemAmount = maxAmount;

			statusBar.SetProgress(itemAmount, maxItemAmount);
		}

		void Update()
		{
			if(!pipeQueue)
				return;
			
			if (itemAmount > 0 && pipeQueue.CanAdd)
			{
				pipeQueue.Queue(itemName);
				itemAmount--;
				statusBar.SetProgress(itemAmount, maxItemAmount);
			}
		}

		public bool IncrementAmount()
		{
			if (itemAmount < maxItemAmount)
			{
				itemAmount++;
				statusBar.SetProgress(itemAmount, maxItemAmount);
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