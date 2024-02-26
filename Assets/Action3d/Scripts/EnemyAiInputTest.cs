using GamePackages.Core.Validation;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	public class EnemyAiInputTest : MonoBehaviour
	{
		[SerializeField, IsntNull] EnemyAiInput enemyAiInput;
		[SerializeField, IsntNull] Transform goal;

		[Button()]
		void SetDestination()
		{
			enemyAiInput.SetDestination(goal.position);
		}
	}
}