using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    public class CharacterViewData : MonoBehaviour
    {
        [IsntNull]
        public Animator viewAnimator;
        
        [IsntNull]
        public Renderer renderer;
        
        [IsntNull]
        public CharacterAnimationEvents characterAnimationEvents;

        public Transform Root => transform; 
    }
}