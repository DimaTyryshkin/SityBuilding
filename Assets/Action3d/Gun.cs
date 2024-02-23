using System;
using System.Linq;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public class Gun : MonoBehaviour
	{
		[SerializeField] GameObject bulletPrefab;
		[SerializeField] Bullet bulletPrefab2;
		
		
		[SerializeField] Transform viewCamera;
		[SerializeField] Transform spawnPoint;
		[SerializeField] AudioSource audioSource;
		[SerializeField] Animator gunAnimator;
		[SerializeField] Transform handRoot;
		[SerializeField] AnimationCurve handRootYFromViewAngle;
		[SerializeField] CharacterMove characterMove;
		[SerializeField] float speed;
		[SerializeField] float shotsInSecond;
		[SerializeField] float audioStartTime;
		[SerializeField] float f;
		[SerializeField] float timeLife;

		public event UnityAction Shot;

		float timeNextShot;
		readonly int shotHash = Animator.StringToHash("shot");
		RaycastHit[] raycastHit;

		void Start()
		{
			raycastHit = new RaycastHit[100];
		}

		void Update()
		{
			float angle = characterMove.ViewAngleX;
			if (angle > 90)
				angle -= 360;
			
			float handY = handRootYFromViewAngle.Evaluate(angle); 
			handRoot.localPosition = new Vector3(0, handY, 0);
			
			if (Time.time > timeNextShot && Input.GetKey(KeyCode.Mouse0))
			{

				//ShotBullet();
				ShotLine();
			
				timeNextShot = Time.time + 1f / shotsInSecond;
				audioSource.Stop();
				audioSource.time = audioStartTime;
				audioSource.Play();
				gunAnimator.SetTrigger(shotHash);
				Shot?.Invoke();
			}
		}

		void ShotBullet()
		{
			GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
			bullet.gameObject.SetActive(true);
			Rigidbody rig = bullet.GetComponent<Rigidbody>();
			rig.AddForce(viewCamera.forward * speed, ForceMode.Impulse);
			Destroy(bullet,30);
		}

		void ShotLine()
		{
			int count = Physics.RaycastNonAlloc(viewCamera.position, viewCamera.forward, raycastHit);
			int index = raycastHit.MinIndex(r => r.distance, count);
			
			Vector3 end;
			Vector3 normal;
			if (index == -1)
			{
				normal = Vector3.up;
				end = spawnPoint.position + viewCamera.forward * 2000;
			}
			else
			{
				end = raycastHit[index].point;
				normal = raycastHit[index].normal;
			}

			Vector3 start = spawnPoint.position;
   
			Bullet bullet = Instantiate(bulletPrefab2, start, Quaternion.identity);
			bullet.gameObject.SetActive(true);
			bullet.SetTarget(end, normal, count!=0);
		}
	}
}