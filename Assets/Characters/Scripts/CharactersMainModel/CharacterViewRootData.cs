using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    public class CharacterViewRootData : MonoBehaviour
    {
        [IsntNull]
        public CharacterController characterController;
        
        [IsntNull]
        public Transform view;
        
        [IsntNull]
        public GameObject debugRedBox;
        
        [IsntNull]
        public GameObject debugAttackMarker; 
		 
        void Start()
        { 
            debugRedBox.SetActive(false);
            debugAttackMarker.SetActive(false);
        }
    }
}