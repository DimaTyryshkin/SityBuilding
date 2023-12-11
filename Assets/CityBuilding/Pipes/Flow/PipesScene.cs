using System.Linq;
using Game.Building;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Flow
{
	public class PipesScene : MonoBehaviour
	{
		[SerializeField, IsntNull] MapBuilder mapBuilder;
		[SerializeField, IsntNull] BuildingManager buildingManager;
		[SerializeField, IsntNull] BuildingInfoBase[] buildingsInfo;

		void Start()
		{
			foreach (var buildingInfo in buildingsInfo)
				buildingInfo.Init();
			
			
			mapBuilder.Init(buildingsInfo);
			IStaticBrushActionHandler[] abbBrushActionHandlers = buildingsInfo.CastIfCan<BuildingInfoBase,IStaticBrushActionHandler>().ToArray();
			buildingManager.Init(abbBrushActionHandlers);
		}
	}
}