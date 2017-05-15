using UnityEngine;
using System.Collections.Generic;

namespace Mads
{
    [ExecuteInEditMode]
    public class LatticeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab = null;
        [SerializeField]
        private float spacing = 1f;
        [SerializeField]
        private int xSize = 5;
        [SerializeField]
        private int ySize = 5;
        [SerializeField]
        private int zSize = 5;

        private List<Transform> particles = new List<Transform>();
        private bool dirty;

        private void OnValidate()
        {
            xSize = Mathf.Max(0, xSize);
            ySize = Mathf.Max(0, ySize);
            zSize = Mathf.Max(0, zSize);
            dirty = true;
        }

        private void Update()
        {
            if (dirty)
            {
                foreach (var particle in particles)
                {
                    if (particle)
                    {
                        DestroyImmediate(particle.gameObject); 
                    }
                }
                particles = SpawnLattice();
                dirty = false;
            }
        }

        public List<Transform> SpawnLattice()
        {
            var particles = new List<Transform>(xSize * ySize * zSize);

            var triangleHeight = spacing * Mathf.Sqrt(3f) / 2f;
            var tetrahedronHeight = spacing * Mathf.Sqrt(6f) / 3f;

            var vertexA = Vector3.right;
            var vertexB = Quaternion.Euler(0f, 60f, 0f) * Vector3.right;
            var vertexC = Quaternion.Euler(0f, 30f, 0f) * (Quaternion.Euler(0f, 0f, 60f) * Vector3.right);

            var rowOffset = false;
            var colOffset = false;
            for (int layer = 0; layer < ySize; layer++)
            {
                for (int col = 0; col < xSize; col++)
                {
                    for (int row = 0; row < zSize; row++)
                    {
                        var v = col * vertexA + layer * vertexB + row * vertexC;
                        var particle = Instantiate(prefab, v, Quaternion.identity);
                        particles.Add(particle.transform);
                    }
                    rowOffset = !rowOffset;
                }
                colOffset = !colOffset;
            }
            return particles;
        }
    }
}
