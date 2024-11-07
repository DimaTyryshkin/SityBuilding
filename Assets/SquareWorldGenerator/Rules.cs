using System;
using System.Collections.Generic;
using UnityEngine;

namespace CellularAutomaton
{
	[CreateAssetMenu(menuName = "CellularAutomaton/Rules")]
	public class Rules : ScriptableObject
	{
		public TypeAndColor[] colors;
		public List<RulesGroup> groups;

		public void GenerateVariants()
		{
			foreach (var g in groups)
			foreach (var r in g.rules)
				r.GenerateVariants();
		}

		public Color CellTypeToColor(CellType cellType)
		{

			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i].type == cellType)
					return colors[i].color;
			}

			return Color.magenta;
		}

		[System.Serializable]
		public struct TypeAndColor
		{
			public CellType type;
			public Color color;
		}
	}

	[Serializable]
	public class RulesGroup
	{
		[HideInInspector] public bool isOpen;
		public CellType cellType;
		public string name;
		public List<Rule> rules;
	}

	[Serializable]
	public class Rule
	{
		public static readonly int n = 3;
		public bool rotation;
		public bool repeat;

		public RuleVariant mainVariant;

		public RuleVariant[] RunTimeVariants { get; private set;}

		public void GenerateVariants()
		{
			var v = CreateVariant();
			for (int x = 0; x < n; x++)
			for (int y = 0; y < n; y++)
			{
				int i = RuleVariant.GetIndex(x, y);
				v.mask[i] = mainVariant.mask[i];
				v.result[i] = mainVariant.result[i];

				if (v.mask[i] == CellMask.None)
					v.mask[i] = (CellMask)byte.MaxValue;
			}
			
			if (rotation)
			{ 
				RunTimeVariants = new RuleVariant[4];
				RunTimeVariants[0] = v;
				RunTimeVariants[1] = RotateVariant90Right(v);
				RunTimeVariants[2] = RotateVariant90Right(RunTimeVariants[1]);
				RunTimeVariants[3] = RotateVariant90Right(RunTimeVariants[2]);
			}
			else
				RunTimeVariants = new RuleVariant[] { v };
		}

		public static Rule Create()
		{
			return new Rule()
			{
				mainVariant = CreateVariant()
			};
		}

		static RuleVariant CreateVariant()
		{
			CellMask[] m = new CellMask[n * n];
			CellMask[] r = new CellMask[n * n];

			return new RuleVariant()
			{
				mask = m,
				result = r
			};
		}
		
		static RuleVariant RotateVariant90Right(RuleVariant oldVariant)
		{
			var v = CreateVariant();
			
			for (int x = 0; x < n; x++)
			for (int y = 0; y < n; y++)
			{
				int i = RuleVariant.GetIndex(x, y);
				int iNew = RuleVariant.GetIndex(y, (n-1) - x);
				v.mask[iNew] = oldVariant.mask[i];
				v.result[iNew] = oldVariant.result[i];
			}

			return v;
		}
	}

	[Serializable]
	public class RuleVariant
	{
		public CellMask[] mask;
		public CellMask[] result;

		public CellMask GetMask(int x, int y)
		{
			return mask[GetIndex(x, y)];
		}
		
		public void SetMask(int x, int y, CellMask value)
		{
			mask[GetIndex(x, y)] = value;
		}
		
		public CellMask GetResult(int x, int y)
		{
			return result[GetIndex(x, y)];
		}
		
		public void SetResult(int x, int y, CellMask value)
		{
			result[GetIndex(x, y)] = value;
		}

		public static int GetIndex(int x, int y)
		{
			return y * Rule.n + x;
		}
	}

	public enum CellType : byte
	{
		None = 0,
		Empty = 1,
		Red = 2,
		Green = 4,
		Blue = 8,
		Yellow = 16,
	}

	[Flags]
	public enum CellMask : byte
	{
		None = 0,
		Empty = 1,
		Red = 2,
		Green = 4,
		Blue = 8,
		Yellow = 16,
	}
}