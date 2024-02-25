using System;
using System.Collections;
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
		[SerializeField] AudioSource otherAudioSource;
		[SerializeField] Animator gunAnimator;
		[SerializeField] Animator handsAnimator;
		[SerializeField] Transform handRoot;
		[SerializeField] AnimationCurve handRootYFromViewAngle;
		[SerializeField] CharacterMove characterMove;
		[SerializeField] float speed;
		[SerializeField] float shotsInSecond;
		[SerializeField] float audioStartTime;  
		[SerializeField] int bulletsMax;
		
		
		[SerializeField] AudioClip noBulletClip;
		[SerializeField] AnimationClip reloadClip;

		public event UnityAction Shot;

		bool isReloading;
		float timeNextShot;
		int bullets;
		readonly int shotHash = Animator.StringToHash("shot");
		readonly int reloadHash = Animator.StringToHash("reload"); 

		void Start()
		{
			isReloading = false;
			bullets = bulletsMax; 
		}

		void Update()
		{
			float angle = characterMove.ViewAngleX;
			if (angle > 90)
				angle -= 360;

			float handY = handRootYFromViewAngle.Evaluate(angle);
			handRoot.localPosition = new Vector3(0, handY, 0);

			if (!isReloading && bullets < bulletsMax && Input.GetKeyDown(KeyCode.R))
			{ 
				StartReload();
				return;
			}

			if (!isReloading && Input.GetKey(KeyCode.Mouse0))
			{ 
				if (Time.time > timeNextShot)
				{
					if (bullets > 0)
					{
						bullets--;
						//ShotBullet();
						ShotLine();

						timeNextShot = Time.time + 1f / shotsInSecond;
						audioSource.Stop();
						audioSource.time = audioStartTime;
						audioSource.Play();
						gunAnimator.SetTrigger(shotHash);

						Shot?.Invoke();
					}
					else
					{
						if (!isReloading)
						{
							otherAudioSource.PlayOneShot(noBulletClip);
							StartReload();
						}
					}
				} 
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

		void StartReload()
		{
			if(isReloading)
				return;
 
			StartCoroutine(Reload());
		}

		IEnumerator Reload()
		{ 
			isReloading = true;
			handsAnimator.SetTrigger(reloadHash); 
			yield return new WaitForSeconds(reloadClip.length);
			bullets = bulletsMax;
			isReloading = false;
		}

		void ShotLine()
		{
			HitInfo hit = HitUtils.RayCast(viewCamera.position, viewCamera.forward);
			Vector3 dir = hit.point - spawnPoint.position;

			Bullet bullet = Instantiate(bulletPrefab2, spawnPoint.position, Quaternion.identity);
			bullet.gameObject.SetActive(true);
			bullet.SetDir(dir);
		}
	}
}