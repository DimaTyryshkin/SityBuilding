using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class ItemConvert : MapElement
	{
		[SerializeField] float processTime = 1;
		
		[NonSerialized] public PipeItemSource outSource;
		[NonSerialized] public PipeItemDestination inDestination;
 

		float timeStartProcess;
		float progress;
		bool isProcess;

		public void Init(PipeItemDestination inDestination, PipeItemSource outSource )
		{
			Assert.IsNotNull(outSource);
			Assert.IsNotNull(inDestination);
			
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
					{
						//statusBar.SetLock();
					}
				}
				else
				{
					//statusBar.SetProgress(progress);
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
					//statusBar.SetEmpty();
				}
			}
		}

		public override void Delinking()
		{
			 
		}
	}
}