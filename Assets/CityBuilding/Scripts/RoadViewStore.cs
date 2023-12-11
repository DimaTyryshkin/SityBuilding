using System.ComponentModel;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu]
	public class RoadViewStore:ScriptableObject
	{
		[System.Serializable]
		public struct NearValuesToView
		{
			public bool[] nearsValues;
			public GameObject prefab;
			public float spawnAngle;
		}

		public NearValuesToView[] map;

		public NearValuesToView GetPrefab(bool[] nearsValues)
		{
			foreach (var nearValuesToView in map)
			{
				if (IsEquals(nearValuesToView.nearsValues, nearsValues))
					return nearValuesToView;
			}

			throw new InvalidEnumArgumentException($"Нет префаба для {nearsValues[0]} {nearsValues[1]} {nearsValues[2]} {nearsValues[3]}");
		}

		public static bool IsEquals(bool[] nearsValues1, bool[] nearsValues2)
		{
			bool isEquals = true;
			for (int i = 0; i < 4; i++)
			{
				isEquals = isEquals && (nearsValues1[i] == nearsValues2[i]);
			}

			return isEquals;
		}
	}
}