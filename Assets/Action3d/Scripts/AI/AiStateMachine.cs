using System;
using System.Collections.Generic;
using Game.Json;
using GamePackages.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game.EnemyAi
{
    public class AiStateMachine : MonoBehaviour
    {
        [SerializeField] EnemyAiInput enemyAiInput;
        List<AiState> states;
        
        void Start()
        {
            states = new List<AiState>(4); 
        }

       
        void Update()
        {
            foreach (var state in states)
            {
                state.UpdateStateAndSubStates();
            }
        }
        
        public void AddState(AiState state)
        { 
            states.Add(state);
            state.Init(enemyAiInput, this);
        }
        
        public void StopState(AiState state)
        { 
            state.StopAndSubStates(); 
            states.Remove(state); 
        }
    }

    public abstract class AiState  
    {
        protected EnemyAiInput enemyAiInput;
        protected AiStateMachine stateMachine;
        List<AiState> subStates;
        protected bool enable;

        public void Init(EnemyAiInput enemyAiInput, AiStateMachine stateMachine)
        {
            Assert.IsNotNull(enemyAiInput);
            Assert.IsNotNull(stateMachine);
            this.enemyAiInput = enemyAiInput;
            this.stateMachine = stateMachine;
            enable = true;
        }

        public void AddSubState(AiState state)
        {
            if (subStates == null)
                subStates = new List<AiState>(4);

            subStates.Add(state);
            state.Init(enemyAiInput, stateMachine);
        }

        public void StopAndSubStates()
        {
            if (enable)
            {
                SelfStop();
                enable = false;

                if (subStates != null)
                {
                    foreach (var state in subStates)
                        state.StopAndSubStates();
                }
            }
        }

        public void UpdateStateAndSubStates()
        {
            if(enable)
            {
                SelfUpdate();

                if (subStates != null)
                {
                    foreach (var state in subStates)
                        state.UpdateStateAndSubStates();
                }
            }            
        }
        
        protected virtual void SelfUpdate()
        {
             
        }  
        
        protected virtual void SelfStop()
        {
             
        }  
    }

    public class PatrolState : AiState
    {
        Transform[] patrolPoints;
        int counter;

        public float TimeStart { get; private set; }

        public void Run(Transform[] patrolPoints)
        {
            AssertWrapper.IsAllNotNull(patrolPoints);
            this.patrolPoints = patrolPoints;

            TimeStart = Time.time;
            NextPoint();
        }

        void NextPoint()
        {
            int index = MathExtension.PingPong(counter, patrolPoints.Length - 1);
            Transform p = patrolPoints[index];

            MoveToTransformState move = new MoveToTransformState();
            AddSubState(move);
            move.Run(p, NextPoint);

            counter++;
        } 
    }

    public class MoveToTransformState : AiState
    {
        Transform target;
        Vector3 pos;
        UnityAction success;
        
        public void Run(Transform target, UnityAction success)
        {
            Assert.IsNotNull(target);
            this.target = target;
            this.success = success;

            SetDestination();
        }

        void OnArrivedDestination()
        {
            if (enable)
            {
                //Debug.Log("[d] OnArrivedDestination"); 
                StopAndSubStates();
                ActionWrapper.ClearAndInvoke(ref success);
            }
        }

        void SetDestination()
        {
            //Debug.Log("[d] SetDestination");
            pos = target.position; 
            enemyAiInput.SetDestination(pos, OnArrivedDestination);
        }

        protected override void SelfUpdate()
        {
            if (Vector3.Distance(target.position, pos) > 0.1f)
                SetDestination();
        }
    } 
}
