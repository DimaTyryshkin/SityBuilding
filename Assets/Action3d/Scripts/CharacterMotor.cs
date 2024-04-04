using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;

namespace Game
{
	public class CharacterMotor : MonoBehaviour
    {
	    [Space]
	    [SerializeField] float speed;
	    [SerializeField] float gravityScale;
	    [SerializeField] float inputLerpFactor = 1;
	    [SerializeField] AnimationCurve jumpCurve;
	    
	    [Space]
	    [SerializeField] CharacterController characterController;
	    [SerializeField] Transform thisCamera;
	    [SerializeField] Transform groundCheck;
	    [SerializeField] LayerMask checkGroundMask;


	    public ValuesForAnimator ForAnimator => valuesForAnimator;
	    
	    ValuesForAnimator valuesForAnimator;
	    float fallSpeed;
	    Vector3 motionByLegs;
	    Vector3 gravityMotion;
	    Vector3 oldPos;
	    bool isGrounded;
	    bool isGroundedForJump;
	    int rotateFrameCounter;
	    
	    // Jump
	    bool isJump;
	    float jumpStartTime;
	    float jumpEndTime;
	    float jumpStartY;
	    
	    // Input
	    Vector2 rotationInput;
	    Vector2 moveInputValue;
	    Vector2 moveInput;
	    Vector3 moveDirInput;
	    bool dirInputMode;
	    bool jumpInput;
	    

	    public float SpeedByLegs
	    {
		    get;
		    private set;
	    }
	    
	    public float ViewAngleVertical
	    {
		    get;
		    private set;
	    }

	    public Vector3 ViewDir => thisCamera.forward;

	    void Start()
	    {
		    rotateFrameCounter = 3; 
		    valuesForAnimator = new ValuesForAnimator();
		    oldPos = transform.position; 
        }

 
	    public void RotateHorizontal(float angle) => rotationInput.x+=angle; 
	    public void RotateVertical(float angle) => rotationInput.y+=angle;

	    public void MoveInput(Vector2 moveInput)
	    {
		    dirInputMode = false;
		    this.moveInput += moveInput;
	    }

	    public void MoveDirInput(Vector3 moveDirInput)
	    {
		    dirInputMode = true;
		    this.moveDirInput += moveDirInput;
	    }
	    
	    public void Jump() => jumpInput = true;

	    Vector3 lastMotionByLegs;
	    void OnDrawGizmos()
	    {
		    Gizmos.color = Color.red;
		    Gizmos.DrawLine(transform.position , transform.position + lastMotionByLegs);
	    }

	    void Update()
	    { 
		    Vector3 deltaPos = transform.position - oldPos;
		    fallSpeed = Mathf.Max(fallSpeed, -deltaPos.y / (Time.deltaTime + 0.01f));

		
		      
		    // Rotate 1
		    //Vector3 viewDirXZ = viewDir;
		    //viewDirXZ.y = 0;
		    //transform.LookAt(transform.position + viewDirXZ);
		    //thisCamera.LookAt(thisCamera.position + viewDir);
		    
		    // Rotate 2
		    if (rotateFrameCounter < 0)
		    { 
			    transform.Rotate(0, rotationInput.x, 0, Space.Self);
			    thisCamera.Rotate(rotationInput.y, 0, 0, Space.Self);
			    float xAngle = thisCamera.transform.eulerAngles.x; 
			    if (xAngle is > 80 and <= 180)
			    {
				    xAngle = 80;
				    thisCamera.transform.localEulerAngles = new Vector3(xAngle,0,0);
			    }
			    else if (xAngle is < 360 - 80 and > 180)
			    {
				    xAngle = (360 - 80);
				    thisCamera.transform.localEulerAngles = new Vector3(xAngle, 0, 0);
			    }

			    ViewAngleVertical = xAngle;
			    rotationInput = Vector2.zero;
		    }
		    else
		    {
			    rotateFrameCounter--;
		    }
		    
		    // Move
		    if (isGrounded)
		    {
			    float forwardInput = 0;
			    float horizontalInput = 0;

			    if (dirInputMode)
			    {
				    forwardInput = Vector3.Dot(moveDirInput, transform.forward);
				    horizontalInput = Vector3.Dot(moveDirInput, transform.right);
			    }
			    else
			    {
				    if (moveInput.magnitude > 0)
					    moveInput.Normalize();

				    forwardInput = Mathf.Clamp(moveInput.x, -1, 1);
				    horizontalInput = Mathf.Clamp(moveInput.y, -1, 1);
			    }
 
			    Vector2 newMoveInputValue = new Vector2(forwardInput, horizontalInput);
			    moveInputValue = Vector2.MoveTowards(moveInputValue, newMoveInputValue, inputLerpFactor * Time.deltaTime);
			    
			    
			    valuesForAnimator.forwardSpeed = moveInputValue.x;
			    valuesForAnimator.rightSpeed = moveInputValue.y;

			    lastMotionByLegs = (transform.forward * moveInputValue.x + transform.right * moveInputValue.y) * speed;
			    motionByLegs = lastMotionByLegs;
			    SpeedByLegs = motionByLegs.magnitude;
			    
			    moveDirInput = Vector3.zero;
			    moveInput = Vector2.zero;
		    }

		    if (!isJump)
		    {
			    float add = Time.deltaTime * gravityScale; 
			    fallSpeed += add;
		    }
		    
	        // Lump
	        if (jumpInput)
	        { 
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

	        valuesForAnimator.isJump = isJump;
	        jumpInput = false;

	      
	        
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
	    
	    public class ValuesForAnimator
	    {
		    public float forwardSpeed;
		    public float rightSpeed;
		    public bool isJump;
	    }
    }
}
