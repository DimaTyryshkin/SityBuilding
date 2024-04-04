using System;
using System.Linq;
using GamePackages.Core;
using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace Game.EnemyAi
{
	public class PatrolScript : MonoBehaviour
	{
		[SerializeField, IsntNull] AiStateMachine aiStateMachine;
		[SerializeField, IsntNull] Transform[] points;


		[Button()]
		void StartPatrol()
		{
			PatrolState patrol = new PatrolState();
			aiStateMachine.AddState(patrol);
			patrol.Run(points);
		}

		void OnDrawGizmosSelected()
		{
			GizmosExtension
				.DrawLines(points.Select( x=>
					{
						NavMesh.SamplePosition(x.position, out var hit,10, NavMesh.AllAreas);
						return hit.position;
					} )
				.ToArray(), 0.1f);
		}
	}
}