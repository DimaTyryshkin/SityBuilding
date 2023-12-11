using System.Collections.Generic;
using System.Linq;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	class RoadViewMapBuilder : MonoBehaviour
	{
		#if UNITY_EDITOR
		[SerializeField] RoadViewStore roadViewMap;
		[SerializeField] GameGrid grid;
	 
		Dictionary<Vector2, GameObject> cellToObject;
		Dictionary<Vector2, bool> cellToValue;
		List<bool[]> processedNearCases;


		[Button()]
		void BuildMap()
		{
			GameObject[] roadSegments = transform.GetComponentsInFirstChild<Transform>()
				.Select(x => x.gameObject)
				.ToArray();
			
			cellToObject = new Dictionary<Vector2, GameObject>();
			cellToValue = new Dictionary<Vector2, bool>();
			processedNearCases = new List<bool[]>();
			
			foreach (GameObject segment in roadSegments)
			{
				Vector2 cell = grid.WorldPointToCell(segment.transform.position);
				Debug.Log($"go={segment.name} cell ={cell.x} {cell.y}");
				cellToObject.Add(cell, segment);
				cellToValue.Add(cell, true);
			}
			
			
			
			// map
			
			var map = new List<RoadViewStore.NearValuesToView>();

			bool[] nearValues = new[] { false, false, false, false };
			
			foreach (GameObject segment in roadSegments)
			{
				string pathToPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(segment);
				GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToPrefab);
				
				Vector2 cell = grid.WorldPointToCell(segment.transform.position);
				for (int i = 0; i < 4; i++)
					nearValues[i] = cellToValue.GetOrDefault(cell + GameGrid.nearOffsets[i], false);
				
				
				for (int rotation = 0; rotation < 4; rotation++)
				{
					if (!IsProcessed(nearValues))
					{
						map.Add(new RoadViewStore.NearValuesToView()
						{
							nearsValues = nearValues.ToArray(),
							prefab = prefab,
							spawnAngle = 90 * rotation,

						});
						
						processedNearCases.Add(nearValues.ToArray());
					}

					nearValues.ShiftRight(1);
				}
			}


			Undo.RecordObject(roadViewMap, "Load");
			EditorUtility.SetDirty(roadViewMap);
			roadViewMap.map = map.ToArray();
		}

		bool IsProcessed(bool[] nearValues)
		{
			foreach (var nearValuesCase in processedNearCases)
			{ 
				if (RoadViewStore.IsEquals(nearValues, nearValuesCase))
					return true;
			}

			return false;
		}
#endif
	}
}