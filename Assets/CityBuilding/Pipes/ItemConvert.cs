using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class ItemConvert : MapElement
	{
		[SerializeField] StatusBar statusBar;
		[SerializeField] float processTime = 1;
		
		public string converterType;
		public PipeItemSource outSource;
		public PipeItemDestination inDestination;
 

		float timeStartProcess;
		float progress;
		bool isProcess;

		public void Init(Vector2Int cell, string converterType, PipeItemDestination inDestination, PipeItemSource outSource )
		{
			Assert.IsNotNull(outSource);
			Assert.IsNotNull(inDestination);
			
			Cell = cell;
			cells = new List<Vector2Int>()
			{
				cell + new Vector2Int(1, 0),
				cell + new Vector2Int(0, 1),
				cell + new Vector2Int(1, 1),
			};
			
			this.converterType = converterType;
			this.outSource = outSource;
			this.inDestination = inDestination;
		}

		void Update()
		{ 
			if (isProcess)
			{
				progress = (Time.time - timeStartProcess) / processTime;
				if (progress >= 1)
				{
					if (outSource.IncrementAmount())
						isProcess = false;
					else
						statusBar.SetLock();
				}
				else
				{
					statusBar.SetProgress(progress);
				}
			}
			else
			{
				if (inDestination.DecrementAmount())
				{
					isProcess = true;
					timeStartProcess = Time.time;
				}
				else
				{
					statusBar.SetEmpty();
				}
			}
		}

		public override void Delinking()
		{
			 
		}
	}
}