using System;
using UnityEngine;

namespace SquareWorldGenerator
{
	[CreateAssetMenu(menuName = "TT/SquareWorldGenerator/ColorCollection")]
	public class TypeCollection : ScriptableObject
	{
		public SquareTypeRules[] rules;

		public int ColorsCount => rules.Length;
		
		public Color GetColor(int squareType)
		{
			return rules[squareType].color;
		}
	}

	[Serializable]
	public class SquareTypeRules
	{
		public Color color;
		public int nearRule;
		public int type1;
		public int type2;
		public int[] hunting;
		public float[] moveChance;
	}
}