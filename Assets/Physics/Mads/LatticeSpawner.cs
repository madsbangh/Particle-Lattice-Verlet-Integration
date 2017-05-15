using UnityEngine;
using System.Collections.Generic;

namespace Mads
{
    public class LatticeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private float spacing;
        [SerializeField]
        private int xSize;
        [SerializeField]
        private int ySize;
        [SerializeField]
        private int zSize;
        
        public List<Transform> SpawnLattice()
        {
            var particles = new List<Transform>(xSize * ySize * zSize);

            var tan60halfSpacing = spacing * 0.5f * Mathf.Tan(Mathf.Deg2Rad * 60f);
            var rowOffset = false;
            var colOffset = false;
            for (int layer = 0; layer < xSize; layer++)
            {
                for (int col = 0; col < ySize; col++)
                {
                    for (int row = 0; row < zSize; row++)
                    {
                        var x = col * spacing + (colOffset ? spacing * 0.5f : 0f);
                        var y = layer * tan60halfSpacing;
                        var z = row * tan60halfSpacing - (rowOffset ? spacing * 0.5f : 0f);
                        var particle = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
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
