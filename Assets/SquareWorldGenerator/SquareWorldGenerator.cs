using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SquareWorldGenerator
{
    [Serializable]
    public struct StartData
    {
        public int x;
        public int y;
        public int type;
    }

    public class SquareWorldGenerator : MonoBehaviour
    {
        [SerializeField] Grid grid;
        [SerializeField] TypeCollection typeCollection;
        [SerializeField] int freq; 
        [SerializeField] int r; 
        [SerializeField] bool autoTick;
        [SerializeField] bool randomRules;
        
        [Space]
        [SerializeField] bool random;
        [SerializeField] bool basicRandom;
        [SerializeField] bool random2;
        [SerializeField] int random2Count = 10;
        
        [SerializeField] StartData[] startData;

        float timeNextUpdate;



        Vector2Int[] clockNears = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };
            
        List<Vector2Int> near;
        
        
        void Start()
        { 
            timeNextUpdate = 0.1f;
            
            
            DrawStart();
        }

        void Update()
        {
            if (autoTick)
            {
                if (Time.time < timeNextUpdate)
                    return;

                timeNextUpdate = Time.time + 1f / freq;
                Tick();
            }
        }

        public void StopAutoTick()
        {
            autoTick = false;
        }

        [Button()]
        void DrawStart()
        {

            autoTick = true;
            near = new List<Vector2Int>();

           
            for (int x = -r; x <= r; x++)
            for (int y = -r; y <= r; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                var a = new Vector2Int(x, y);
                if (a.magnitude <= r)
                    near.Add(a);

            }

            if (randomRules)
                GenerateRandomRiles();
 
            grid.Init();
 
            if (basicRandom)
            {
                FillBasicRandom(); 
            }
            else if (random2)
            {
                FillRandom2();
            }
            else if (random)
            {
                FillRandom();
            }
            else
            {
                FillFromData();
            }
        }



        void FillFromData()
        { 
            foreach (var data in startData)
            {
                grid.Get(data.x, data.y).Type = data.type;
            }
        }

        void FillRandom()
        {  
            for (int i = 0; i < grid.Count; i++)
            { 
                grid.Get(i).Type = Random.Range(0, typeCollection.ColorsCount);
            }
        }

        void FillBasicRandom()
        { 
            for (int i = 0; i < typeCollection.ColorsCount; i++)
            {
                int x = Random.Range(0, grid.width);
                int y = Random.Range(0, grid.height);
                grid.Get(x, y).Type = i;
            }
        }

        void FillRandom2()
        { 
            for (int i = 0; i < random2Count; i++)
            {
                int x = Random.Range(0, grid.width);
                int y = Random.Range(0, grid.height);
                grid.Get(x, y).Type = Random.Range(0, typeCollection.ColorsCount);
            }
        }

        [Button()]
        public void Tick()
        {
            for (int i = 0; i < grid.Count; i++)
                grid.Get(i).waitForUpdate = true;

            for (int i = 0; i < grid.Count; i++)
            {
                Vector2Int cell = grid.GetCell(i);
                Square square = grid.Get(i);

                if (square.waitForUpdate && square.Type != 0)
                {
                    SquareTypeRules rule = typeCollection.rules[square.Type];
                    
                    //Ходим вов се стороны
                    for (int n = 0; n < clockNears.Length; n++)
                    {
                        Vector2Int nearCell = cell + clockNears[n];
                        ProcessCell(nearCell, square, rule);
                    }
                    
                    // Ходим в одну сторону
                    //Vector2Int dir = clockNears[PickDir(rule.moveChance)];
                    //Vector2Int nearCell = cell + dir;
                    //ProcessCell(nearCell, square, rule);
                }
            }
        }

        void ProcessCell(Vector2Int nearCell, Square square, SquareTypeRules rule)
        {
            var nearSquare = grid.GetLoop(nearCell);
            if (nearSquare.waitForUpdate)
            {  
                for (int j = 0; j < rule.hunting.Length; j++)
                {
                    if (nearSquare.Type == rule.hunting[j])
                    {
                        square.Type = rule.type1; 
                        nearSquare.Type = rule.type2;
                        nearSquare.waitForUpdate = false;
                    }
                } 
            }
        }

        int PickDir(float[] moveChance)
        {
            float rnd = Random.Range(0f, 1f);

            float value = 0;
            for (int i = 0; i < moveChance.Length; i++)
            {
                value += moveChance[i];
                if (value >= rnd)
                    return i;
            }

            return moveChance.Length - 1;
        }

        [Button()]
        public void Clear()
        {
            grid.Clear();
        } 
        
        [Button()]
        public void RemoveObjects()
        {
            grid.RemoveObjects(); 
        }

        [Button()]
        public void GenerateBasicRiles()
        { 
            for (int i = 1; i < typeCollection.rules.Length; i++)
            {
                int hunt = i + 1;
                if (hunt >= typeCollection.rules.Length)
                    hunt = 1;
                
                typeCollection.rules[i].hunting = new int[]
                {
                    0,
                    hunt
                };

                typeCollection.rules[i].nearRule = 0;
                typeCollection.rules[i].type1 = i;
                typeCollection.rules[i].type2 = i;
                GenerateRandomMoveChance(i);
            }  
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(typeCollection);
            #endif
        }
        
        [Button()]
        public void GenerateRandomRiles()
        { 
            for (int i = 1; i < typeCollection.rules.Length; i++)
            {
                //int hunt1 = Random.Range(1, typeCollection.rules.Length);
                int hunt2 = Random.Range(1, typeCollection.rules.Length);
                int hunt3 = Random.Range(1, typeCollection.rules.Length);
                
                int hunt = i + 1;
                if (hunt >= typeCollection.rules.Length)
                    hunt = 1;

                typeCollection.rules[i].hunting = new int[]
                {
                    0,
                    hunt2,
                    hunt3
                };

                GenerateRandomMoveChance(i);
              
                
                typeCollection.rules[i].nearRule = 1;// Random.Range(0, near.Count);

                typeCollection.rules[i].type1 = Random.Range(1, typeCollection.rules.Length);
                typeCollection.rules[i].type2 = Random.Range(1, typeCollection.rules.Length);
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(typeCollection);
#endif
        }

        void GenerateRandomMoveChance(int i)
        {
            typeCollection.rules[i].moveChance = new float[8];
            float sum = 0;
            for (int j = 0; j < 8; j++)
            {
                float chance = Random.Range(0, 100);
                sum += chance;
                typeCollection.rules[i].moveChance[j] = chance;
            }
                
            for (int j = 0; j < 8; j++)
            { 
                typeCollection.rules[i].moveChance[j] /= sum;
            }
        }
    }
}
