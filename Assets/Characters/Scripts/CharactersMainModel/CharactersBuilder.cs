using System;
using GamePackages.Core;
using GamePackages.Core.Validation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class CharactersBuilder : MonoBehaviour
    {
        [SerializeField, IsntNull()]
        GameObject playerPrefab;

        [SerializeField, IsntNull()]
        GameObject zombiePrefab;

        Camera sceneCamera;

        Character player;
        Transform root;

        public void Init(Camera sceneCamera)
        {
            Assert.IsNotNull(sceneCamera);

            this.sceneCamera = sceneCamera;

            var go = new GameObject("Root");
            root = go.transform;
        }

        public Character SpawnPlayer(Vector3 point)
        {
            GameObject player = root.InstantiateAsChild(playerPrefab);
            CharacterMotor motor = player.GetComponent<CharacterMotor>();
            motor.Warp(point);

            Character character = InitPlayer(player);
             
            player.SetActive(true);
            player.gameObject.name = "Player";
            return character;
        }

        public Character InitPlayer(GameObject player)
        { 
            Character character = player.GetComponent<Character>();
            Assert.IsNotNull(character);
              
            var go = new GameObject("MouseAndKeyboardInput");
            go.transform.SetParent(root);

            var followCamera = sceneCamera.AddComponent<FollowCamera>();
            followCamera.Init(sceneCamera, player.transform);
            
            var input = go.AddComponent<MouseAndKeyboardInput>();
            input.Init(followCamera.Camera, character);
            this.player = character; 
            return character;
        }

        public Character BuildZombie(Vector3 pos)
        {
            Character character = BuildEnemy(pos);
            GetZombieAi(character.gameObject, player.transform);

            return character;
        }

        Character BuildEnemy(Vector3 pos)
        {
            GameObject zombie = root.InstantiateAsChild(zombiePrefab);

            CharacterMotor motor = zombie.GetComponent<CharacterMotor>();
            Enemy character = zombie.GetComponent<Enemy>();

            motor.Warp(pos);

            zombie.SetActive(true);
            return character;
        }

        void GetZombieAi(GameObject character, Transform target)
        {
            ZombieAi ai = character.GetComponent<ZombieAi>();
            ai.Init(target);
        }
    }
}