using System;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public enum FactoryType
	{
		None = -1,
		Fuel = 0,
		Shells = 1,
		Food = 2,
		Materials = 3
	}
	
	[Serializable]
	public struct Products
	{
		public float fuel;
		public float shells;
		public float food;
		public float materials;
		
		public int ProductIndex { get; set; }


		public void Clamp(float min, float max)
		{
			fuel = Mathf.Clamp(fuel, min, max);
			shells = Mathf.Clamp(shells, min, max);
			food = Mathf.Clamp(food, min, max);
			materials = Mathf.Clamp(materials, min, max);
		}

		public void Add(int index, float value)
		{
			if (index == 0) fuel += value;
			if (index == 1) shells += value;
			if (index == 2) food += value;
			if (index == 3) materials += value;
		}

		public float this[int index]
		{
			get
			{
				if (index == 0) return fuel;
				if (index == 1) return shells;
				if (index == 2) return food;
				return materials;
			}

			set
			{
				if (index == 0) fuel = value;
				if (index == 1) shells = value;
				if (index == 2) food = value;
				if (index == 3) materials = value;
			}
		}

		public Products(float fuel, float shells, float food, float materials)
		{
			this.fuel = fuel;
			this.shells = shells;
			this.food = food;
			this.materials = materials;
			ProductIndex = -1;
		}

		public Products(int productIndex, float value)
		{
			this.fuel = 0;
			this.shells = 0;
			this.food = 0;
			this.materials = 0;
			ProductIndex = productIndex; 
			Add(productIndex, value);
		}
 
		public static bool operator >(Products a, Products b)
			=> 
				a.fuel > b.fuel &&
				a.shells > b.shells &&
				a.food > b.food &&
				a.materials > b.materials ;

		public static bool operator <(Products a, Products b)
			=> 
				a.fuel < b.fuel &&
				a.shells < b.shells &&
				a.food < b.food &&
				a.materials < b.materials ;

		public static Products operator +(Products a, Products b)
			=> new Products(
				a.fuel + b.fuel,
				a.shells + b.shells,
				a.food + b.food,
				a.materials + b.materials);

		public static Products operator -(Products a, Products b)
			=> new Products(
				a.fuel - b.fuel,
				a.shells - b.shells,
				a.food - b.food,
				a.materials - b.materials);

		public static Products operator *(Products a, float b)
			=> new Products(
				a.fuel * b,
				a.shells * b,
				a.food * b,
				a.materials * b);

		public override string ToString()
		{
			return $"F:{fuel:000.0} S:{shells:000.0} F:{food:000.0} M:{materials:000.0}";
		}
	}
}