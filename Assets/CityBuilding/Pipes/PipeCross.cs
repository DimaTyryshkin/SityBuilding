using System;
using System.Collections.Generic;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class PipeCross: PipeBaseElement,IPipeSource,IPipeDestination
	{
		public List<PipeQueue> inPipes;
		public List<PipeQueue> outPipes;

		int inIndex;
		int outIndex;

		public void Init()
		{
			Assert.IsNotNull(inPipes);
			Assert.IsNotNull(outPipes);
 
			inPipes = new List<PipeQueue>();
			outPipes = new List<PipeQueue>();
		}

		void Update()
		{
			foreach (var i in LoopFor(inIndex, inPipes.Count))
			{
				var inPipe = inPipes[i];
				if (inPipe.HaveElementAtTheEnd)
				{
					foreach (var i2 in LoopFor(outIndex, outPipes.Count))
					{
						var outPipe = outPipes[i2];
						if (outPipe.CanAdd)
						{
							PipeItem item = inPipe.Enqueue();
							outPipe.Queue(item);

							outIndex = (i2 + 1) % outPipes.Count;
							inIndex = (i + 1) % inPipes.Count;
							
							break;
						}
					}
				}
			}
		}

		[Button()]
		void Print()
		{
			String s = LoopFor(2, 4).ToStringMultiline();
			Debug.Log(s);
		}

		IEnumerable<int> LoopFor(int startIndex, int lenght)
		{
			for (int i = 0; i < lenght; i++)
				yield return (startIndex + i) % lenght;
		}

		public void RemovePipe(PipeQueue pipe)
		{
			if (inPipes.Remove(pipe))
			{
				pipe.outElement = null;
				pipe.OnChanged();
			}

			if (outPipes.Remove(pipe))
			{
				pipe.inElement = null;
				pipe.OnChanged();
			}
		}

		public void AddInPipe(PipeQueue pipe)
		{
			inPipes.Remove(pipe);
			inPipes.Add(pipe);
			pipe.outElement = this;
		}

		public void AddOutPipe(PipeQueue pipe)
		{
			outPipes.Remove(pipe);
			outPipes.Add(pipe);
			pipe.inElement = this;
		}

		public override void Delinking()
		{
			foreach (var pipe in inPipes)
			{
				pipe.outElement = null;
				pipe.OnChanged();
			}

			foreach (var pipe in outPipes)
			{
				pipe.inElement = null;
				pipe.OnChanged();
			}

			inPipes = null;
			outPipes = null;
		}
	}
}