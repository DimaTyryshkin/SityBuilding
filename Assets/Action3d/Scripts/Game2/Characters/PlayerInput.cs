using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
    public abstract class PlayerInput : MonoBehaviour
    {
        [SerializeField, IsntNull] float rotationSensitivity;
        [SerializeField, IsntNull] CharacterMotor characterMove;

        int rotateFrameCounter;

        protected virtual void Start()
        {
            rotateFrameCounter = 3;
            Cursor.lockState = CursorLockMode.Locked; //TODO 
        }

        void RotateAndMove()
        {
            // Rotate
            {
                if (rotateFrameCounter < 0)
                {
                    float horizontalInput = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSensitivity;
                    float verticalInput = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSensitivity;

                    //viewDir = Quaternion.Euler(new Vector3(0, xInput, 0)) * viewDir;
                    //viewDir = Quaternion.AngleAxis(yInput, transform.right) * viewDir;
                    characterMove.RotateHorizontal(horizontalInput);
                    characterMove.RotateVertical(verticalInput);
                }
                else
                {
                    rotateFrameCounter--;
                }
            }

            //Move
            {
                float forwardInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");
                characterMove.MoveInput(new Vector2(forwardInput, horizontalInput));
            }

            if (Input.GetKeyDown(KeyCode.Space))
                characterMove.Jump();
        }
        void Update()
        {
            RotateAndMove();
            UpdteInternal();
        }

        protected abstract void UpdteInternal();
    }
}