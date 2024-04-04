using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class CharacterHp : MonoBehaviour, IDamageable
    {  
        [SerializeField]
        int maxHp;

        [SerializeField]
        bool isPlayer;
        
        public bool IsDeath => isDeath;
        public bool IsPlayer => isPlayer;
		
        public event UnityAction Death;
        public event UnityAction Damaged;

        int hp;
        bool isDeath;

        void Start()
        { 
            hp = maxHp;
        }

        public void Reset()
        {
            hp = maxHp;
            isDeath = false;
        }
 
        public void ApplyDamage(Damage damage)
        {
            if(isDeath)
                return;
			
            hp -= damage.damage;

            if (hp <= 0)
            {
                hp = 0;
                isDeath = true;
                Death?.Invoke();
                //Destroy(this);
            }
            else
            {
                Damaged?.Invoke();
            }
        }
    }
}