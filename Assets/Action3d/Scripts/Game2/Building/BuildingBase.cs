using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    public abstract class BuildingBase : MonoBehaviour
    {
        [NonSerialized] public List<Vector3Int> cells;
        public Vector3Int Cell { get; set; }
        public abstract void Delinking();
    }
}