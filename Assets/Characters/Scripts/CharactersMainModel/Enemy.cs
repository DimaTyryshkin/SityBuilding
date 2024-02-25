using System.Collections;
using UnityEngine; 

namespace Game
{
    public class Enemy : Character
    {
        Coroutine corotine;

        protected override void InitInternal()
        {
            base.InitInternal();
            
            hp.Damaged += OnDamaged; 
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            view.renderer.material.color = Color.gray;
            
            if (corotine != null)
                StopCoroutine(corotine);
        }

        void OnDamaged()
        {
            if (corotine != null)
                StopCoroutine(corotine);

            corotine = StartCoroutine(Hit());
        }

        IEnumerator Hit()
        {
            canInput = false; 
            motor.SetInput(Vector3.zero);
         
            //view.viewAnimator.SetFloat(CharacterMotor.speedFHash,0);
            view.viewAnimator.SetBool("BrakeAttack",true);
            view.viewAnimator.SetTrigger("Hit");
            yield return new WaitForSeconds(0.2f); 
            view.viewAnimator.SetBool("BrakeAttack",false);

            canInput = true;
            canAttack = true;
            corotine = null;
        }
    }
}