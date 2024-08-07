using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cities
{
    public class HeatmapGenerator : MonoBehaviour
    {
        public int width = 256;
        public int height = 256;
        public Gradient gradient;
        private Texture2D texture;

        void Start()
        {
            texture = new Texture2D(width, height);
            GetComponent<Renderer>().material.mainTexture = texture;
            GenerateHeatmap();
        }

        void GenerateHeatmap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = Mathf.PerlinNoise((float)x / width, (float)y / height);
                    texture.SetPixel(x, y, gradient.Evaluate(value));
                }
            }
            texture.Apply();
        }
    }
}