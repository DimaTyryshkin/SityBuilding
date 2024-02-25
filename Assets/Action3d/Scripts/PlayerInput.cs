using UnityEngine;

namespace Game
{
	public class PlayerInput : MonoBehaviour
	{
		[SerializeField] float rotationSensitivity;
		[SerializeField] CharacterMove characterMove; 
		
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
		}
	}
}