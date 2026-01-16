using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2.Building
{
    [Serializable]
    public class MapJson
    {
        public List<WallJson> wallsA = new List<WallJson>();
        //public List<PipeJson> pipes = new List<PipeJson>();
        //public List<CrossJson> cross = new List<CrossJson>();
        //public List<SourceJson> sources = new List<SourceJson>();
        //public List<DestinationJson> destinations = new List<DestinationJson>();
        //public List<ItemMinerJson> dirtWaterMiners = new List<ItemMinerJson>();
        //public List<ItemMinerJson> coalMiners = new List<ItemMinerJson>();
        //public List<ItemConverterJson> waterCleanerConverters = new List<ItemConverterJson>();
    }

    public abstract class BuildingJson
    {
        public int id;

        public abstract Vector3Int Cell { get; }
    }

    [Serializable]
    public class WallJson : BuildingJson
    {
        public Vector3Int cell;
        public int roration;
        public override Vector3Int Cell => cell;
    }


    // === OLD ====

    [Serializable]
    public class PipeJson : BuildingJson
    {
        public List<Vector3Int> cells = new List<Vector3Int>();

        public Vector3Int Start => cells[0];
        public Vector3Int End => cells[^1];

        public Vector3Int GetCenter()
        {
            Vector3Int center = cells[0];
            for (int i = 1; i < cells.Count; i++)
                center += cells[i];

            return center / cells.Count;
        }

        public override Vector3Int Cell => cells[0];
    }

    [Serializable]
    public class CrossJson : BuildingJson
    {
        public Vector3Int cell;
        public List<int> inPipes = new List<int>();
        public List<int> outPipes = new List<int>();

        public override Vector3Int Cell => cell;
    }

    [Serializable]
    public class SourceJson : BuildingJson
    {
        public Vector3Int cell;
        public int pipe;

        public string resourceName;

        public override Vector3Int Cell => cell;
    }

    [Serializable]
    public class DestinationJson : BuildingJson
    {
        public Vector3Int cell;
        public int pipe;

        public string resourceName;
        public int amount;
        public int maxAmount;

        public override Vector3Int Cell => cell;
    }

    [Serializable]
    public class ItemMinerJson : BuildingJson
    {
        public Vector3Int cell;
        public string resourceName;
        public int pipeSourceId;

        public override Vector3Int Cell => cell;
    }

    [Serializable]
    public class ItemConverterJson : BuildingJson
    {
        public Vector3Int cell;
        public int inDestinationId;
        public int outSourceId;

        public override Vector3Int Cell => cell;
    }
}