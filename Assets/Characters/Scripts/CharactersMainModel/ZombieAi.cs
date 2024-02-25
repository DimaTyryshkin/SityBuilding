using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Game
{
    public class ZombieAi : MonoBehaviour
    { 
        [SerializeField]
        Character character;

        [SerializeField]
        float minAttackDistance;
        
        [SerializeField]
        float maxAttackDistance;
         
        [NonSerialized]
        public Transform target;
        
        float attackDistance;
 
        public void Init(Transform target)
        {
            Assert.IsNotNull(target);
            this.target = target; 
            GetAttackDistance();
        }

        void Update()
        {
            var dir = target.position - transform.position;
            dir.y = 0;

            Vector3 input = dir.normalized;
            if (input.magnitude > 1)
                input = input.normalized;

            character.SetInput(input, dir);
			
            if (dir.magnitude < attackDistance)
            {
                GetAttackDistance();
                character.Attack();
            }
        }
		
        void GetAttackDistance()
        {
            attackDistance = Random.Range(minAttackDistance, maxAttackDistance);
        }
    }
}