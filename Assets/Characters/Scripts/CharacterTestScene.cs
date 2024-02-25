using GamePackages.Core.Validation;
using UnityEngine;

namespace Game
{
	public class CharacterTestScene : MonoBehaviour
	{
		[SerializeField, IsntNull] CharactersBuilder charactersBuilder;
		[SerializeField, IsntNull] Camera gameCamera;
		[SerializeField, IsntNull] Character player;

		void Start()
		{
			charactersBuilder.Init(gameCamera);
			charactersBuilder.InitPlayer(player.gameObject);
		}
	}
}