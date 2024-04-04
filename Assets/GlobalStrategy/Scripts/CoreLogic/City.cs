using System; 
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GlobalStrategy.CoreLogic
{
	public class City : MonoBehaviour
	{
		public int id;
		public int country;
		
		public Products balance;
		[NonSerialized] public Products credit;

		[SerializeField] CityState state;

		public CityState State
		{
			get => state;
			private set
			{
				Assert.IsNotNull(state);
				if (state)
					Destroy(state);

				state = value;
				state.StartState(this);
			}
		}

		public static readonly float maxCapacityValue = 200; 
		public event UnityAction Updated;
 
		
		void Update()
		{ 
			state.Frame();
		}

		
		public void Init()
		{ 
			state.StartState(this);
		}
		
		public void Add(Products p)
		{
			balance += p;
			
			balance.Clamp(0,maxCapacityValue);
			Updated?.Invoke();
		}
 
		public void Add(int index, float value)
		{
			balance.Add(index, value);
			
			balance.Clamp(0,maxCapacityValue);
			Updated?.Invoke();
		}
		
		public void Remove(Products p)
		{
			balance -= p;
			
			balance.Clamp(0,maxCapacityValue);
			Updated?.Invoke();
		}
		
		public void Remove(int index, float value)
		{
			balance.Add(index, -value);
			
			balance.Clamp(0,maxCapacityValue);
			Updated?.Invoke();
		}
 
		public bool CanRemove(Products b)
		{
			Products c = balance - b;
			if (c.fuel      < 0) return false;
			if (c.shells    < 0) return false;
			if (c.food      < 0) return false;
			if (c.materials < 0) return false;

			return true;
		}
	}
}
