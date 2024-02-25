using UnityEngine;

namespace Game
{
	public class Speedometer : MonoBehaviour
	{
		const float MToKm = 3.6f;
		
		Transform target;
		Vector3 lastPosition;

		public Vector3 Velocity { get; private set; }
		public float Speed => Velocity.magnitude;
		public float SpeedKm => Velocity.magnitude * MToKm;
		
		
		public Vector3 OldVelocity { get; private set; }
		public float OldSpeed => OldVelocity.magnitude;
		public float OldSpeedKm => OldVelocity.magnitude * MToKm;

		public static Speedometer Get(Transform target)
		{
			Speedometer speedometer = target.gameObject.GetComponent<Speedometer>();
			
			if (speedometer)
			{
				return speedometer;
			}
			else
			{
				speedometer = target.gameObject.AddComponent<Speedometer>();
				speedometer.target = target;

				return speedometer;
			}
		}

		void Start()
		{
			lastPosition = target.position;
		}

		void LateUpdate()
		{
			float dt = Time.deltaTime;
			Vector3 newPosition = target.position;

			if (!Mathf.Approximately(dt, 0))
			{
				OldVelocity = Velocity;
				Velocity = (newPosition - lastPosition) / dt;
			}

			lastPosition = newPosition;
		}
	}
}