using System;
using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class HandsAnimation : MonoBehaviour
	{
		[SerializeField, IsntNull] AudioClip magazineHideClip;
		[SerializeField, IsntNull] AudioClip pullGateClip;
		[SerializeField, IsntNull] AudioSource audioSource;
		[SerializeField, IsntNull] GameObject magazinePrefab;
		[SerializeField, IsntNull] Transform magazineOrigin;
		[SerializeField, IsntNull] Transform forwardProvider;
		[SerializeField, IsntNull] float  force;



		Speedometer speedometer;

		void Start()
		{
			speedometer = Speedometer.Get(transform);
		}

		public void OnMagazineHide() 
		{ 
			audioSource.PlayOneShot(magazineHideClip);

			GameObject newMagazine = Instantiate(magazinePrefab, magazineOrigin.position, magazineOrigin.rotation);
			newMagazine.SetActive(true);
			newMagazine.GetComponent<Rigidbody>().AddForce(forwardProvider.forward * force, ForceMode.Impulse);
			newMagazine.GetComponent<Rigidbody>().AddForce(speedometer.Velocity, ForceMode.Impulse);
		}

		public void OnPullGate()
		{
			audioSource.PlayOneShot(pullGateClip);
		}
	}
}