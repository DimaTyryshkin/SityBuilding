using System.Collections;
using GamePackages.Core.Validation; 
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class CharacterHpDebug : MonoBehaviour
    {
        [SerializeField, IsntNull]
        CharacterViewRootData view;

        [SerializeField, IsntNull]
        CharacterHp hp;
        
        void Start()
        {
            Assert.IsNotNull(view);
            hp.Damaged += OnDamaged;
        }

        void OnDamaged()
        {
            StartCoroutine(ShowIcon());
        }

        IEnumerator ShowIcon()
        {
            view.debugRedBox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            view.debugRedBox.SetActive(false);
        }
    }
}