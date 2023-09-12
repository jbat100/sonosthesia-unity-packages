using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Builder
{
    public class World : MonoBehaviour
    {
        public Vector3 worldDimensions = new Vector3(3, 3, 3);
        public Vector3 chunkDimensions = new Vector3(10, 10, 10);
        public GameObject chunkPrefab;
        public Slider slider;

        protected void Start()
        {
            slider.gameObject.SetActive(false);
            slider.maxValue = worldDimensions.x * worldDimensions.y * worldDimensions.z;
            StartCoroutine(Build());
        }

        private IEnumerator Build()
        {
            slider.gameObject.SetActive(true);
            
            int count = 0;
            for (int z = 0; z < worldDimensions.z; z++)
            {
                for (int y = 0; y < worldDimensions.y; y++)
                {
                    for (int x = 0; x < worldDimensions.x; x++)
                    {
                        GameObject chunk = Instantiate(chunkPrefab);
                        Vector3 position = new Vector3(chunkDimensions.x * x, chunkDimensions.y * y, chunkDimensions.z * z);
                        chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, position);
                        count++;
                        slider.value = count;
                        yield return null;
                    }
                }
            }
            
            slider.gameObject.SetActive(false);
        }
    }
}