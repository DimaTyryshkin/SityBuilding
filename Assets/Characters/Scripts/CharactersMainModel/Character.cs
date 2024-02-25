using System.Collections;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game
{
    public class Character : MonoBehaviour
    {
        [SerializeField, IsntNull]
        protected CharacterHp hp;

        [SerializeField, IsntNull]
        protected CharacterViewData view;

        [SerializeField, IsntNull]
        protected CharacterMotorOld motor;

        [SerializeField, IsntNull]
        protected MeleeAttack meleeAttack;

        [SerializeField, IsntNull]
        CharacterViewRootData characterRoot;

        [SerializeField]
        protected CharacterSetting settings;

        public bool CanAttack => canAttack;

        public event UnityAction DeathAnimationFinish;
        public event UnityAction AttackFinish;

        //state
        protected bool canInput;
        protected bool canAttack;

        void Start()
        {
            InitInternal();
        }

        protected virtual void InitInternal()
        {
            hp.Death += OnDeath;
            canInput = true;
            canAttack = true;

            view.characterAnimationEvents.StartAttack += OnStartAttack;
            view.characterAnimationEvents.EndAttack += OnEndAttack;
            view.characterAnimationEvents.MiddleAttack += OnMiddleAttack;
        }

        private void OnMiddleAttack()
        {
            meleeAttack.Attack(transform.position, view.transform.forward, settings.attackDistance, hp);
        }

        void OnStartAttack()
        {
        }

        void OnEndAttack()
        {
            canAttack = true;
            AttackFinish?.Invoke();
        }

        public void SetInput(Vector3 input, Vector3 dir)
        {
            if (canInput)
            {
                motor.SetInput(input);

                //if(canAttack) //В момент атаки персонаж не модет крутиться
                motor.SetViewPoint(dir);
            }
        }

        public void Attack()
        {
            if (!canInput)
                return;

            if (!canAttack)
                return;

            canAttack = false;
            motor.Attack();
        }

        protected virtual void OnDeath()
        {
            canInput = false;
            characterRoot.characterController.enabled = false;
            motor.enabled = false;
            StartCoroutine(DeathAnimation());
        }

        IEnumerator DeathAnimation()
        {
            motor.SetInput(Vector3.zero);
            view.viewAnimator.SetTrigger("Kill");
            yield return new WaitForSeconds(1.5f);
            DeathAnimationFinish?.Invoke();
        }
    }
}