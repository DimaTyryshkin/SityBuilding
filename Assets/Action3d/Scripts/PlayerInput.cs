using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{  
	public class PlayerInput : MonoBehaviour
	{
		[SerializeField, IsntNull] float rotationSensitivity;
		[SerializeField, IsntNull] CharacterMotor characterMove; 
		[SerializeField, IsntNull] Gun gun; 
		
		int rotateFrameCounter;
  
		void Start()
		{ 
			rotateFrameCounter = 3; 
			Cursor.lockState = CursorLockMode.Locked; 
		}

		void Update()
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
				characterMove.Move(new Vector2(forwardInput, horizontalInput));
			}

			if (Input.GetKeyDown(KeyCode.Space))
				characterMove.Jump();
			
			if(Input.GetKeyDown(KeyCode.R))
				gun.ReloadInput();
			
			if(Input.GetMouseButton(0))
				gun.ShotInput();
		}
	}
}