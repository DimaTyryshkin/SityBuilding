using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class GameMode : MonoBehaviour
	{
		[SerializeField, IsntNull] RoadLayer roadLayer;
		[SerializeField, IsntNull] RoadView roadView;
		[SerializeField, IsntNull] CarDeliveryLogistic carLogistic;
		[SerializeField, IsntNull] Factory[] factories; 

		void Start()
		{
			roadLayer.StartIt();
			roadView.StartIt();
			carLogistic.Init();

			foreach (var factory in factories)
				factory.Init();
		}
	}
}