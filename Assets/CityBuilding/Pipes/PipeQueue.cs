using System.Collections.Generic;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Game
{
	public class PipeItem
	{
		public float time;
		public string name;
	}

	public class PipeQueue : PipeBaseElement
	{
		[SerializeField] float elementDistance = 1;
		public QueueWithIndexer<PipeItem> queue;
		
		public Vector2Int StartCell=>cells[0];
		public Vector2Int EndCell=>cells[^1];
		
		public IPipeSource inElement;
		public IPipeDestination outElement; 
		
		float lenght;

		public event UnityAction Changed;
		public event UnityAction Removing;

		public bool CanAdd {
			get
			{
				if (queue.Count == 0)
					return true;

				return queue[^1].time > elementDistance;
			}
		}

		public bool HaveElementAtTheEnd => ItemAtTheEnd != null;

		public PipeItem ItemAtTheEnd
		{
			get
			{
				if (queue.Count == 0)
					return null;

				var item = queue[0];
				if (item.time >= lenght)
					return item;

				return null;
			}
		}

		public void Init(List<Vector2Int> cells)
		{
			this.cells = cells;
			lenght = cells.Count - 1;
			queue = new QueueWithIndexer<PipeItem>(); 
		}

		void Update()
		{
			if (queue.Count == 0)
				return;

			float dt = Time.deltaTime * 1f;
			queue[0].time = Mathf.Min(lenght, queue[0].time + dt);

			for (int i = 1; i < queue.Count; i++)
			{
				float distanceToNext = queue[i - 1].time - queue[i].time;
				queue[i].time += Mathf.Min(dt, distanceToNext - elementDistance);
			}
		}

		public void OnChanged()
		{
			lenght =  cells.Count - 1;
			Changed?.Invoke();
		}

		public Vector3 GetWorldPosition(float time, GameGrid grid)
		{
			time = Mathf.Clamp(time, 0, lenght - 0.0001f);
			int i1 = (int)time;
			int i2 = i1 + 1;
			Vector2 cell = Vector2.LerpUnclamped(cells[i1], cells[i2], time % 1);
			//Vector2 cell = Vector2.Lerp(StartCell, EndCell, time / lenght);
			return  grid.CellToWorldPoint(cell);
		}

		public void Queue(string name)
		{
			Queue(new PipeItem()
			{
				name = name
			});
		}
		
		public void Queue(PipeItem item)
		{
			item.time = 0;
			queue.Queue(item);
		}

		public PipeItem Enqueue()
		{
			Assert.IsTrue(queue.Count > 0);

			return queue.Enqueue();
		}

		public override void Delinking()
		{
			inElement?.RemovePipe(this);
			outElement?.RemovePipe(this);

			inElement = null;
			outElement = null;
			
			Removing?.Invoke();
		}

		[Button()]
		void Log()
		{
			Debug.Log($"In  is Null={inElement == null}");
			Debug.Log($"Out is Null={outElement == null}");
		}
	}
}