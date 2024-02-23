using UnityEngine;

namespace Game
{
	public class PipeItemDestination : MapElement, IPipeDestination
	{
		[SerializeField] StatusBar statusBar;
		public int itemAmount;
		public int maxItemAmount;
		public PipeQueue pipeQueue;
		public string itemName;

		public void Init(string itemName, int amount, int maxAmount)
		{ 
			this.itemName = itemName; 
			itemAmount = amount;
			maxItemAmount = maxAmount;

			statusBar.SetProgress(itemAmount, maxItemAmount);
		}

		void Update()
		{
			if(!pipeQueue)
				return;
			
			if (itemAmount < maxItemAmount)
			{
				var item = pipeQueue.ItemAtTheEnd;
				if (item != null && item.name == itemName)
				{
					pipeQueue.Enqueue();
					itemAmount++;
					statusBar.SetProgress(itemAmount, maxItemAmount);
				}
			}
		}
		
		public bool DecrementAmount()
		{
			if (itemAmount >0)
			{
				itemAmount--;
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
				pipeQueue.outElement = null;
				pipeQueue = null;
				pipe.OnChanged();
			}
		}
		
		public void AddInPipe(PipeQueue pipe)
		{
			if (pipe != pipeQueue)
			{
				if(pipeQueue)
					pipeQueue.outElement = null;
				
				pipeQueue = pipe;
				pipeQueue.outElement = this;
			}
		}

		public override void Delinking()
		{
			RemovePipe(pipeQueue);
		}
	}
}