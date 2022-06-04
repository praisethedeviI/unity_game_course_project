using UnityEngine;

namespace ProceduralGenerator
{
    public class NoiseMap : MonoBehaviour
    {
        public float scale = .1f;
        public static NoiseMap instance;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(this.gameObject);

        }

        public float[,] Generate(int sizeX, int sizeY)
        {
            float xOffset = Random.Range(-10000f, 10000f);
            float yOffset = Random.Range(-10000f, 10000f);
            float[,] noiseMap = new float[sizeX, sizeY];
            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    noiseMap[x, y] = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                } 
            }

            return noiseMap;
        }
    }
}