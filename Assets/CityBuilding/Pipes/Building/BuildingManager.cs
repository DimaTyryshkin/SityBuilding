using System.Linq;
using Game.GameGui;
using Game.Json;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{
	public class BuildingManager : MonoBehaviour
	{
		[SerializeField, IsntNull] MapBuilder mapBuilder;
		[SerializeField, IsntNull] BuildingBrushPanel buildingBrushPanel;
		[SerializeField, IsntNull] PipeBrush pipeBrush;
		[SerializeField, IsntNull] StaticObjectBrush staticObjectBrush;

		IStaticBrushActionHandler[] buildingsInfo;

		public void Init(IStaticBrushActionHandler[] buildingsInfo)
		{   
			AssertWrapper.IsAllNotNull(buildingsInfo);
			this.buildingsInfo = buildingsInfo;
			
			buildingBrushPanel.SelectBrush += OnSelectBuildingBrush;
			OnSelectBuildingBrush(BuildingType.None);
		}

		void OnSelectBuildingBrush(BuildingType brushType)
		{
			if (brushType == BuildingType.None)
			{
				pipeBrush.gameObject.SetActive(false);
				staticObjectBrush.gameObject.SetActive(false);
			}
			else if (brushType == BuildingType.Pipe)
			{
				pipeBrush.gameObject.SetActive(true);
				staticObjectBrush.gameObject.SetActive(false);
			}
			else
			{
				pipeBrush.gameObject.SetActive(false);
				staticObjectBrush.gameObject.SetActive(true);
				
				var buildingInfo = buildingsInfo.First(b => b.BuildingType == brushType);
				staticObjectBrush.Set(buildingInfo.GetCellsMask(), cell =>
				{
					buildingInfo.BuildByBrushCommand(cell, mapBuilder );
				});
			}
		}
	}
	
}