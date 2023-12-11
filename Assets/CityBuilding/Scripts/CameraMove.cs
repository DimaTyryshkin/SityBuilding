using UnityEngine;

namespace Game
{
    public class CameraMove : MonoBehaviour
    {
	    [SerializeField] float moveSensitiveFactor; 
	    [SerializeField] float scaleSensitiveFactor;
	    [SerializeField] Transform thisCamera;

	    float distance;
	    float minDistance;

	    void Start()
	    {
		    distance = Vector3.Distance(transform.position, thisCamera.position);
		    minDistance = 2;
	    }

	    void LateUpdate()
        {
	        distance -= distance * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scaleSensitiveFactor;
	        distance = Mathf.Clamp(distance, minDistance, 100);
	        thisCamera.localPosition = -thisCamera.forward * distance;
	        
	        float horizontalInput = Input.GetAxis("Horizontal");
	        float verticalInput = Input.GetAxis("Vertical"); 
	        Vector3 input = new Vector3(horizontalInput, 0, verticalInput); 
	        transform.position += input * (Time.deltaTime * moveSensitiveFactor * distance); 
        }
    }
}
