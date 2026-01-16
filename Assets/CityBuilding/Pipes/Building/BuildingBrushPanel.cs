using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Building
{
    public class BuildingBrushPanel : MonoBehaviour
    {
        [SerializeField, IsntNull] ToggleGroup toggleGroup;
        [SerializeField, IsntNull] Toggle pipeToggle;
        [SerializeField, IsntNull] Toggle productionToggle;

        [Space]
        [SerializeField, IsntNull] GameObject productionPanel;
        [SerializeField, IsntNull] Toggle togglePrefab;

        public event UnityAction SelectPipe;


        public void Init()
        {
            pipeToggle.onValueChanged.AddListener(OnClickPipe);
            productionToggle.onValueChanged.AddListener(OnClickProduction);
            productionPanel.SetActive(false);
            productionPanel.transform.DestroyChildren();
        }

        public void AddProductionButton(string text, UnityAction onSelect)
        {
            var toggle = productionPanel.transform.InstantiateAsChild(togglePrefab);
            toggle.gameObject.SetActive(true);
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(isOn => onSelect.Invoke());

            toggle.GetComponentInChildren<Text>().text = text;
        }

        void OnClickPipe(bool isOn)
        {
            if (isOn)
            {
                productionPanel.SetActive(false);
                SelectPipe?.Invoke();
            }
        }

        void OnClickProduction(bool isOn)
        {
            if (isOn)
                productionPanel.SetActive(true);
        }
    }
}
