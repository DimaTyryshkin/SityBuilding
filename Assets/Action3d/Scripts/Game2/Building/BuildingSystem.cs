using GamePackages.Core.Validation;
using GamePackages.InputSystem;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField, IsntNull] Transform cellMarkersRoot;
        [SerializeField, IsntNull] Material fantomMaterial;

        [Header("Building List")]
        [SerializeField, IsntNull] WallList wallList;
        [SerializeField, IsntNull] MetallMiningCampList metallMiningCampList;


        CellMarkerList[] cellMarkerLists;

        List<PointBrush> brushes;
        BuildingBrush activeBrush;


        public void Init()
        {
            CellMarker[] cellMarkers = cellMarkersRoot.GetComponentsInChildren<CellMarker>();
            cellMarkerLists = cellMarkers
                .GroupBy(m => m.Marker)
                .Select(
                group => new CellMarkerList(
                    group.Key,
                    group.Select(m => m.Cell).ToArray())
                ).ToArray();

            BuildingListBase[] allBuildingLists = new BuildingListBase[]
            {
                wallList,
                metallMiningCampList
            };

            foreach (BuildingListBase buildingList in allBuildingLists)
                buildingList.Init();

            buildingBrushPanel.Init();
            gridContent.Init(allBuildingLists, cellMarkerLists);

            BrushInject brushInject = new BrushInject(
                cameraForBuilding,
                gridContent,
                grid,
                guiHit,
                fantomMaterial
            );

            activeBrush = null;

            brushes = new List<PointBrush>();
            SetBrush(AddButton(
                new PointBrush(brushInject, wallList),
                "Wall",
                cell =>
                {
                    wallList.InstantiateAsNew(cell, gridContent);
                }));

            AddButton(
                new MiningCampBrush(brushInject, metallMiningCampList, CellMarkerValue.MetallMiningCamp),
                "Metal Mining",
                cell =>
                {
                    metallMiningCampList.InstantiateAsNew(cell, gridContent);
                });

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

            buildingBrushPanel.AddButton("None", () => SetBrush(null));
        }

        PointBrush AddButton(PointBrush brush, string buttonLabel, UnityAction<Vector3Int> buildAction)
        {
            brushes.Add(brush);
            buildingBrushPanel.AddButton(buttonLabel, () => SetBrush(brush));
            brush.Build += buildAction;
            return brush;
        }

        void SetBrush([CanBeNull] PointBrush brush)
        {
            if (activeBrush != null)
                activeBrush.OnDisableBrush();

            activeBrush = brush;
            brush?.OnEnableBrush();
        }

        void Update()
        {
            activeBrush?.LateUpdate();
        }
    }

}