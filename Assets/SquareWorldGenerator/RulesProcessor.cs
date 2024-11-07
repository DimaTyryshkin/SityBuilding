using GamePackages.Core;
using NaughtyAttributes;
using SquareWorldGenerator;
using UnityEngine;
using Grid = SquareWorldGenerator.Grid;
using Random = UnityEngine.Random;

namespace CellularAutomaton
{
	public class RulesProcessor:MonoBehaviour
	{
		[SerializeField] Grid grid;
		[SerializeField] Rules rules;
		 
		[SerializeField] int freq;   
		[SerializeField] bool autoTick;
		[SerializeField] bool autoTickOnStart;
        
        
		[Space]
		[SerializeField] bool randomRules;
		[SerializeField] bool fullRandom;
		[SerializeField] bool randomPoints;
		[SerializeField] int randomPointsCount = 10;
          
		float timeNextUpdate;
   
		CellType[,] field1;
		CellType[,] field2;
		int[] counter; 
        
        
		void Start()
		{
			randomRules = false;
			grid.Click += BrushToSelected;
			timeNextUpdate = 0.0f;
			DrawStart();
		}

		void Update()
		{
			if (autoTick)
			{
				if (Time.time < timeNextUpdate)
					return;

				timeNextUpdate = Time.time + 1f / freq;
				Tick();
			}
		}

		public void StopAutoTick()
		{
			autoTick = false;
		}

		[Button()]
		void DrawStart()
		{ 
			if(randomRules)
				RandomRules();
				
			rules.GenerateVariants();
			field1 = new CellType[grid.width, grid.height];
			field2 = new CellType[grid.width, grid.height];
		
            
			if (autoTickOnStart)
				autoTick = true;

			//if (randomRules)
			//	GenerateRandomMatrix(r); 
			
 
			grid.Init();
			FillClearField();
 
			if (fullRandom)
			{
				FullRandom(); 
			}
			else if (randomPoints)
			{
				RandomPoints();
			}
			else
			{
				FillClearField();
			}
		}


		void RandomRules()
		{
			foreach (var group in rules.groups)
			{
				group.rules.Clear();

				for (int n = 0; n < 3; n++)
				{
					var r = Rule.Create();

					r.repeat = Random.Range(0, 100) > 70;
					r.rotation = Random.Range(0, 100) > 70;

					bool haveMask = false;
					for (int i = 0; i < Rule.n * Rule.n; i++)
					{
						r.mainVariant.mask[i] = CellMask.None;
						r.mainVariant.result[i] = CellMask.None;

						if (Random.Range(0, 100) > 80)
						{
							r.mainVariant.mask[i] = (CellMask)(1 << Random.Range(0, rules.colors.Length));
							haveMask = true;
						}
					}

					if (!haveMask)
						continue;

					for (int i = 0; i < Rule.n * Rule.n; i++)
					{
						if (Random.Range(0, 100) > 80)
							r.mainVariant.result[i] = (CellMask)(1 << Random.Range(0, rules.colors.Length));
					}
					
					group.rules.Add(r);
				}
			}
		}

		void FullRandom()
		{
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				field1[x, y] = rules.colors.Random().type;
				grid.Get(x, y).CellType = field1[x, y];
			}
		}


		void RandomPoints()
		{ 
			for (int i = 0; i < randomPointsCount; i++)
			{
				int x = Random.Range(0, grid.width);
				int y = Random.Range(0, grid.height);
				field1[x, y] =  rules.colors.Random().type;
				grid.Get(x, y).CellType = field1[x, y];
			}
		}
		
		void FillClearField()
		{ 
			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				field1[x, y] = CellType.Empty;
				grid.Get(x, y).CellType = field1[x, y];
			}
		}

		[Button()]
		void Tick()
		{
			for (int i = 0; i < grid.Count; i++)
			{
				Vector2Int cell = grid.GetCell(i);
				field2[cell.x, cell.y] = field1[cell.x, cell.y];
			}

			for (int i = 0; i < grid.Count; i++)
			{
				Vector2Int cell = grid.GetCell(i);
				//ApplyRules(cell);
				ApplyGameOfLive(cell);
			}

			for (int x = 0; x < grid.width; x++)
			for (int y = 0; y < grid.height; y++)
			{
				field1[x, y] = field2[x, y];
				grid.Get(x, y).CellType = field1[x, y];
			}
		}
  
		//int n = 0;
		void ApplyRules(Vector2Int cell)
		{

			//n = 0;
			foreach (var group in rules.groups)
			foreach (var rule in group.rules)
			{
				//n++;
				if (ApplyRule(cell, rule))
					return;
			} 
		}

		[Header("Game of Live")]
		[SerializeField] int n = 3;
		void ApplyGameOfLive(Vector2Int cell)
		{
			int liveCounter = 0;
			foreach (var overlay in grid.Mask(n, cell.x, cell.y))
			{
				bool isCenter = overlay.x == cell.x && overlay.y == cell.y;
				if (!isCenter)
					if (field1[overlay.x, overlay.y] == CellType.Green)
						liveCounter++;
			}

			if (field1[cell.x, cell.y] == CellType.Empty)
			{
				if (liveCounter == 3)
					field2[cell.x, cell.y] = CellType.Green;
			}
			else
			{
				if (liveCounter is < 2 or > 3)
					field2[cell.x, cell.y] = CellType.Empty;
			}
		}

		bool ApplyRule(Vector2Int cell, Rule r)
		{  
			
			bool anyMatch = false;
			foreach (var ruleVariant in r.RunTimeVariants)
			{
			 
				bool match = true;
				foreach (var overlay in grid.Mask(Rule.n, cell.x, cell.y))
				{
					CellMask value = (CellMask)field1[overlay.x, overlay.y] & ruleVariant.GetMask(overlay.maskX, overlay.maskY);
					//Debug.Log((CellType)value);
					//grid.Get(overlay.x, overlay.y).CellType = (CellType)value;
					if (value == CellMask.None)
					{
						// Маска не подходит

						match = false;
						break;
					}
				}
				
				
 
				// Маска подходит
				if(match)
				{
					//Debug.Log($"match {n}");
					foreach (var overlay in grid.Mask(Rule.n, cell.x, cell.y))
					{
						CellType result = (CellType)(byte)ruleVariant.GetResult(overlay.maskX, overlay.maskY);
						if (result != CellType.None)
						{
							//grid.Get(overlay.x, overlay.y).SetColor(Color.blue);
							//Debug.Log($"result = {result}");
							field2[overlay.x, overlay.y] = result;
							grid.Get(overlay.x, overlay.y).CellType = result; //TODO удалить
						}
					}

					if (!r.repeat)
					{
						//Debug.Log($"return true {n}");
						return true;
					}
					//Debug.Log($"continue {n}");

					anyMatch = true;
				}  
			}

			return anyMatch;
		}

		void ProcessCell(Vector2Int nearCell, Square square, SquareTypeRules rule)
		{
			var nearSquare = grid.GetLoop(nearCell);
			if (nearSquare.waitForUpdate)
			{  
				for (int j = 0; j < rule.hunting.Length; j++)
				{
					if (nearSquare.Type == rule.hunting[j])
					{
						square.Type = rule.type1; 
						nearSquare.Type = rule.type2;
						nearSquare.waitForUpdate = false;
					}
				} 
			}
		} 
        
		[Button()]
		public void Clear()
		{
			grid.Clear();
		}

	

		[Button()]
		public void RemoveObjects()
		{
			grid.RemoveObjects(); 
		} 
		
		[Button()]
		void ApplyMatrixToSelection()
		{
			ApplyRules(grid.SelectedCell);
		}

		[SerializeField] CellType brush;

		[Button()]
		void BrushToSelected()
		{
			if(brush != CellType.None)
			{
				grid.HideSelection();
				int x = grid.SelectedCell.x;
				int y = grid.SelectedCell.y;
				field1[x, y] = brush;
				grid.Get(x, y).CellType = field1[x, y];
			}
		}
		
		[Button()]
		void Test()
		{
			Debug.Log(CellMask.None & (CellMask)CellType.Red);
			Debug.Log(CellMask.Red & (CellMask)CellType.Red);
			Debug.Log((CellMask)byte.MaxValue & (CellMask)CellType.Red);
			
			
			Debug.Log((CellType)(byte)CellMask.Blue);
		}
	}
}