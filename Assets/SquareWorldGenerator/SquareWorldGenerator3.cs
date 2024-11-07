using System;
using System.Collections.Generic;
using GamePackages.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using static UnityEngine.Mathf;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SquareWorldGenerator
{
    public class SquareWorldGenerator3 : MonoBehaviour
    {
        [SerializeField] Grid grid; 
        [SerializeField] TypeCollection typeCollection; 
        [SerializeField] int freq; 
        [SerializeField] int r;  
        [SerializeField] bool autoTick;
        [SerializeField] bool autoTickOnStart;
        [SerializeField] bool randomRules;
        
        
        [Space]
        [SerializeField] bool fullRandom;
        [SerializeField] bool randomPoints;
        [SerializeField] int randomPointsCount = 10;
         

        float timeNextUpdate;
  
        float[,] matrix;
        int[,] field1;
        int[,] field2;
        int[] counter;
        float matrixMaxValue;
        
        
        void Start()
        { 
            timeNextUpdate = 0.0f;
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
            counter = new int[typeCollection.ColorsCount];
            field1 = new int[grid.width, grid.height];
            field2 = new int[grid.width, grid.height];
            
            if (autoTickOnStart)
                autoTick = true;

            if (randomRules)
            {
                GenerateRandomMatrix(r); 
            }
 
            grid.Init();
 
            if (fullRandom)
            {
                FullRandom(); 
            }
            else if (randomPoints)
            {
                RandomPoints();
            }
           
        }


        void FullRandom()
        {
            for (int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
            {
                field1[x, y] = Random.Range(0, typeCollection.ColorsCount);
                grid.Get(x, y).Type = field1[x, y];
            }
        }


        void RandomPoints()
        { 
            for (int i = 0; i < randomPointsCount; i++)
            {
                int x = Random.Range(0, grid.width);
                int y = Random.Range(0, grid.height);
                field1[x, y] = Random.Range(0, typeCollection.ColorsCount);
                grid.Get(x, y).Type = field1[x, y];
            }
        }

        [Button()]
        void Tick()
        {
            for (int i = 0; i < grid.Count; i++)
            {
                Vector2Int cell = grid.GetCell(i);

                //SquareTypeRules rule = typeCollection.rules[square.Type];

                int newType = ApplyMatrix(cell);
                //float functionValue = ActivationFunction(matrixValue);
                if (newType >= typeCollection.ColorsCount)
                    throw new Exception($"newType={newType} cell={cell}");

                if (newType > 0)
                { 
                    field2[cell.x, cell.y] = newType;
                }
                else
                    field2[cell.x, cell.y] = field1[cell.x, cell.y];
            }

            for (int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
            {
                field1[x, y] = field2[x, y];
                grid.Get(x, y).Type = field1[x, y];
            }
        }

        float ActivationFunction(float value)
        {
            
            return Sin(value * PI) * 2f - 1f;
            //return Sin(value *  PI * 0.5f);

            if (value == 0)
                return 0;

            return 1;
        }


        int ApplyMatrix(Vector2Int cell)
        {
            for (int i = 0; i < counter.Length; i++)
                counter[i] = 0;

            for (int x = 0; x < r * 2 + 1; x++)
            for (int y = 0; y < r * 2 + 1; y++)
            {
                var a = new Vector2Int(x - r, y - r);
                float dist = a.magnitude;
                if (dist <= r)
                {
                    var c = new Vector2Int(cell.x - r + x, cell.y - r + y);
                    c = grid.LoopCell(c);

                    int type = field1[c.x, c.y];
                    counter[type]++;
                }
            }

            int maxCount = 0;
            int maxIndex = 0;
            for (int i = 1; i < counter.Length; i++)
            {
                if (counter[i] > 0)
                {
                    if (counter[i] > maxCount)
                    {
                        maxCount = counter[i];
                        maxIndex = i;
                    }
                }
            }

            return maxIndex;
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
        
        [Button()]
        public void Clear()
        {
            grid.Clear();
        }

        [Button()]
        void ApplyMatrixToSelection()
        {
            int value = ApplyMatrix(grid.SelectedCell);
            Debug.Log(value);
        }

        [Button()]
        public void RemoveObjects()
        {
            grid.RemoveObjects(); 
        }

       
        
        [Button()]
        public void GenerateRandomRiles()
        {
            GenerateRandomMatrix(r); 
        }

        [Button()]
        void SetMatrix()
        {
            r = 1;
            matrix = new float[,]
            {
                { 0, 0, 0 },
                { 0, 0, 1 },
                { 0, 0, 0 },
            };

        }

        void GenerateRandomMatrix(int r)
        {
            matrix = new float[r * 2 + 1, r * 2 + 1];

            matrixMaxValue = 0;

            for (int x = 0; x < r * 2 + 1; x++)
            for (int y = 0; y < r * 2 + 1; y++)
            {
                var a = new Vector2Int(x - r, y - r);
                float dist = a.magnitude;
                if (dist <= r)
                {
                    //if (!(x == r && y == r))
                    //{
                    //matrix[x, y] = Random.Range(0f,1f);
                    //matrix[x, y] = 1 - (dist) / (r + 1);
                    matrix[x, y] = Sin(dist * PI / (r + 1));
                    
                    matrixMaxValue++;
                    //}
                }
                //matrix[x, y] = 1;
            }
        }
    }
}
