using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public class FollowCamera : MonoBehaviour
	{ 
		Camera thisCamera;
		public Transform target;

		public Camera Camera => thisCamera;
		
		Vector3 deviation;

		public void Init(Camera camera, Transform target)
		{
			Assert.IsNotNull(camera);
			Assert.IsNotNull(target);
			thisCamera = camera;
			this.target = target;
			
			deviation = target.position - transform.position;
		}

		void LateUpdate()
		{
			transform.position = target.position - deviation;
		}
	}
}