using GamePackages.Core;
using UnityEngine;

namespace Game.Core
{
	public class TestInput2 : MonoBehaviour
	{
		[SerializeField] Camera playerCamera;
	 
		TankMotor2 tankMotor;
		AimTurret[] turrets;

		void LateUpdate()
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] raycastHits = Physics.RaycastAll(ray);

				foreach (var hit in raycastHits)
				{
					var rigidbody = hit.rigidbody;
					if (rigidbody)
					{
						var motor = rigidbody.GetComponent<TankMotor2>();
						if (motor)
						{
							tankMotor = motor;
							turrets = motor.GetComponentsInChildren<AimTurret>();
						}
					}
				}
			}

			if (tankMotor && Input.GetKey(KeyCode.Mouse1))
			{
				Vector3 point = playerCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
				tankMotor.SetPoint(point);

				foreach (var turret in turrets)
					turret.SetTarget(point + Vector3.up);
			} 
		}
	}
}