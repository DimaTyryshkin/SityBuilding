using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	public class Scene : MonoBehaviour
	{
		[SerializeField] Camera playerCamera;
		[SerializeField] Camera enemyCamera;

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
				SwitchCamera();
		}

		[Button()]
		void SwitchCamera()
		{
			if (playerCamera.gameObject.activeSelf)
			{
				playerCamera.gameObject.SetActive(false);
				enemyCamera.gameObject.SetActive(true);
			}
			else
			{
				playerCamera.gameObject.SetActive(true);
				enemyCamera.gameObject.SetActive(false);
			}
		}
	}
}