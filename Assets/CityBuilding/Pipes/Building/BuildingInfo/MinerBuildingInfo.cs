using Game.Json;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game.Building
{

    public abstract class MinerBuildingInfo : BuildingInfo<ItemMiner, ItemMinerJson>
    {
        [SerializeField] string resourceName;
        [SerializeField] Vector2Int sourceOffset;
        [SerializeField, IsntNull] SourceBuildingInfo sources;

        protected override ItemMinerJson PrintToJson(ItemMiner item)
        {
            return new ItemMinerJson()
            {
                cell = item.Cell,
                resourceName = item.itemName,
                pipeSourceId = sources.itemToId[item.pipeItemSource]
            };
        }

        protected override void RemoveInternal(ItemMiner value)
        {
            sources.Remove(value.pipeItemSource);
        }

        protected override void InitAfterInstFormSave(ItemMiner item, Vector2Int cell, ItemMinerJson json, MapBuilder mapBuilder)
        {
            item.Init(json.resourceName, sources.idToItem[json.pipeSourceId]);
        }

        public void InstantiateNew(Vector2Int cell, MapBuilder mapBuilder)
        {
            var outSourceCell = cell + sourceOffset;

            PipeItemSource source = sources.InstantiateNewSource(outSourceCell, resourceName, mapBuilder);

            ItemMiner newItem = InstantiateAsNew(cell, mapBuilder);
            newItem.Init(resourceName, source);
        }
    }
}