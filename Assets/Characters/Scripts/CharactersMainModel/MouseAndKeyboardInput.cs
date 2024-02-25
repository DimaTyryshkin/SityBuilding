using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class MouseAndKeyboardInput : MonoBehaviour
    {
        Camera playerCamera;
        Character character;

        [SerializeField]
        bool alwaysForward;

        public void Init(Camera playerCamera, Character character)
        {
            Assert.IsNotNull(playerCamera);
            Assert.IsNotNull(character);
            this.playerCamera = playerCamera;
            this.character = character;
        }

        void Update()
        {
            // Move
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(horizontal, 0, vertical);


            // rotation 
            Vector3 dir;

            if (alwaysForward)
            {
                dir = Vector3.forward;
            }
            else
            {
                Vector3 viewPoint = playerCamera.ScreenPointToWorldPointOnPlane(Input.mousePosition, Plaine.XZ);
                Vector3 characterPos = character.transform.position;
                viewPoint.y = 0;
                characterPos.y = 0;
                dir = viewPoint - characterPos;
            }

            character.SetInput(input, dir);

            // attack
            if (Input.GetMouseButtonDown(0))
                character.Attack();
        }
    }
}