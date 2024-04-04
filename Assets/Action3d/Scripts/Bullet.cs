using System;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Game
{ 
	public class Bullet : MonoBehaviour
	{ 
		public const float MaxDistance = 200;
		[SerializeField] float speed;
		[SerializeField] GameObject hitViewPrefab;
		[SerializeField] TrailRenderer trailRenderer;
		[SerializeField] LayerMask layerMask;
	 
		float stopTime;   
		bool skipFrame;
		float trailTimeLife;
		Vector3 dir; 
		
		public void SetDir(Vector3 dir)
		{
			this.dir = dir.normalized;
			trailTimeLife = 0;
			skipFrame = false; 
			stopTime = Time.time + MaxDistance / speed;
		}

		void Update()
		{
			if (!skipFrame)
			{
				skipFrame = true;
				return;
			}

			float distance = Time.deltaTime * speed;
			HitInfo hit = HitUtils.RayCast(transform.position, dir, layerMask, distance);
			
			transform.position = Vector3.MoveTowards(transform.position, hit.point, distance);

			if (hit.collider)
			{  
				if(!(hit.collider is TerrainCollider))
					hit.point = hit.collider.ClosestPoint(hit.point);
					
				GameObject hitView = Instantiate(hitViewPrefab, hit.point, Quaternion.LookRotation(hit.normal));
				hitView.transform.SetParent(hit.collider.transform, true);
				hitView.SetActive(true);
				
				
				
				var damageable = hit.GetDamageable();
				if (damageable != null)
				{
					damageable.ApplyDamage(new Damage()
					{
						damage = 1,
						damageDir = dir,
						worldPoint = hit.point
					});
				}
				
				enabled = false;
				Destroy(gameObject,5);
			}
			else
			{
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
					Destroy(gameObject,5); 
					enabled = false;
				}
			} 
		} 
	}
}