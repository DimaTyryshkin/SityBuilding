using System;
using UnityEngine;

namespace Game
{
	public class Bullet : MonoBehaviour
	{
		[SerializeField] float speed;
		[SerializeField] GameObject hit;
		[SerializeField] TrailRenderer trailRenderer;
		Vector3 point;
		float stopTime;
		Vector3 pointNormal;
		bool hasHit;

		bool skipFrame;
		float trailTimeLife;

		public void SetTarget(Vector3 point, Vector3 pointNormal, bool hasHit)
		{
			trailTimeLife = 0;
			skipFrame = false;
			this.point = point; 
			this.pointNormal = pointNormal; 
			this.hasHit = hasHit; 
			stopTime = Time.time + Vector3.Distance(transform.position, point) / speed;
		}

		void LateUpdate()
		{
			if (!skipFrame)
			{
				skipFrame = true;
				return;
			}

		
			
			
			transform.position = Vector3.MoveTowards(transform.position, point, Time.deltaTime * speed);
			if (Time.time <= stopTime)
			{
				if (trailTimeLife < 0.3f)
				{
					trailTimeLife += Time.deltaTime * 0.5f;
					trailRenderer.time = trailTimeLife;
				}
			}
			else
			{
				//trailRenderer.emitting = false;
				//transform.position += dir * (Time.deltaTime * speed);
				Destroy(gameObject,5);
				if (hasHit)
				{
					var hitGo = Instantiate(hit, point, Quaternion.LookRotation(pointNormal));
					hitGo.SetActive(true);
				}

				enabled = false;
			}
		}
	}
}