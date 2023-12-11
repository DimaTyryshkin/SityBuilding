using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Core
{
	public class AimTurret : MonoBehaviour
	{
		[SerializeField] bool yRotation;
		[SerializeField] bool xRotation;
		[SerializeField] float rotationSpeed; 
		[SerializeField] AimTurret dependOnTurret; 
 
		Vector3 originAngles;
		Vector3 dir;
		
		float targetAngleY;
		float targetAngleX;

		float angleX;
		float angleY;


		public float DeltaAngle { get; private set; }

		void Start()
		{
			originAngles = transform.localEulerAngles;
			targetAngleY = originAngles.y;
			targetAngleX = originAngles.x;
			angleX = targetAngleX;
			angleY = targetAngleY;
			DeltaAngle = 0;
		}

		public void SetTarget(Vector3 worldPoint)
		{   
			dir = worldPoint - transform.position;

			if (dir.sqrMagnitude < 0.1f)
				return;

			Vector3 forward = transform.parent.forward;

			if (yRotation)
			{ 
				Vector3 up = transform.up; 
				Vector3 flatDir = Vector3.ProjectOnPlane(dir, up);
				targetAngleY = originAngles.y + Vector3.SignedAngle(forward, flatDir, up); 
			}

			if (xRotation)
			{
				Vector3 right = transform.right;
				Vector3 flatDir = Vector3.ProjectOnPlane(dir, right);
				targetAngleX = originAngles.x + Vector3.SignedAngle(forward, flatDir, right);
			}
		}

		void Update()
		{
			DeltaAngle = 0;
			if (yRotation)
			{
				angleY = Mathf.MoveTowardsAngle(angleY, targetAngleY, rotationSpeed * Time.deltaTime);
				DeltaAngle += Mathf.Abs(Mathf.DeltaAngle(angleY, targetAngleY));
			}

			if (xRotation)
			{
				float yDelta = dependOnTurret.DeltaAngle;
				float targetX = yDelta > 45 ? originAngles.x : targetAngleX;// Чтобы ствол не задирался вверх и вниз
				angleX = Mathf.MoveTowardsAngle(angleX, targetX, rotationSpeed * Time.deltaTime);
				DeltaAngle += Mathf.Abs(Mathf.DeltaAngle(angleX, targetAngleX));
			} 
			
			transform.localRotation = Quaternion.Euler(angleX, angleY, 0); 
		} 
	}
}