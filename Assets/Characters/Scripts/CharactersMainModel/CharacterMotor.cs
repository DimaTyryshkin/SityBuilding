using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField, IsntNull]
        CharacterViewData view;
        
        [SerializeField, IsntNull]
        CharacterViewRootData viewRoot;
        
        [SerializeField]
        CharacterMotorSetting motorSetting;

        static public readonly int speedFHash = Animator.StringToHash("SpeedF");
        static public readonly int speedRHash = Animator.StringToHash("SpeedR");

        float currentSpeedF;
        float currentSpeedR;
        float timeMaxSpeed;

        Vector3 input;
        Vector3 dir;

        public void Warp(Vector3 pos)
        {
            viewRoot.characterController.enabled = false;
            transform.position = pos;
            viewRoot.characterController.enabled = true;
        }

        public void SetInput(Vector3 input)
        {
            this.input = input;
        }

        public void SetViewPoint(Vector3 dir)
        {
            this.dir = dir;
        }

        public void Attack()
        {
            view.viewAnimator.SetTrigger("Attack");
            timeMaxSpeed = Time.time + 1;
        }

        void Update()
        {
            // move   
            float currentMaxSpeed = Time.time > timeMaxSpeed ? motorSetting.maxSpeed : motorSetting.maxSpeedInAttack;

            var motion = input * currentMaxSpeed;
            if (motion.magnitude > currentMaxSpeed)
                motion = motion.normalized * currentMaxSpeed;

            viewRoot.characterController.Move(motion * Time.deltaTime);

            // rotation 
            if (dir != Vector3.zero)
            {
                var target = Quaternion.LookRotation(dir);
                view.Root.rotation = Quaternion.RotateTowards(view.Root.rotation, target,    motorSetting.rotationSpeed * Time.deltaTime);
            }


            // Animation 
            Vector3 forward = view.Root.forward;
            Vector3 right = Vector3.Cross(forward, Vector3.up);

            float speedF = Vector3.Dot(input, forward);
            float speedR = Vector3.Dot(input, right);

            currentSpeedF = Mathf.Lerp(currentSpeedF, speedF, Time.deltaTime * motorSetting.animationLerp);
            currentSpeedR = Mathf.Lerp(currentSpeedR, speedR, Time.deltaTime * motorSetting.animationLerp);

            view.viewAnimator.SetFloat(speedFHash, currentSpeedF);
            view.viewAnimator.SetFloat(speedRHash, -currentSpeedR);
        } 
    }
}