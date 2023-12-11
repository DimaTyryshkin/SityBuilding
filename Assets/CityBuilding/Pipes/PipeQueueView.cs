using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class PipeQueueView : MonoBehaviour
	{ 
		[SerializeField, IsntNull] ItemView itemViewPrefab;
		[SerializeField, IsntNull] PipeQueue pipeQueue;
		[SerializeField, IsntNull] Transform itemsRoot;
		[SerializeField, IsntNull] Transform viewRoot;
		[SerializeField, IsntNull] GameObject segmentView;
		[SerializeField, IsntNull] GameObject startEndSegmentView;
		[SerializeField, IsntNull] GameObject cornerSegmentView;
		
		GameGrid grid;
		  
		public void Init(GameGrid grid)
		{
			Assert.IsNotNull(pipeQueue);
			Assert.IsNotNull(grid);
			this.grid = grid;
			
			DrawLine(); 
			pipeQueue.Changed += OnPipe_Changed;
		}

		void OnPipe_Changed()
		{
			DrawLine();
		}

		[Button()]
		void DrawLine()
		{
			viewRoot.DestroyChildren();

			if (pipeQueue.inElement == null)
			{
				Debug.Log("inElement == null");
				var c2 = pipeQueue.cells[0];
				var c3 = pipeQueue.cells[1];
				var c1 = c2 - (c3 - c2);
				
				BuildSegment(c1, c2, c3, startEndSegmentView);
			}
			for (int i = 1; i < pipeQueue.cells.Count-1; i++)
			{
				Vector2Int c1 = pipeQueue.cells[i - 1];
				Vector2Int c2 = pipeQueue.cells[i - 0];
				Vector2Int c3 = pipeQueue.cells[i + 1];

				BuildSegment(c1, c2, c3, segmentView);
			}

			if (pipeQueue.outElement == null)
			{
				Debug.Log("outElement == null");
				var c1 = pipeQueue.cells[^2];
				var c2 = pipeQueue.cells[^1];
				var c3 = c2 + (c2 - c1);

				BuildSegment(c1, c2, c3, startEndSegmentView, 180);
			}
		}

		void BuildSegment(Vector2Int c1, Vector2Int c2,Vector2Int c3, GameObject linePrefab, int addAngle=0)
		{
			Vector2Int d1 = c1 - c2;
			Vector2Int d2 = c3 - c2;

			bool isLine = d1 + d2 == Vector2Int.zero; 
			if (isLine)
			{
				var newSegmentView = viewRoot.InstantiateAsChild(linePrefab);
				newSegmentView.transform.position = grid.CellToWorldPoint(c2);
				int angle = grid.GetAngle(c2-c1) + addAngle;
				
				newSegmentView.transform.Rotate(0,angle,0);
			}
			else
			{
				var newCornerSegmentView = viewRoot.InstantiateAsChild(cornerSegmentView);
				newCornerSegmentView.transform.position = grid.CellToWorldPoint(c2);

				int angle = grid.GetAngle(c2-c1) + addAngle;
				if (d2.x == d1.y && d2.y == -d1.x)
					angle += 90;
					
				newCornerSegmentView.transform.Rotate(0,angle,0);
			}
		}

		void Update()
		{
			itemsRoot.DestroyChildren();

		
			foreach (var pipeItem in pipeQueue.queue)
			{
				var view = itemsRoot.InstantiateAsChild(itemViewPrefab);
				view.SetColor(ItemNameToColor(pipeItem.name));
				view.gameObject.SetActive(true);

				Vector3 oldPos =  pipeQueue.GetWorldPosition(pipeItem.time - 0.05f, grid);
				Vector3 newPos = pipeQueue.GetWorldPosition(pipeItem.time, grid);

				view.transform.position = newPos;

				Vector3 delta = newPos - oldPos;
				view.transform.rotation = Quaternion.LookRotation(delta.normalized, Vector3.up);
			}
		}

		Color ItemNameToColor(string itemName)
		{
			switch (itemName)
			{
					
				case "dirt-water": return Color.gray;
				case "clean-water": return Color.blue;
				default: return Color.black;
			}
		}
	
	}
}