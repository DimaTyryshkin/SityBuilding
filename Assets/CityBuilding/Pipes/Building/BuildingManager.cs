using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Building
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField, IsntNull] MapBuilder mapBuilder;
        [SerializeField, IsntNull] BuildingBrushPanel buildingBrushPanel;
        [SerializeField, IsntNull] Camera thisCamera;
        [SerializeField, IsntNull] GameGrid grid;
        [SerializeField, IsntNull] GameObject cellMarkerAvailable;
        [SerializeField, IsntNull] GameObject cellMarkerLock;
        [SerializeField, IsntNull] GuiHit guiHit;

        [Header("Building info")]
        [SerializeField, IsntNull] PipeBuildingInfo pipeBuildingInfo;
        [SerializeField, IsntNull] CrossBuildingInfo crossBuildingInfo;
        [SerializeField, IsntNull] MinerBuildingInfo dirtWaterMinerBuildingInfo;
        [SerializeField, IsntNull] MinerBuildingInfo coalMinerBuildingInfo;
        [SerializeField, IsntNull] ConverterBuildingInfo converterBuildingInfo;

        PointBrush staticObjectBrush;

        PipeBrush pipeBrush;
        List<PointBrush> staticBrushes;
        BuildingBrush activeBrush;


        public void Init()
        {
            BrushBuilder brushBuilder = new BrushBuilder(
                thisCamera,
                mapBuilder,
                grid,
                cellMarkerAvailable,
                cellMarkerLock,
                guiHit
            );

            pipeBrush = new PipeBrush(pipeBuildingInfo, crossBuildingInfo, brushBuilder);

            activeBrush = null;
            buildingBrushPanel.SelectPipe += () => activeBrush = pipeBrush;

            staticBrushes = new List<PointBrush>();


            AddProduction(
                brushBuilder,
                dirtWaterMinerBuildingInfo,
                "Water Pump",
                cell => dirtWaterMinerBuildingInfo.InstantiateNew(cell, mapBuilder));

            AddProduction(
                brushBuilder,
                coalMinerBuildingInfo,
                "Coal Miner",
                cell => coalMinerBuildingInfo.InstantiateNew(cell, mapBuilder));


            AddProduction(
                brushBuilder,
                converterBuildingInfo,
                "Water Cleaner",
                cell => converterBuildingInfo.InstantiateNew(cell, mapBuilder));
        }

        void AddProduction(BrushBuilder brushBuilder, BuildingInfoBase buildingInfo, string buttonLabel, UnityAction<Vector2Int> buildAction)
        {
            PointBrush newBrush = new PointBrush(pipeBrush, brushBuilder, buildingInfo.GetCellsMask());
            staticBrushes.Add(newBrush);
            buildingBrushPanel.AddProductionButton(buttonLabel, () => activeBrush = newBrush);
            newBrush.Build += buildAction;
        }

        void Update()
        {
            activeBrush?.LateUpdate();
        }
    }

}