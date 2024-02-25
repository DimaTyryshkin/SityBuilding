using UnityEngine;

namespace Game
{
	public class BaryCentricDistanceTest:MonoBehaviour
	{
		[SerializeField] MeshFilter target;


		void OnDrawGizmos()
		{
			BaryCentricDistance b = new BaryCentricDistance(target);
			var result = b.GetClosestTriangleAndPoint(transform.position);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, result.closestPoint);

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, result.centre);

			Gizmos.color = Color.red;
			Gizmos.DrawLine(result.closestPoint, result.closestPoint + result.normal);
		}
	}
}