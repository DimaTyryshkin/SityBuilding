
using GamePackages.Core;
using GamePackages.Localization.EndingOfNumerals;
using UnityEngine;

namespace Game.Core
{
	public class TestInput : MonoBehaviour
	{
		[SerializeField] Camera playerCamera;
		[SerializeField] TankMotor2 tankMotor;
		 
		[SerializeField] float factor;
		[SerializeField] ForceMode forceMode;
		
		[Header("Tower")]
		[SerializeField] AimTurret tower1;
		[SerializeField] AimTurret tower2;
		[SerializeField] Transform target;

		void LateUpdate()
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				Vector3 point = playerCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
				tankMotor.SetPoint(point);
			} 
			
			tower1.SetTarget(target.position);
			tower2.SetTarget(target.position);
		}

		void FixedUpdate()
		{
			Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
			Vector3 up = Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;
			
			Vector3 xInput = Input.GetAxis("Horizontal") *right ;
			Vector3 yInput = Input.GetAxis("Vertical")* up;

			Vector3 input = xInput + yInput;
			if (input.magnitude > 1)
				input = input.normalized;
			
			//box.AddForce(input * factor , forceMode);
		}
	}
}