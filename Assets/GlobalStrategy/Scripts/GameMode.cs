using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using GlobalStrategy.CoreLogic;
using UnityEngine;


namespace GlobalStrategy
{
	public class GameMode : MonoBehaviour
	{ 
		[SerializeField, IsntNull] ProductRequests productRequests;
		[SerializeField, IsntNull] Camera mainCamera;
		[SerializeField, IsntNull] TouchInput touchInput ;

		void Start()
		{ 
			touchInput.Init(mainCamera);
			productRequests.Init();
		}
	}
}