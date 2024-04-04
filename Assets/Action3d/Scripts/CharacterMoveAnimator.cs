using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class CharacterMoveAnimator : MonoBehaviour
	{
		static readonly int speedForwardHash = Animator.StringToHash("SpeedForward");
		static readonly int speedRightHash = Animator.StringToHash("SpeedRight");
		static readonly int jumpHash = Animator.StringToHash("IsJump");
		static readonly int shotHash = Animator.StringToHash("Shot");

		[SerializeField, IsntNull] CharacterMotor motor;
		[SerializeField, IsntNull] Gun gun;
		[SerializeField, IsntNull] Animator animator;
		[SerializeField, IsntNull] float factorF =1;
		[SerializeField, IsntNull] float factorR =1;

		
		void Start()
		{
			gun.Shot += OnGun_Shoot;
		}

		void OnGun_Shoot()
		{
			animator.SetTrigger(shotHash);
		}

		void Update()
		{
			animator.SetFloat(speedForwardHash, motor.ForAnimator.forwardSpeed * factorF);
			animator.SetFloat(speedRightHash, motor.ForAnimator.rightSpeed * factorR);
			//animator.SetBool(jumpHash, motor.ForAnimator.isJump);
		}
	}
}