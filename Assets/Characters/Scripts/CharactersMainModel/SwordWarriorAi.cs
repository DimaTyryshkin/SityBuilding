using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class SwordWarriorAi : MonoBehaviour
    {
        public Transform target;
        
        
        Character character;

        float minAttackDistance, maxAttackDistance;
        float attackDistance;
        Vector3 input;
        float timeNextAttack;

        public void Init(Transform target, Character character, float minAttackDistance, float maxAttackDistance)
        {
            Assert.IsNotNull(target);
            Assert.IsNotNull(character);
            this.target = target;
            this.character = character;
            this.minAttackDistance = minAttackDistance;
            this.maxAttackDistance = maxAttackDistance;

            character.AttackFinish += OnAttackFinish;
            
            GetAttackDistance();
        }

        void OnAttackFinish()
        {
            timeNextAttack = Time.time + 1f;
        }

        void Update()
        {
            var dir = target.position - transform.position;
            dir.y = 0;

            Vector3 rawInput = dir.normalized;

            if (Time.time < timeNextAttack)
            {
                if(dir.magnitude < attackDistance *2)
                {
                    rawInput *= -1;
                //rawInput *= 0;
                }
            }

            input = rawInput;
            input = Vector3.Lerp(input, rawInput, Time.deltaTime * 0.1f);
            character.SetInput(input, dir);

            if (dir.magnitude < attackDistance)
            {
                if (character.CanAttack)
                { 
                    character.Attack();
                    GetAttackDistance();
                }
            }
        }

        void GetAttackDistance()
        {
            attackDistance = Random.Range(minAttackDistance, maxAttackDistance);
        }
    }
}