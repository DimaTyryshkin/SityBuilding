using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Game
{
	public class EnemyAiInput : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform target;
		[SerializeField, IsntNull] Transform navMeshPipette;
		[SerializeField, IsntNull] Transform thisCamera;
		[SerializeField, IsntNull] CharacterMotor motor;
		[SerializeField, IsntNull] CameraVibration cameraVibration;
		[SerializeField, IsntNull] Gun gun;
		[SerializeField, IsntNull] float rotationSpeed = 100;
		[SerializeField, IsntNull] AnimationCurve shotAngleThresholdFromDistance;
		[SerializeField, IsntNull] bool canShot = true;
		[SerializeField, IsntNull] RangeMinMaxInt shotAmount;
		[SerializeField, IsntNull] int shotDelay = 2;

		Vector3 viewDir;
		int shotCount;
		float timeNextShot;
		
		
		[Header("Kickback")]
		[SerializeField] float kickbackSpeed = 5f; 
		RangeMinMax horizontalKickback;
		RangeMinMax verticalKickback;
		Vector2 deviation;
		
		[Header("Move")]
		[SerializeField, IsntNull] NavMeshAgent navMeshAgent;
		[SerializeField] bool isMove;
		[SerializeField] float wayPintAccuracy= 0.2f;
		NavMeshPath path;
		int actualPathCornerIndex;
		UnityAction navigationCallback;


		void Start()
		{
			shotCount = -1;
			horizontalKickback = new RangeMinMax(
				-cameraVibration.HorizontalKickbackAngle * 3f,
				+cameraVibration.HorizontalKickbackAngle * 3f
			);
			
			verticalKickback = new RangeMinMax(
				+cameraVibration.VerticalKickbackAngle * 3f,
				-cameraVibration.VerticalKickbackAngle * 4f
			); 
			
			viewDir = motor.ViewDir;
			gun.Shot += OnGun_Shot;
			
			// Move
			
			navMeshAgent.isStopped = true;
			navMeshAgent.updatePosition = false;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updateUpAxis = false;

			path = new NavMeshPath();
			actualPathCornerIndex = -1;
		}


		void OnEnable()
		{
			cameraVibration.enabled = false;
		}

		void Update()
		{
			float maxAngle = rotationSpeed * Time.deltaTime;
			viewDir = motor.ViewDir;
			Vector3 toTarget = target.position - thisCamera.position;

			// Perlin
			{
				// float t = Time.time * kickbackSpeed;
				// float horizontalDeviation = MathExtension.MapUnclamped(0, 1, -h, h, Mathf.PerlinNoise(t, 1));
				// float verticalDeviation = MathExtension.MapUnclamped(0, 1, -v, v, Mathf.PerlinNoise(t, 1000));
				// toTarget = Quaternion.AngleAxis(horizontalDeviation, Vector3.up) * toTarget;
				// toTarget = Quaternion.AngleAxis(verticalDeviation, thisCamera.right) * toTarget;
			}

			// Random
			{
				toTarget = Quaternion.AngleAxis(deviation.x, Vector3.up) * toTarget;
				toTarget = Quaternion.AngleAxis(deviation.y, thisCamera.right) * toTarget;
				deviation = Vector3.Lerp(deviation, Vector2.zero, Time.deltaTime * kickbackSpeed);
			}


			// To target
			Vector3 toTargetHorizontal = toTarget;
			toTargetHorizontal.y = 0;

			Vector3 viewDirHorizontal = viewDir;
			viewDirHorizontal.y = 0;

			Vector3 toTargetVertical = Vector3.ProjectOnPlane(toTarget, thisCamera.right);



			// Horizontal Input
			float horizontalAngle = Vector3.SignedAngle(viewDirHorizontal, toTargetHorizontal, Vector3.up);
			float horizontalAngleDelta = Mathf.Clamp(horizontalAngle, -maxAngle, maxAngle);
			motor.RotateHorizontal(horizontalAngleDelta);

			// Vertical Input
			float verticalAngle = Vector3.SignedAngle(viewDir, toTargetVertical, thisCamera.right);
			if (Mathf.Abs(horizontalAngle) < 45)
			{
				float verticalAngleDelta = Mathf.Clamp(verticalAngle, -maxAngle, maxAngle);
				motor.RotateVertical(verticalAngleDelta);
			}

			// Shot
			if (canShot && Time.time > timeNextShot)
			{
				float shotThreshold = shotAngleThresholdFromDistance.Evaluate(toTarget.magnitude);
				if (Mathf.Abs(horizontalAngle) < shotThreshold && Mathf.Abs(verticalAngle) < shotThreshold)
				{
					if (shotCount < 0)
						StartShotCounter();

					//TODO Первый выстрел всегда идеальный, это плохо
					gun.ShotInput();
				}
				else
				{
					shotCount = -1;
				}
			}
			
			// Move
			if (actualPathCornerIndex != -1)
			{
				AiMove();
			}
		}

		

		void OnGun_Shot()
		{
			deviation += new Vector2
			(
				horizontalKickback.Random(), // [-1.5, 1.5] хорошо сомтрится 
				verticalKickback.Random() // [3,-4] хорошо сомтрится
			);
			
			
			shotCount--;
			if (shotCount == 0)
			{
				StartShotCounter();
				timeNextShot = Time.time + shotDelay;
			}
		}

		void StartShotCounter()
		{
			shotCount = shotAmount.Random();
		}


		Vector3 lastInput;
		void OnDrawGizmos()
		{
			if (path != null && path.corners != null && path.corners.Length > 0 && actualPathCornerIndex >= 0)
			{
				Gizmos.color = Color.white;
				GizmosExtension.DrawLines(path.corners,0.05f);
				
				Vector3 pathPoint = path.corners[actualPathCornerIndex];
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, pathPoint);
				Gizmos.color = Color.black;
				Gizmos.DrawLine(transform.position, transform.position + lastInput);
			}

		}

		[SerializeField] bool isDebug;
		
		void AiMove()
		{  
			Vector3 pathPoint = path.corners[actualPathCornerIndex];
			Vector3 toTarget = pathPoint - transform.position;
			toTarget.y = 0;
			lastInput = toTarget.normalized;
			motor.MoveDirInput(lastInput);

			Vector3 delta = pathPoint - navMeshPipette.position;
			Vector3 flatDelta = new Vector3(delta.x, 0, delta.z);
			float flatDistance =  flatDelta.magnitude;
			float heightDistance = Mathf.Abs(delta.y);
			 
			if (isMove && (flatDistance < wayPintAccuracy) && (heightDistance < 0.6f))
			{
				actualPathCornerIndex++;

				if (actualPathCornerIndex >= path.corners.Length)
				{ 
					actualPathCornerIndex = -1;
					ActionWrapper.ClearAndInvoke(ref navigationCallback);
				}
			} 
		}
		
		public void SetDestination(Vector3 point, UnityAction callBack = null)
		{
			navMeshAgent.Warp(transform.position);
			navigationCallback = callBack;
			
			NavMesh.SamplePosition(point, out var hit, 10, NavMesh.AllAreas);
			bool success = navMeshAgent.CalculatePath(hit.position, path);

			actualPathCornerIndex = success ? 1 : -1; 
		}
	}
}
