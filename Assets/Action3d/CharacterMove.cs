using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
	public class CharacterMove : MonoBehaviour
    {
	    [SerializeField] float speed;
	    [SerializeField] float rotationSensitivity;
	    [SerializeField] float gravityScale;
	    [SerializeField] AnimationCurve jumpCurve;
	    
	    [Space]
	    [SerializeField] CharacterController characterController;
	    [SerializeField] Transform thisCamera;
	    [SerializeField] Transform groundCheck;
	    [SerializeField] LayerMask checkGroundMask;

	    float fallSpeed;
	    Vector3 motionByLegs;
	    Vector3 gravityMotion;
	    Vector3 oldPos;
	    int rotateFrameCounter;
	    bool isGrounded;
	    bool isGroundedForJump;
	    
	    // Jump
	    bool isJump;
	    float jumpStartTime;
	    float jumpEndTime;
	    float jumpStartY;

	    public float SpeedByLegs
	    {
		    get;
		    private set;
	    }
	    
	    public float ViewAngleX
	    {
		    get;
		    private set;
	    }

	    void Start()
	    {
		    oldPos = transform.position;
		    rotateFrameCounter = 3;
	        Cursor.lockState = CursorLockMode.Locked; 
        }


	    void Update()
	    { 
		    Vector3 deltaPos = transform.position - oldPos;
		    fallSpeed = Mathf.Max(fallSpeed, -deltaPos.y / (Time.deltaTime + 0.01f));
			    
		    // Rotate
		    if (rotateFrameCounter < 0)
		    {
			    transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotationSensitivity, 0, Space.Self);
			    thisCamera.Rotate(-Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSensitivity, 0, 0, Space.Self);
			    ViewAngleX = thisCamera.transform.localRotation.eulerAngles.x;
		    }
		    else
		    {
			    rotateFrameCounter--;
		    }


		    // Move
		    if (isGrounded)
		    { 
			    float forwardInput = Input.GetAxis("Vertical");
			    float horizontalInput = Input.GetAxis("Horizontal");

			    motionByLegs = (transform.forward * forwardInput + transform.right * horizontalInput) * speed;
			    SpeedByLegs = motionByLegs.magnitude;
		    }
  
		    if (!isJump)
		    {
			    float add = Time.deltaTime * gravityScale; 
			    fallSpeed += add;
		    }
		    
	        // Lump
	        if (Input.GetKeyDown(KeyCode.Space) )
	        {
		        Debug.Log($"[d] isJump={isJump} isGrounded={isGroundedForJump}");
		        if (!isJump && isGroundedForJump)
		        {
			        isGrounded = false;
			        isGroundedForJump = false;
			        isJump = true;
			        jumpStartY = transform.position.y;
			        jumpStartTime = Time.time;
			        jumpEndTime = jumpStartTime + jumpCurve.keys[jumpCurve.length - 1].time;
		        }
	        }

	      
	        
	        // Result 
		    Vector3 motion = Vector3.zero;
		    motion += motionByLegs * Time.deltaTime;


		    if (isJump)
		    {
			    float jumpTime = Time.time - jumpStartTime;
			    float actualHeight = jumpCurve.Evaluate(jumpTime) + jumpStartY;
			    Vector3 jumpMotion = Vector3.up * Mathf.Max(actualHeight - transform.position.y, 0);
			    motion += jumpMotion;

			    if (Time.time > jumpEndTime)
			    {
				    fallSpeed = 0;
				    isJump = false;
			    }
		    }
		    else
	        { 
		        motion += Vector3.down * (fallSpeed * Time.deltaTime);
	        }

		    oldPos = transform.position;
	        characterController.Move(motion);
	        isGroundedForJump = Physics.CheckSphere(groundCheck.position, 0.6f,checkGroundMask); 
	        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f,checkGroundMask); 
	    } 
    }
}
