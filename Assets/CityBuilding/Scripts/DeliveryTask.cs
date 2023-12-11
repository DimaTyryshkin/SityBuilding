using System;
using UnityEngine;

namespace Game
{
	struct DeliveryRequest
	{
		public string resource;
		public ResourceStorage sourceStorage;
	}

	class DeliveryTask
	{
		public enum Status
		{
			Free = 0,
			GoToSource = 1,
			GoToDestination = 2,
			Complete = 3,
		}

		public Status status;
		public Car car;
		public string resource;
		public ResourceStorage sourceStorage;
		public ResourceStorage destinationStorage;
		public int tempIndex;

		public void Update(GameGrid grid)
		{
			if (car.IsMove == false)
			{
				if (status == Status.GoToSource)
				{
					OnVisitSource(grid);
					return;
				}

				if (status == Status.GoToDestination)
				{
					OnVisitDestination();
					return;
				}
			}
		}

		public void GoToSource(GameGrid grid)
		{
			status = Status.GoToSource;
			car.SetTarget(grid.WorldPointToCell(sourceStorage.Transform.position));
		}

		void OnVisitSource(GameGrid grid)
		{
			if (sourceStorage.Value > 0)
			{
				sourceStorage.Value--;
				car.SetTarget(grid.WorldPointToCell(destinationStorage.Transform.position));
				car.SetCargoEnable(true, GetColorFromResource(sourceStorage.ResourceName));
				status = Status.GoToDestination;
			}
			else
			{
				status = Status.Free;
			}
		}

		Color GetColorFromResource(String resourceName)
		{
			if(resourceName== "r1")
				return Color.green;
			else
			{
				return Color.blue;
				;
			}
		}

		void OnVisitDestination()
		{
			if (destinationStorage.Value < 10)
			{
				destinationStorage.Value++;
				status = Status.Complete;
				car.SetCargoEnable(false,Color.black);
			}
			else
			{
				// Стоим и ждем
			}
		}
	}
}