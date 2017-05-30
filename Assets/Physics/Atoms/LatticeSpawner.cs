using UnityEngine;
using System.Collections.Generic;

namespace Atoms
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

        private Vector3 vertexA;
        private Vector3 vertexB;
        private Vector3 vertexC;

        private void OnValidate()
        {
            xSize = Mathf.Max(0, xSize);
            ySize = Mathf.Max(0, ySize);
            zSize = Mathf.Max(0, zSize);

            vertexA = Vector3.right * spacing;
            vertexB = Quaternion.Euler(0f, 60f, 0f) * Vector3.right * spacing;
            vertexC = Quaternion.Euler(0f, 30f, 0f) * (Quaternion.Euler(0f, 0f, 60f) * Vector3.right) * spacing;
        }

        private void OnDrawGizmosSelected()
        {
            var rowOffset = false;
            var colOffset = false;
            for (int layer = 0; layer < ySize; layer++)
            {
                for (int col = 0; col < xSize; col++)
                {
                    for (int row = 0; row < zSize; row++)
                    {
                        var v = col * vertexA + layer * vertexB + row * vertexC;

                        Gizmos.DrawWireSphere(v, 0.5f);
                    }
                    rowOffset = !rowOffset;
                }
                colOffset = !colOffset;
            }
        }

        [EasyButtons.Button(EasyButtons.ButtonMode.EnabledInPlayMode)]
        public List<Transform> SpawnLattice()
        {
            var particles = new List<Transform>(xSize * ySize * zSize);

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
