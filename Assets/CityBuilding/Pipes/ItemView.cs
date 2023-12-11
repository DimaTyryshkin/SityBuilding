using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class ItemView:MonoBehaviour
	{
		[SerializeField, IsntNull] MeshRenderer meshRenderer;

		public void SetColor(Color color)
		{
			meshRenderer.material.color = color;
		}
	}
}