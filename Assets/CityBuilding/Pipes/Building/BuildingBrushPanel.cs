using Game.Building;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.GameGui
{
    public class BuildingBrushPanel : MonoBehaviour
    {
        [SerializeField, IsntNull] Toggle pipeToggle;
        [SerializeField, IsntNull] Toggle productionToggle;

        [Space] [SerializeField, IsntNull] GameObject productionPanel;
        [SerializeField, IsntNull] Toggle minerToggle;
        [SerializeField, IsntNull] Toggle converterToggle;
        [SerializeField, IsntNull] Toggle storeToggle;
        [SerializeField, IsntNull] Toggle destinationToggle;

        public event UnityAction<BuildingType> SelectBrush;


        void Start()
        {
            pipeToggle.onValueChanged.AddListener(OnClickPipe);
            productionToggle.onValueChanged.AddListener(OnClickProduction);

            productionPanel.SetActive(false);
            storeToggle.onValueChanged.AddListener(OnClickStore);
            destinationToggle.onValueChanged.AddListener(OnClickDestination);
            minerToggle.onValueChanged.AddListener(OnClickMiner);
            converterToggle.onValueChanged.AddListener(OnClickConverter);

            pipeToggle.isOn = true;
            //pipeToggle.OnPointerClick(new PointerEventData(null));
        }

        void OnClickConverter(bool isOn)
        {
            if (isOn)
                SelectBrush?.Invoke(BuildingType.ItemConverter);
        }

        void OnClickMiner(bool isOn)
        {
            if (isOn)
                SelectBrush?.Invoke(BuildingType.ItemMine);
        }

        void OnClickDestination(bool isOn)
        {
            if (isOn)
                SelectBrush?.Invoke(BuildingType.Destination);
        }

        void OnClickStore(bool isOn)
        {
            if (isOn)
                SelectBrush?.Invoke(BuildingType.Source);
        }

        void OnClickPipe(bool isOn)
        {
            if (isOn)
            {
                productionPanel.SetActive(false);
                SelectBrush?.Invoke(BuildingType.Pipe);
            }
        }

        void OnClickProduction(bool isOn)
        {
            if (isOn)
                productionPanel.SetActive(true);
        }
    }
}
