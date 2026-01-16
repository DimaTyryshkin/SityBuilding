using Game2.Building;
using GamePackages.Audio;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game2
{
    public class Scene : MonoBehaviour
    {
        [SerializeField, IsntNull] Camera playerCamera;
        [SerializeField, IsntNull] Camera enemyCamera;
        [SerializeField, IsntNull] AppSounds appSounds;
        [SerializeField, IsntNull] BuildingSystem buildingSystem;

        [Header("Denug")]
        [SerializeField, Range(0.1f, 1)] float timeScale = 1;


        private void Start()
        {
            appSounds.Init(new AppAudioAccountData());
            AppSounds.SetAsSceneSound(appSounds);

            buildingSystem.Init();
        }

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

        [Button()]
        void SetTimeScale()
        {
            Time.timeScale = timeScale;
        }
    }
}