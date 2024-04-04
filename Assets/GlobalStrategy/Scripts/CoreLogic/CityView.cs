using GamePackages.Core.Validation;
using UnityEngine;

namespace GlobalStrategy.CoreLogic
{
	public class CityView : MonoBehaviour
	{
		[SerializeField, IsntNull] City city;
		[SerializeField, IsntNull] Transform fuelView;
		[SerializeField, IsntNull] Transform shellsView;
		[SerializeField, IsntNull] Transform foodView;
		[SerializeField, IsntNull] Transform materialsView;
		

		void Start()
		{
			Draw();
			city.Updated += Draw;
		}

		void Draw()
		{
			Draw(fuelView, city.balance.fuel);
			Draw(shellsView, city.balance.shells);
			Draw(foodView, city.balance.food);
			Draw(materialsView, city.balance.materials);
		}

		void Draw(Transform progressBat, float value)
		{
			float k = value / 100f;
			progressBat.localScale = new Vector3(1, k, 1);
		}
	}
}