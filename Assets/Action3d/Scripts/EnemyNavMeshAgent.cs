using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
	public class EnemyNavMeshAgent : MonoBehaviour
	{
		[SerializeField, IsntNull] Transform goal;
		[SerializeField, IsntNull] NavMeshAgent navMeshAgent;
       
		void Start () { 
			navMeshAgent.destination = goal.position; 
		}
	}
}