using System;
using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	public class CarDeliveryLogistic : MonoBehaviour
	{ 
		[SerializeField] GameGrid gameGrid;
		[SerializeField] Car[] cars;
		[SerializeField] ResourceStorage[] storages; 
    
		Dictionary<ResourceStorage, int> storageToDeliveryRequest; 
		List<DeliveryTask> tasks;

		List<Car> freeCars;
		List<ResourceStorage> availableDestinations;
		List<DeliveryTask> availableTasks;

		bool needUpdateTasks;

		public void Init()
		{
			freeCars = new List<Car>(cars);
			availableTasks = new List<DeliveryTask>();
			availableDestinations = new List<ResourceStorage>(); 
			storageToDeliveryRequest = new Dictionary<ResourceStorage,int>();
			tasks = new List<DeliveryTask>();

			foreach (var storage in storages)
			{
				storageToDeliveryRequest[storage] = 0;
				
				if(storage.AccessRules.HasFlag(StorageAccessRules.Get))
					storage.ValueAdd += (s, add) => Storage_OnValueChanged(storage, add);
			} 
		}

		void Storage_OnValueChanged(ResourceStorage storage, int add)
		{
			storageToDeliveryRequest[storage]+=add;
			needUpdateTasks = true;
		}

		void Update()
		{ 
			// Update all tasks
			foreach (var task in tasks)
			{
				if (task.status != DeliveryTask.Status.Free)
					task.Update(gameGrid);
			}

			RemoveCompleteTasks();

			if (needUpdateTasks)
				CreateNewDeliveryTasks();
		}

		void RemoveCompleteTasks()
		{
			foreach (var task in tasks)
			{
				if (task.status == DeliveryTask.Status.Complete)
					freeCars.Add(task.car);
			}

			int count = tasks.RemoveAll(t => t.status == DeliveryTask.Status.Complete);

			if (count > 0)
				needUpdateTasks = true;
		}

		[Button()]
		void CreateNewDeliveryTasks()
		{
			needUpdateTasks = false;
			availableTasks.Clear();
			foreach (KeyValuePair<ResourceStorage, int> pair in storageToDeliveryRequest)
			{
				ResourceStorage sourceStorage = pair.Key;
				for (int i = 0; i < pair.Value; i++)
				//if( pair.Value>0)
				{
					availableDestinations.Clear();
					AddAvailableDestinationsInList(sourceStorage, ref availableDestinations);

					if (availableDestinations.Count > 0)
					{
						ResourceStorage nearDestination = availableDestinations.MinItem(s => Vector3.SqrMagnitude(s.Transform.position - sourceStorage.Transform.position));

						foreach (var freeCar in freeCars)
						{
							DeliveryTask task = new DeliveryTask()
							{
								resource = sourceStorage.ResourceName,
								sourceStorage = sourceStorage,
								destinationStorage = nearDestination,
								status = DeliveryTask.Status.Free,
								car = freeCar,
								tempIndex = i,
							};
							
							availableTasks.Add(task);
						}
					}
				}
			}

			var log= availableTasks.ToStringMultiline("availableTasks", t => $"source={t.sourceStorage.name} destination={t.destinationStorage.name} car={t.car.name}");
			Debug.Log(log);
			 
			var sorted = availableTasks
				.OrderByDescending(x => Vector3.SqrMagnitude(x.car.transform.position - x.sourceStorage.Transform.position))
				.ToList();

			int n = 9999999;
			while (sorted.Count > 0)
			{
				n--;
				Assert.IsTrue(n > 0);
				
				int index = sorted.Count - 1;

				DeliveryTask task = sorted[index];
				if (!task.car.IsMove)
				{
					freeCars.Remove(task.car);
					task.GoToSource(gameGrid);
					tasks.Add(task);
					storageToDeliveryRequest[task.sourceStorage]--;

					sorted.RemoveAll(t => t.car == task.car);
					sorted.RemoveAll(t => (t.tempIndex == task.tempIndex) && (t.sourceStorage == task.sourceStorage));
				}
				else
				{
					sorted.RemoveAt(sorted.Count - 1);
				}
			}

			var log2= tasks.ToStringMultiline("tasks", t => $"source={t.sourceStorage.name} destination={t.destinationStorage.name} car={t.car.name}");
			Debug.Log(log2);
		}

		void AddAvailableDestinationsInList(ResourceStorage source, ref List<ResourceStorage> availableDestinations)
		{
			foreach (ResourceStorage destination in storages)
			{
				if(destination.Value<10 && source.CanDeliverTo(destination))
					availableDestinations.Add(destination);
			}
		}


		[Button()]
		void Load()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Load");
#endif
			
			storages = FindObjectsOfType<ResourceStorage>();
			cars = FindObjectsOfType<Car>();
		}
	}
}