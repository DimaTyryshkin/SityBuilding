using Game.Building;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Flow
{
    public class PipesScene : MonoBehaviour
    {
        [SerializeField, IsntNull] MapBuilder mapBuilder;
        [SerializeField, IsntNull] BuildingBrushPanel buildingBrushPanel;
        [SerializeField, IsntNull] BuildingManager buildingManager;
        [SerializeField, IsntNull] BuildingInfoBase[] buildingsInfo;

        void Start()
        {
            buildingBrushPanel.Init();

            foreach (var buildingInfo in buildingsInfo)
                buildingInfo.Init();


            mapBuilder.Init(buildingsInfo);
            buildingManager.Init();
        }
    }
}