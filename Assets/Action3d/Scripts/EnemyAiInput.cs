using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class EnemyAiInput : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform target;
		[SerializeField, IsntNull] Transform thisCamera;
		[SerializeField, IsntNull] CharacterMotor motor;
		[SerializeField, IsntNull] CameraVibration cameraVibration;
		[SerializeField, IsntNull] Gun gun;
		[SerializeField, IsntNull] float rotationSpeed = 100;
		[SerializeField, IsntNull] AnimationCurve shotAngleThresholdFromDistance;
		[SerializeField, IsntNull] bool canShot = true;

		Vector3 viewDir;
		
		
		[Header("Kickback")]
		[SerializeField] float kickbackSpeed = 5f; 
		RangeMinMax horizontalKickback;
		RangeMinMax verticalKickback;
		Vector2 deviation;


		void Start()
		{
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
			if(canShot)
			{
				float shotThreshold = shotAngleThresholdFromDistance.Evaluate(toTarget.magnitude);
				if (Mathf.Abs(horizontalAngle) < shotThreshold && Mathf.Abs(verticalAngle) < shotThreshold)
					gun.ShotInput();
			}
		}

		void OnGun_Shot()
		{
			deviation += new Vector2
			(
				horizontalKickback.Random(), // [-1.5, 1.5] хорошо сомтрится 
				verticalKickback.Random() // [3,-4] хорошо сомтрится
			);
		}
	}
}
