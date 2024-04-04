using System;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	[CreateAssetMenu(menuName = "Game/ProductsData")]
	public class ProductsData : ScriptableObject
	{
		[SerializeField] Color fuelColor;
		[SerializeField] Color shellColor;
		[SerializeField] Color foodColor;
		[SerializeField] Color materialsColor;
		
		public Color GetColor(int productIndex)
		{
			if (productIndex == 0)
				return fuelColor;
			
			if (productIndex == 1)
				return shellColor;
			
			if (productIndex == 2)
				return foodColor;
			
			if (productIndex == 3)
				return materialsColor;

			throw new ArgumentException();
		}
	}
}