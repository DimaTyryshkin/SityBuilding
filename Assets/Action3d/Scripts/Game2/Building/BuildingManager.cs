using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game2.Building
{
    public class BuildingSystem : MonoBehaviour
    {
        [SerializeField, IsntNull] GridContent gridContent;
        [SerializeField, IsntNull] ButtonsPanel buildingBrushPanel;
        [SerializeField, IsntNull] Camera cameraForBuilding;
        [SerializeField, IsntNull] GameGrid grid;
        [SerializeField, IsntNull] GuiHit guiHit;

        [Header("Building info")]
        [SerializeField, IsntNull] WallList wallList;

        List<PointBrush> staticBrushes;
        BuildingBrush activeBrush;


        public void Init()
        {
            wallList.Init();
            buildingBrushPanel.Init();

            gridContent.Init(new BuildingListBase[]
            {
                wallList,
            });

            BrushInject brushBuilder = new BrushInject(
                cameraForBuilding,
                gridContent,
                grid,
                guiHit
            );

            //pipeBrush = new PipeBrush(pipeBuildingInfo, crossBuildingInfo, brushBuilder);

            activeBrush = null;
            //buildingBrushPanel.SelectPipe += () => activeBrush = pipeBrush;

            staticBrushes = new List<PointBrush>();


            SetBrush(AddProduction(
                brushBuilder,
                wallList,
                "Wall",
                cell =>
                {
                    Debug.Log("BuildAction");
                    wallList.InstantiateAsNew(cell, gridContent);
                }));

            //AddProduction(
            //    brushBuilder,
            //    coalMinerBuildingInfo,
            //    "Coal Miner",
            //    cell => coalMinerBuildingInfo.InstantiateNew(cell, mapBuilder));


            //AddProduction(
            //    brushBuilder,
            //    converterBuildingInfo,
            //    "Water Cleaner",
            //    cell => converterBuildingInfo.InstantiateNew(cell, mapBuilder));
        }

        PointBrush AddProduction(BrushInject brushInject, BuildingListBase buildingInfo, string buttonLabel, UnityAction<Vector3Int> buildAction)
        {
            PointBrush newBrush = new PointBrush(brushInject, buildingInfo);
            staticBrushes.Add(newBrush);
            buildingBrushPanel.AddButton(buttonLabel, () => SetBrush(newBrush));
            newBrush.Build += buildAction;
            return newBrush;
        }

        void SetBrush(PointBrush brush)
        {
            if (activeBrush != null)
                activeBrush.OnDisableBrush();

            activeBrush = brush;
            brush.OnEnableBrush();
        }

        void Update()
        {
            activeBrush?.LateUpdate();
        }
    }

}