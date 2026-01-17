using GamePackages.Core;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game2.Building
{
    public abstract class BuildingBase : MonoBehaviour
    {
        [NonSerialized] public Vector3Int actualCell;
        [NonSerialized] public List<Vector3Int> actualCells;

        [NonSerialized] bool IsDrawGizmos;

        public Vector3Int[] CellMask { get; private set; }
        public Vector3 OffsetFromCellToPivot { get; private set; }


        public abstract void Delinking();

        private void OnDrawGizmosSelected()
        {
            if (!IsDrawGizmos)
                return;

            Init();

            //GizmosExtension.DrawBounds(transform.GetTotalRendererBounds()); 

            Bounds bound = BoundsExtension.Encompass(actualCells.Select(p => new Bounds(p + Vector3.one * 0.5f, Vector3.one)).ToArray());
            GizmosExtension.DrawBounds(bound);

            foreach (var cell in actualCells)
            {
                Gizmos.DrawSphere(GameGrid.CellToWorldPoint(cell), 0.2f);
            }
        }

        [Button]
        public void DrawGizmos()
        {
            IsDrawGizmos = true;
            Init();
            Vector3Int cell = GameGrid.WorldPointToCell(transform.position - OffsetFromCellToPivot);
            MoveToCell(cell);
        }

        public void Init()
        {
            Bounds bounds = transform.GetTotalRendererBounds();

            int xCellSize = (int)Mathf.Ceil(bounds.size.x);
            int yCellSize = (int)Mathf.Ceil(bounds.size.y);
            int zCellSize = (int)Mathf.Ceil(bounds.size.z);

            int xFirst = (xCellSize - 1) / 2;
            int yFirst = 0;
            int zFirst = (zCellSize - 1) / 2;

            List<Vector3Int> cells = new List<Vector3Int>(xCellSize * yCellSize * zCellSize);
            for (int x = 0; x < xCellSize; x++)
                for (int y = 0; y < yCellSize; y++)
                    for (int z = 0; z < zCellSize; z++)
                    {
                        Vector3Int cell = new Vector3Int(
                            x - xFirst,
                            y - yFirst,
                            z - zFirst
                            );

                        cells.Add(cell);
                    }

            CellMask = cells.ToArray();


            Vector3 basePoint = bounds.center;
            basePoint.y -= bounds.extents.y;
            Vector3 offsetFromCellToPivot = transform.position - basePoint;
            offsetFromCellToPivot.y -= 0.5f;// чтобы здание встало на нижную грань ячейки
            offsetFromCellToPivot.x += xCellSize % 2 == 0 ? 0.5f : 0;
            offsetFromCellToPivot.z += zCellSize % 2 == 0 ? 0.5f : 0;
            OffsetFromCellToPivot = offsetFromCellToPivot;
        }

        public void MoveToCell(Vector3Int cell)
        {
            Assert.IsNotNull(CellMask);
            Assert.IsTrue(CellMask.Length > 0);

            transform.position = GameGrid.CellToWorldPoint(cell) + OffsetFromCellToPivot;
            actualCell = cell;
            actualCells = TranslateCellsMask(cell);
        }

        public List<Vector3Int> TranslateCellsMask(Vector3Int offset)
        {
            List<Vector3Int> newMask = new List<Vector3Int>(CellMask.Length);

            for (int i = 0; i < CellMask.Length; i++)
                newMask.Add(CellMask[i] + offset);

            return newMask;
        }

        internal void SetFantomColor(UnityEngine.Color color)
        {
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i].color = color;
                    //TODO Emission
                }

                renderer.materials = mats;
            }
        }

        internal void SetFantomMode(Material material)
        {
            foreach (var c in GetComponentsInChildren<Collider>())
                DestroyImmediate(c);

            foreach (var a in GetComponentsInChildren<Animator>())
                DestroyImmediate(a);

            foreach (var mb in GetComponentsInChildren<MonoBehaviour>())
            {
                if (mb == this)
                {
                    mb.enabled = false;
                }
                else
                {
                    DestroyImmediate(mb);
                }
            }

            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = material;

                renderer.materials = mats;
            }
        }
    }
}