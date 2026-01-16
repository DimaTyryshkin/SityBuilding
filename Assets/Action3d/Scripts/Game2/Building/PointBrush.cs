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
        readonly Vector3Int[] cellsMask;
        readonly Vector3Int[] cells;
        readonly RaycastHit[] raycastHits;
        readonly BuildingListBase buildingList;
        CellCast cast;
        Transform buildingFantom;


        public event UnityAction<Vector3Int> Build;

        public PointBrush(BrushInject inject, BuildingListBase buildingList)
        {
            Assert.IsNotNull(inject);
            Assert.IsNotNull(buildingList);

            this.buildingList = buildingList;
            this.inject = inject;
            cellsMask = buildingList.GetCellsMask();
            cells = new Vector3Int[cellsMask.Length];
            raycastHits = new RaycastHit[1];
            cast = new CellCast();
        }

        public override void LateUpdate()
        {
            if (inject.guiHit.IsGuiUnderPointer)
                return;

            Ray ray = new Ray(inject.thisCamera.transform.position, inject.thisCamera.transform.forward);
            int count = Physics.RaycastNonAlloc(ray, raycastHits, 10);

            buildingFantom.gameObject.SetActive(count > 0);
            if (count == 0)
                return;

            //Vector3 worldPoint = data.thisCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
            Vector3Int cell = inject.grid.WorldPointToCell(raycastHits[0].point);
            for (int i = 0; i < cells.Length; i++)
                cells[i] = cellsMask[i] + cell;

            Vector3 pos = inject.grid.CellToWorldPoint(cell);
            //data.cellMarkerAvailable.transform.position = pos;
            //data.cellMarkerLock.transform.position = pos;



            bool isFree = true;
            foreach (var c in cells)
            {
                inject.gridContent.CastNonAllocate(c, ref cast);
                if (!cast.IsFree)
                {
                    isFree = false;
                    break;
                }
            }


            buildingFantom.position = inject.grid.CellToWorldPoint(cell);
            var allRenderer = buildingFantom.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in allRenderer)
                renderer.material.color = isFree ? Color.green : Color.red;

            //data.cellMarkerAvailable.SetActive(isFree);
            //data.cellMarkerLock.SetActive(!isFree);

            if (isFree)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Build.Invoke(cell);
                }
            }


            if (Input.GetMouseButton(1))
                Remove(cells);
        }

        public override void OnEnableBrush()
        {
            GameObject go = Object.Instantiate(buildingList.Prefab.gameObject);
            buildingFantom = go.transform;

            foreach (var c in go.GetComponentsInChildren<Collider>())
                Object.DestroyImmediate(c);
        }

        public override void OnDisableBrush()
        {
            if (buildingFantom)
                Object.DestroyImmediate(buildingFantom);
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