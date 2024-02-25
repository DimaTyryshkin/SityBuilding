using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class TargetMarker : MonoBehaviour,IDamageable
	{
		[SerializeField, IsntNull] Transform disk;
		[SerializeField, IsntNull] AudioSource audioSource;
		[SerializeField, IsntNull] float friction;
		[SerializeField, IsntNull] float rotationFactor;

		float angularSpeed;

		void Start()
		{
			angularSpeed = 0;
		}

		void Update()
		{
			disk.Rotate(0, 0, angularSpeed * Time.deltaTime, Space.Self);
			angularSpeed -= (angularSpeed * friction) * Time.deltaTime;
		}

		public void ApplyDamage(Damage damage)
		{
			Vector3 shoulder = damage.worldPoint - disk.position;
			Vector3 forceMoment = Vector3.Cross(damage.damageDir, shoulder) * rotationFactor;
			float speedAdd = Vector3.Dot(forceMoment, disk.forward);
			angularSpeed += speedAdd;
			
			audioSource.Play();
		}
	}
}