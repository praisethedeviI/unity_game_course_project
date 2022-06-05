using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralGenerator
{
    public class Generator : MonoBehaviour
    {
        public PlacementManager placementManager;
        private int width, height;
        public GameObject grass, water;
        
        [Range(0f, 1f)]
        public float waterLevel = 0.5f;
        
        [Range(0f, 1f)]
        public float natureLevel = 0.7f;

        public List<GameObject> NatureTreeList;
        public List<GameObject> NatureRockList;

        private float[,] noiseMap;

        private void Awake()
        {
            width = placementManager.width;
            height = placementManager.height;

            noiseMap = NoiseMap.instance.Generate(width, height);
        }

        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var isWater = noiseMap[x, y] < waterLevel;
                    if (isWater)
                    {
                        placementManager.PlaceGround(new Vector3Int(x, 0, y), water, CellType.Water);
                    }
                    else
                    {
                        placementManager.PlaceGround(new Vector3Int(x, 0, y), grass, CellType.Empty);
                        if (noiseMap[x, y] > natureLevel + waterLevel)
                        {
                            var randPosX = Random.Range(-0.4f, 0.4f);
                            var randPosY = Random.Range(-0.4f, 0.4f);
                            var randNature = Random.Range(0, 5);
                            if (randNature <= 3)
                            {
                                var randNatureTree = Random.Range(0, NatureTreeList.Count - 1);
                                placementManager.PlaceNature(new Vector3(x + randPosX, 0, y + randPosY),
                                    NatureTreeList[randNatureTree], "Tree");
                            }
                            else
                            {
                                var randNatureRock = Random.Range(0, NatureRockList.Count - 1);
                                placementManager.PlaceNature(new Vector3(x + randPosX, 0, y + randPosY),
                                    NatureRockList[randNatureRock], "Rock");
                            }
                            
                        }
                    }
                }
            }
        }
    }
}