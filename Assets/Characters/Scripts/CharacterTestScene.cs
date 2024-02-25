using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class CharacterTestScene : MonoBehaviour
	{
		[SerializeField, IsntNull] CharactersBuilder charactersBuilder;
		[SerializeField, IsntNull] Camera gameCamera;

		void Start()
		{
			charactersBuilder.Init(gameCamera);
		}
	}
}