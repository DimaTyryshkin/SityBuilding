using System;
using GamePackages.Core;
using GamePackages.Core.Validation;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public class CameraMove : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform thisCamera;
		[SerializeField] RangeMinMax cameraOffset;
		[SerializeField] float scrollSensitivity = 1;
		[SerializeField] float scrollLerpFactor = 1;
		[SerializeField] float moveSensitivity = 1;

		float targetCameraOffset;
		float actualCameraOffset;

		void Start()
		{
			targetCameraOffset = Vector3.Distance(thisCamera.position, transform.position);
			targetCameraOffset = cameraOffset.Clamp(targetCameraOffset);
			actualCameraOffset = targetCameraOffset;
		}

		void Update()
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			targetCameraOffset *= (1 - scroll * scrollSensitivity);
			targetCameraOffset = cameraOffset.Clamp(targetCameraOffset);
			actualCameraOffset = Mathf.Lerp(actualCameraOffset, targetCameraOffset, Time.time * scrollLerpFactor);
			thisCamera.localPosition = thisCamera.forward * (-actualCameraOffset);
			
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");


			Vector3 forward = thisCamera.forward;
			forward.y = 0;
			forward.Normalize();
			
			Vector3 input = thisCamera.right * h + forward * v;
			if (input.magnitude > 1)
				input = input.normalized;

			transform.position += input * (moveSensitivity * actualCameraOffset * Time.deltaTime);
		}
	}
}