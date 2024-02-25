﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
	public class CameraVibration : MonoBehaviour
	{
		[SerializeField] float speed;
		[SerializeField] float scale; 
		
		[Header("Kickback")]
		[SerializeField] float horizontalKickbackAngle; 
		[SerializeField] float verticalKickbackAngle; 
		[SerializeField] AnimationCurve kickbackCurve;

		[Space]
		[SerializeField] CharacterMove move;
		[SerializeField] Transform viewCamera;  
		[SerializeField] Transform cameraRotationRoot; 
		[SerializeField] Gun gun;

		float time;
		float lastHorizontalKickbackAngle;
		bool isShot;
		float shotStartTime;
		float shotEndTime;
		float lastShotAngle;

		void Start()
		{
			gun.Shot += OnGun_Shot;
		}

		void Update()
		{
			if (move.SpeedByLegs > 0.1f)
			{
				viewCamera.localPosition = new Vector3(Mathf.Sin(time * speed), 0, 0) * scale;
				time += Time.deltaTime;
			}
			else
			{
				time = 0;
			}
			
			if (isShot)
			{
				float shotTime = Time.time - shotStartTime;
				float angle = - kickbackCurve.Evaluate(shotTime);
				float deltaAngle = angle - lastShotAngle;
				lastShotAngle = angle;
				cameraRotationRoot.Rotate(deltaAngle * verticalKickbackAngle,0,0,Space.Self);
				transform.Rotate(0, deltaAngle * lastHorizontalKickbackAngle, 0, Space.Self);
				if (Time.time > shotEndTime)
					isShot = false;
			}
		}
		
		void OnGun_Shot()
		{
			isShot = true;
			lastShotAngle = 0;
			shotStartTime = Time.time;
			shotEndTime = shotStartTime + kickbackCurve.keys[kickbackCurve.length - 1].time;
			lastHorizontalKickbackAngle = Random.Range(-horizontalKickbackAngle, horizontalKickbackAngle);
		}
	}
}