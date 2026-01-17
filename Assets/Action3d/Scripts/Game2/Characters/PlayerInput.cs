using GamePackages.Core.Validation;
using System.Collections;
using UnityEngine;

namespace Game
{
    public abstract class PlayerInput : MonoBehaviour
    {
        [SerializeField, IsntNull] float rotationSensitivity;
        [SerializeField, IsntNull] CharacterMotor characterMove;

        bool isEnableInput;

        protected virtual void Start()
        {
            isEnableInput = false;
            StartCoroutine(WaitAndEnable());
        }

        IEnumerator WaitAndEnable()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            isEnableInput = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void RotateAndMove()
        {
            // Rotate
            {

                float horizontalInput = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSensitivity;
                float verticalInput = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSensitivity;

                //viewDir = Quaternion.Euler(new Vector3(0, xInput, 0)) * viewDir;
                //viewDir = Quaternion.AngleAxis(yInput, transform.right) * viewDir;
                characterMove.RotateHorizontal(horizontalInput);
                characterMove.RotateVertical(verticalInput);


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
            if (isEnableInput)
            {
                RotateAndMove();
                UpdteInternal();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isEnableInput = !isEnableInput;
                Cursor.lockState = isEnableInput ? CursorLockMode.Locked : CursorLockMode.None; //TODO 
            }
        }

        protected abstract void UpdteInternal();
    }
}