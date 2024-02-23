using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Legs : MonoBehaviour
    {
        [Serializable]
        public class Lag
        {
            public Transform effector;
            public Transform target;
            public float originDx;
        }

        [SerializeField] float bodyMoveSpeed = 1;
        [SerializeField] float offset = 0;
        [SerializeField] float stepTime;
        [SerializeField] float stepDistance;
        [SerializeField] float bodySpeedToDistanceFactor = 1;
        [SerializeField] float criticalDistance;
        [SerializeField, Range(0,1)] float kFactor;
        [SerializeField] float lagsSpeed;
        [SerializeField] Transform bodyCenter;
        [SerializeField] Lag[] lags;
        
        float timeNextStep;
        float distance;
        float oldC;
        float[] dx;
        Vector3[] realPos;
        Vector3[] targetPos;
        int counter;
        
        

        void Start()
        {
            dx = new float[lags.Length];
            realPos = lags.Select(x => x.target.position).ToArray();
            targetPos = realPos.ToArray();

            float c = bodyCenter.position.x;
            for (int i = 0; i < lags.Length; i++)
            {
                lags[i].originDx = c - lags[i].target.position.x;
            }
        }

        void LateUpdate()
        {
            bodyCenter.position += new Vector3(Time.deltaTime * bodyMoveSpeed, 0, 0);
            
            float c = bodyCenter.position.x;
            distance += oldC - c;
          
            
            for (int i = 0; i < lags.Length; i++)
            {
                lags[i].target.position = realPos[i];
                realPos[i] = Vector3.MoveTowards(realPos[i], targetPos[i], Time.deltaTime * lagsSpeed);
            }

            // for (int i = 0; i < lags.Length; i++)
            // {
            //     float d = Mathf.Abs(targetPos[i].x - bodyCenter.position.x);
            //     if (d > criticalDistance)
            //         targetPos[i].x = bodyCenter.position.x + criticalDistance;
            // }

            if (Mathf.Abs(distance)>(stepDistance * bodyMoveSpeed * bodySpeedToDistanceFactor) + 0.05f || Time.time > timeNextStep)
            {
                distance = 0;
                timeNextStep = Time.time + stepTime;
                targetPos[counter].x = bodyCenter.position.x + (stepDistance * bodyMoveSpeed * bodySpeedToDistanceFactor) + offset;
                counter = (counter + 1) % lags.Length;
            }

            oldC = c;
        }
    }
}
