using System;
using System.Collections.Generic;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.AI; 

namespace Game.Core
{
    public class TankMotor : MonoBehaviour
    {
        [SerializeField] float maxSpeed;
        [SerializeField] float acceleration;
        
        [Header("Rotation")]
        [SerializeField] float maxAngularSpeed;
        [SerializeField] float rotationFactor;
        [SerializeField] float stopAngle;
        [SerializeField] AnimationCurve rotationCurve;
        //[SerializeField] float targetAngularSpeed;
        [SerializeField] ForceMode forceMode = ForceMode.Acceleration;
        
        [Space]
        [SerializeField] NavMeshAgent navMeshAgent;
        [SerializeField] Rigidbody thisRigidbody;
        [SerializeField] Camera thisCamera;
 
        [SerializeField] bool isMove = true;
        


        NavMeshPath path;
        int actualPathCornerIndex;

        void Start()
        {
            // navMeshAgent.isStopped = true;
            // navMeshAgent.updatePosition = false;
            // navMeshAgent.updateRotation = false;
            // navMeshAgent.updateUpAxis = false;

            path = new NavMeshPath();
            actualPathCornerIndex = -1;
            gizmosValuers = new List<string>();
        }

        void FixedUpdate()
        {
            gizmosValuers.Clear();

            if (actualPathCornerIndex < 0)
                return;

            Vector3 pathPoint = path.corners[actualPathCornerIndex];
            float angle = Rotate2(pathPoint);

            if (isMove && Mathf.Abs(angle) < 2)
                Move(pathPoint);
 
            gizmosValuers.Add($"actualPathCornerIndex = {actualPathCornerIndex}");
            Vector3 pos = transform.position;
            pathPoint.y = 0;
            pos.y = 0;

            Vector3 pathDir = pathPoint - path.corners[actualPathCornerIndex - 1];
            Vector3 toTarget = pathPoint - transform.position;
            
            
            if (isMove && Vector3.Dot(pathDir,toTarget)<0)
            {
                actualPathCornerIndex++;

                if (actualPathCornerIndex >= path.corners.Length)
                    actualPathCornerIndex = -1;
            }
        }

        void Move(Vector3 wordPoint)
        {
            wordPoint.y = 0;
            
            gizmosValuers.Add($"-- Move --");
            Vector3 velocity = thisRigidbody.velocity;
            if (velocity.magnitude > maxSpeed)
                return;

            Vector3 dir = wordPoint - transform.position;
            dir.y = 0;
            
            Vector3 targetVelocity = dir.normalized * maxSpeed;
            Vector3 add = targetVelocity - velocity;
 
            gizmosValuers.Add($"targetVelocity=={targetVelocity.magnitude}");
            gizmosValuers.Add($"add=={add.magnitude}");
            
            float resultAcceleration = add.magnitude / (Time.fixedDeltaTime + 0.001f);
            gizmosValuers.Add($"resultAcceleration=={resultAcceleration}");
            gizmosValuers.Add($"acceleration=={acceleration}");
            if (resultAcceleration > acceleration)
            {
                float k = acceleration / resultAcceleration;
                add *= k;
                gizmosValuers.Add($"k=={k}");
            }

            gizmosValuers.Add($"add={add.magnitude}");
            thisRigidbody.AddForce(add, forceMode);
        }

        float Rotate(Vector3 worldPoint)
        {
            Vector3 forward = thisRigidbody.transform.forward;
            Vector3 dir = worldPoint - thisRigidbody.transform.position;

            float deltaAngle = Vector3.SignedAngle(forward, dir, thisRigidbody.transform.up);

            float targetAngularSpeed = Mathf.Sign(deltaAngle) * maxAngularSpeed;

            if (Mathf.Abs(deltaAngle) < 2)
            {
                deltaAngle = 0;
                targetAngularSpeed = 0;
            }
            
         
           

            float k = Mathf.Abs(targetAngularSpeed * Time.fixedDeltaTime) / Mathf.Abs(deltaAngle);
            if (k >= 1f)
                targetAngularSpeed = deltaAngle / Time.fixedDeltaTime;
            
          

            float actualAngularSpeed = thisRigidbody.angularVelocity.y * Mathf.Rad2Deg;
            float angularDelta = targetAngularSpeed - actualAngularSpeed;

           


            float normalizerAngularSpeed = actualAngularSpeed / (targetAngularSpeed + 0.01f);
            var factor = rotationCurve.Evaluate(normalizerAngularSpeed);

            float add = angularDelta * factor * rotationFactor;
            thisRigidbody.AddRelativeTorque(0, add * Mathf.Deg2Rad, 0, forceMode);
            return deltaAngle;
        }
        
        float Rotate2(Vector3 worldPoint)
        {
            Vector3 forward = thisRigidbody.transform.forward;
            Vector3 dir = worldPoint - thisRigidbody.transform.position;
            dir.y = 0;
            forward.y = 0;
            dir = dir.normalized;
            forward = forward.normalized;

            float deltaAngle = Vector3.SignedAngle(forward, dir, thisRigidbody.transform.up);
            float targetAngularSpeed = Mathf.Sign(deltaAngle) * maxAngularSpeed;

            if (Mathf.Abs(deltaAngle) < 2)
            {
                deltaAngle = 0;
                targetAngularSpeed = 0;
            }
            
            gizmosValuers.Add($"deltaAngle = {deltaAngle}");

            float anglePerFrame = targetAngularSpeed * Time.fixedDeltaTime; 
            gizmosValuers.Add($"anglePerFrame = {anglePerFrame}");

            float k = Mathf.Abs(anglePerFrame) / Mathf.Abs(deltaAngle);
            if (k >= 1f)
            {
                targetAngularSpeed = deltaAngle / Time.fixedDeltaTime;
                gizmosValuers.Add($"k = {k}");
            }
            
            gizmosValuers.Add($"targetAngularSpeed = {targetAngularSpeed}");

            Vector3 actualAngularSpeed = thisRigidbody.angularVelocity;
            gizmosValuers.Add($"actualAngularSpeed = {actualAngularSpeed.y * Mathf.Rad2Deg}");

            actualAngularSpeed.y = targetAngularSpeed * Mathf.Deg2Rad;
            thisRigidbody.angularVelocity = actualAngularSpeed;
            
            return deltaAngle;
        }



        List<String> gizmosValuers;

        void OnDrawGizmosSelected()
        {
            if (path != null && path.corners != null && path.corners.Length > 0 && actualPathCornerIndex >= 0)
            {
                Vector3 pathPoint = path.corners[actualPathCornerIndex];
                GizmosExtension.DrawLines(path.corners,1);
            }

        }

        void OnGUI()
        { 
            if(gizmosValuers==null)
                return;
            
            Vector3 screenPoint = thisCamera.WorldToScreenPoint(transform.position,Camera.MonoOrStereoscopicEye.Mono);

            Rect rect = new Rect(screenPoint.x, screenPoint.y, 500, 500);

            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            {
                for (int i = 0; i < gizmosValuers.Count; i++)
                    GUILayout.Label(gizmosValuers[i]);

            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
 

        public void SetPoint(Vector3 point)
        {
            //navMeshAgent.Warp(transform.position);
            //bool success = navMeshAgent.CalculatePath(point, path);
            bool success = NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path);

            actualPathCornerIndex = success ? 1 : -1;
        }
    }
}
