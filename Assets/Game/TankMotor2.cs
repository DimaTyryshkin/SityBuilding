using System;
using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Core
{
	public class TankMotor2 : MonoBehaviour
	{
		public const float MToKm = 3.6f;
		public const float KmToM = 1f / MToKm;
		
		
		[SerializeField] WheelCollider[] leftWheelColliders;
		[SerializeField] WheelCollider[] rightWheelColliders;
		WheelCollider[] wheelColliders;
        
		[SerializeField] float maxSpeed;
		[SerializeField] float motorTorque;
		[SerializeField] float breakTorque;

		[Header("Rotation")] 
		[SerializeField] float maxRotationSpeed;
		[SerializeField] float rotationMotorTorque;
		[SerializeField] AnimationCurve rotationFactorFromK;
        
		[Space]
		[SerializeField] NavMeshAgent navMeshAgent;
		[SerializeField] Rigidbody thisRigidbody;
		[SerializeField] Camera thisCamera;
 
		[SerializeField] bool isMove = true;

		NavMeshPath path;
		int actualPathCornerIndex;
		
		float[] steerFactor =new []{1f,1f,-1f,-1f};

		void Start()
		{
			wheelColliders = leftWheelColliders.Concat(rightWheelColliders).ToArray();
			navMeshAgent.isStopped = true;
			navMeshAgent.updatePosition = false;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updateUpAxis = false;

			path = new NavMeshPath();
			actualPathCornerIndex = -1;
			gizmosValuers = new List<string>();
		}

		void Update()
		{
			if (Input.GetKey(KeyCode.Space))
				Time.timeScale = 0.1f;
			else
				Time.timeScale = 1f;
		}

		void FixedUpdate()
		{
			gizmosValuers.Clear();
			float forwardInput = Input.GetAxis("Vertical");
			float rotationInput = Input.GetAxis("Horizontal");

			FixedUpdateNavigationMove();
		}

		void FixedUpdateNavigationMove()
		{
			if (actualPathCornerIndex < 0)
			{
				Break();
				return;
			}

			Vector3 pathPoint = path.corners[actualPathCornerIndex];
			pathPoint.y = 0;
			
			Vector3 forward = thisRigidbody.transform.forward;
			forward.y = 0;

			Vector3 flatPosition = thisRigidbody.transform.position;
			flatPosition.y = 0;
			Vector3 dir = pathPoint - flatPosition; 

			float deltaAngle = Vector3.SignedAngle(forward, dir, thisRigidbody.transform.up);
			float absDeltaAngle = Mathf.Abs(deltaAngle);
			float flatDistance = Vector3.Distance(flatPosition, pathPoint);


			float forwardInput = 0;
			if (absDeltaAngle < 10 || (flatDistance < 3) && absDeltaAngle < 90)
			{
				if (isMove)
					forwardInput = 1;
			}
			
			FixedUpdateInput(forwardInput,deltaAngle * 0.1f);
		 
		 
		  
			Vector3 pathDir = pathPoint - path.corners[actualPathCornerIndex - 1];
			Vector3 toTarget = pathPoint - transform.position; 
			if (isMove && Vector3.Dot(pathDir,toTarget)<0)
			{
				actualPathCornerIndex++;

				if (actualPathCornerIndex >= path.corners.Length)
					actualPathCornerIndex = -1;
			}
		}

		void FixedUpdateInput(float forwardInput, float rotationInput)
		{
			float absSpeed = thisRigidbody.velocity.magnitude;

			if (absSpeed < 1f * KmToM && forwardInput == 0)
			{
				if (rotationInput != 0)
				{
					RotateWithoutMove(rotationInput);
				}
				else
				{
					Break();
				}
			}
			else
			{
				if (forwardInput == 0 && rotationInput == 0)
				{
					Break();
				}
				else
				{
					Move(forwardInput, rotationInput);
				}
			}
		}

		 

		void Move(float forwardInput, float rotationInput)
		{
			gizmosValuers.Add($"Move");
			Vector3 velocity = thisRigidbody.velocity;
			bool accelerate = forwardInput>0 && velocity.magnitude < maxSpeed;

			float rotationSide = rotationInput > 0 ? 1 : (rotationInput < 0 ? -1 : 0);
			gizmosValuers.Add($"accelerate={accelerate}");
			gizmosValuers.Add($"rotationSide={rotationSide}");
			float torque = accelerate ? motorTorque : 0;
			for (int i = 0; i < wheelColliders.Length / 2; i++)
			{
				leftWheelColliders[i].steerAngle = rotationInput * 15 * steerFactor[i];
				rightWheelColliders[i].steerAngle = rotationInput * 15 * steerFactor[i];
				leftWheelColliders[i].brakeTorque = 0;
				rightWheelColliders[i].brakeTorque = 0;
				leftWheelColliders[i].motorTorque = torque;
				rightWheelColliders[i].motorTorque = torque;
			}
		}
		
		void Break()
		{ 
			gizmosValuers.Add($"Break");
			foreach (var wheel in wheelColliders)
			{ 
					wheel.steerAngle = 0; 
					wheel.motorTorque = 0; 
					wheel.brakeTorque = breakTorque;
			}
		}
 
		void RotateWithoutMove(float yInput)
		{
			float rotationSpeed = thisRigidbody.angularVelocity.magnitude * Mathf.Rad2Deg;
			bool isMaxRotationSpeed = rotationSpeed >= maxRotationSpeed;
			float limitFactor = isMaxRotationSpeed ? 0 : 1;
			
			float rotationSide = yInput > 0 ? 1 : (yInput <0 ? -1 : 0);

			float leftFactor = rotationSide;
			float rightFactor = -rotationSide;

			gizmosValuers.Add($"yInput={yInput}");
			gizmosValuers.Add($"rotationSide={rotationSide}");
			
			float torque = rotationMotorTorque * limitFactor;
			for (int i = 0; i < wheelColliders.Length / 2; i++)
			{
				float steerAngle = Mathf.Clamp(45 * yInput * steerFactor[i], -45, 45);
				gizmosValuers.Add($"i={i} steerAngle={steerAngle}");

				leftWheelColliders[i].steerAngle = steerAngle * leftFactor;
				rightWheelColliders[i].steerAngle = steerAngle * rightFactor;

				leftWheelColliders[i].brakeTorque = 0;
				rightWheelColliders[i].brakeTorque = 0;

				leftWheelColliders[i].motorTorque = torque * leftFactor;
				rightWheelColliders[i].motorTorque =torque  * rightFactor;
			}
		}

		List<String> gizmosValuers;

		void OnDrawGizmos()
		{
			if (path != null && path.corners != null && path.corners.Length > 0 && actualPathCornerIndex >= 0)
			{
				Gizmos.color = Color.white;
				GizmosExtension.DrawLines(path.corners,1);
				
				Vector3 pathPoint = path.corners[actualPathCornerIndex];
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, pathPoint);
			}

		}

		void OnGUI()
		{ 
			if(gizmosValuers==null)
				return;
            
			Vector3 screenPoint = thisCamera.WorldToScreenPoint(transform.position,Camera.MonoOrStereoscopicEye.Mono);

			Rect rect = new Rect(screenPoint.x, screenPoint.y, 500, 500);

			GUILayout.BeginArea(rect);
			GUILayout.BeginVertical();
			{
				for (int i = 0; i < gizmosValuers.Count; i++)
					GUILayout.Label(gizmosValuers[i]);

			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
 

		public void SetPoint(Vector3 point)
		{
			navMeshAgent.Warp(transform.position);
			bool success = navMeshAgent.CalculatePath(point, path);
			//bool success = NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path);

			actualPathCornerIndex = success ? 1 : -1;
		}
        
	}
}