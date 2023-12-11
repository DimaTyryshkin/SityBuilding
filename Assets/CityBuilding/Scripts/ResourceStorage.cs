using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public enum StorageOrder
	{
		Miner = 0,
		Store = 1,
		Factory = 2,
		Consumer = 3
	}

	[Flags]
	public enum StorageAccessRules
	{
		Lock = 0,
		Put = 1,
		Get = 2,
		GetAndPut = Put | Get,
	}

	public class ResourceStorage : MonoBehaviour
	{
		[SerializeField] string resourceName;
		[SerializeField] int value;
		[SerializeField] StorageOrder order;
		[SerializeField] StorageAccessRules accessRules;

		public Transform Transform => transform;
		public StorageOrder Order => order;
		public StorageAccessRules AccessRules => accessRules;
		public string ResourceName => resourceName;
		
	    public event UnityAction<ResourceStorage,int> ValueChanged;
		public event UnityAction<ResourceStorage,int> ValueAdd;


		public int Value
		{
			get => value;
			set
			{ 
				int add = value - this.value;
				this.value = value;

				ValueChanged?.Invoke(this, value);
				if (add > 0)
					ValueAdd?.Invoke(this, add);
			}
		}


		public void SetModes(StorageOrder order, StorageAccessRules accessRules)
		{
			this.order = order;
			this.accessRules = accessRules;
		}

		public bool CanDeliverTo(ResourceStorage destination)
		{
			if (!destination.accessRules.HasFlag(StorageAccessRules.Put))
				return false;

			if (resourceName != destination.resourceName)
				return false;
			
			
			// Таблица
			if (order < destination.order)
				return true;
			
			if (order == StorageOrder.Factory && destination.order == StorageOrder.Factory)
				return true;
			
			if (order == StorageOrder.Factory && destination.order == StorageOrder.Store)
				return true;

			return false;
		}

		[Button()]
		void Add1()
		{
			Value+=1;
		}
		
		[Button()]
		void Add2()
		{
			Value+=2;
		}
	}
}