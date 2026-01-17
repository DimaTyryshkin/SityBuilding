using NaughtyAttributes;
using UnityEngine;

namespace Game2.Building
{
    public class GameGrid : MonoBehaviour
    {
        public static Vector2Int[] nearOffsets = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        public static int[,] dirToAngle = new int[3, 3]
        {
            {225,270,315},
            {180,000,000},
            {135,090,045},
        };


        public static Vector3Int WorldPointToCell(Vector3 worldPoint) => WorldPointToCell(worldPoint.x, worldPoint.y, worldPoint.z);

        public static Vector3Int WorldPointToCell(float x, float y, float z)
        {
            int cellX = (int)Mathf.Floor(x);
            int cellY = (int)Mathf.Floor(y);
            int cellZ = (int)Mathf.Floor(z);
            return new Vector3Int(cellX, cellY, cellZ);
        }

        public static Vector3 CellToWorldPoint(Vector3 cell)
        {
            float x = cell.x;
            float y = cell.y;
            float z = cell.z;
            return new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
        }

        public int GetAngle(Vector2Int dir)
        {
            return dirToAngle[dir.x + 1, dir.y + 1];
        }

        [Button()]
        void Print()
        {
            Debug.Log(GetAngle(new Vector2Int(0, 1)));
            Debug.Log(GetAngle(new Vector2Int(1, 0)));
        }

        [SerializeField] GameObject goToAlign;
        [Button()]
        void AlignToGrid()
        {
            Vector3Int cell = WorldPointToCell(goToAlign.transform.position);
            goToAlign.transform.position = CellToWorldPoint(cell);
        }
    }
}