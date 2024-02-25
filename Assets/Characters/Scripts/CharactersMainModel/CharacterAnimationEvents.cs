using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class CharacterAnimationEvents : MonoBehaviour
    {   
        public event UnityAction StartAttack;
        public event UnityAction EndAttack;
        public event UnityAction MiddleAttack;
		  
        public void AttackStart()
        {
            StartAttack?.Invoke();
        }

        public void AttackEnd()
        {
            EndAttack?.Invoke();
        }
        
        public void AttackMiddle()
        {
            MiddleAttack?.Invoke();
        }
    }
}