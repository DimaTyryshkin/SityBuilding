using GamePackages.Core;
using GamePackages.Core.Validation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game2.Building
{
    public class ButtonsPanel : MonoBehaviour
    {
        [SerializeField, IsntNull] Button buttonPrefab;
        [SerializeField, IsntNull] GameObject productionPanel;

        public void Init()
        {
            productionPanel.SetActive(true);
            productionPanel.transform.DestroyChildren();
        }

        public void AddButton(string text, UnityAction onClick)
        {
            Button button = productionPanel.transform.InstantiateAsChild(buttonPrefab);
            button.gameObject.SetActive(true);
            button.onClick.AddListener(onClick);
            button.GetComponentInChildren<TMP_Text>().text = text;
        }
    }
}
