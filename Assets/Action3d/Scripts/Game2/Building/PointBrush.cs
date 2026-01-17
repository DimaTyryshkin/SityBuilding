using GamePackages.Core;
using GamePackages.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game2.Building
{
    public class BrushInject
    {
        public readonly Camera thisCamera;
        public readonly GridContent gridContent;
        public readonly GameGrid grid;
        public readonly GuiHit guiHit;

        public BrushInject(
            Camera thisCamera,
            GridContent gridContent,
            GameGrid grid,
            GuiHit guiHit
        )
        {
            Assert.IsNotNull(thisCamera);
            Assert.IsNotNull(gridContent);
            Assert.IsNotNull(grid);
            Assert.IsNotNull(guiHit);

            this.thisCamera = thisCamera;
            this.gridContent = gridContent;
            this.grid = grid;
            this.guiHit = guiHit;
        }
    }

    public class PointBrush : BuildingBrush
    {
        readonly BrushInject inject;
        readonly RaycastHit[] raycastHits;
        readonly BuildingListBase buildingList;
        CellCast cast;
        BuildingBase buildingFantom;


        public event UnityAction<Vector3Int> Build;

        public PointBrush(BrushInject inject, BuildingListBase buildingList)
        {
            Assert.IsNotNull(inject);
            Assert.IsNotNull(buildingList);

            this.buildingList = buildingList;
            this.inject = inject;
            raycastHits = new RaycastHit[10];
            cast = new CellCast();
        }

        public override void LateUpdate()
        {
            if (inject.guiHit.IsGuiUnderPointer)
                return;

            Assert.IsTrue(buildingFantom);

            Ray ray = new Ray(inject.thisCamera.transform.position, inject.thisCamera.transform.forward);
            int count = Physics.RaycastNonAlloc(ray, raycastHits, 10);
            buildingFantom.gameObject.SetActive(count > 0);
            if (count == 0)
                return;

            Vector3 p = raycastHits.MinItem(static x => x.distance, count).point;
            Vector3Int cell = GameGrid.WorldPointToCell(p.x, p.y + 0.5f, p.z);
            buildingFantom.MoveToCell(cell);

            bool isFree = true;
            foreach (Vector3Int c in buildingFantom.actualCells)
            {
                inject.gridContent.CastNonAllocate(c, ref cast);
                if (!cast.IsFree)
                {
                    isFree = false;
                    break;
                }
            }

            buildingFantom.SetFantomColor(isFree ? Color.green : Color.red);

            if (isFree)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Build.Invoke(cell);
                }
            }

            //if (Input.GetMouseButton(1))
            //    Remove(cells);
        }

        public override void OnEnableBrush()
        {
            buildingFantom = Object.Instantiate(buildingList.Prefab);
            buildingFantom.gameObject.SetActive(true);
            buildingFantom.Init();
            buildingFantom.SetFantomMode();
        }

        public override void OnDisableBrush()
        {
            if (buildingFantom)
            {
                Object.Destroy(buildingFantom);
                buildingFantom = null;
            }
        }

        void Remove(Vector3Int[] cells)
        {
            foreach (var cell in cells)
            {
                inject.gridContent.CastNonAllocate(cell, ref cast);
                foreach (var entity in cast.buildings)
                {
                    //if (entity is PipeItemSource)
                    //    continue;

                    //if (entity is PipeItemDestination)
                    //   continue;

                    //if (entity is PipeBaseElement)
                    //    pipeBrush.RemoveCell(cell);

                    inject.gridContent.Remove(entity);
                }
            }
        }
    }
}