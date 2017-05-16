using UnityEngine;

namespace Mads
{
    public class BoundTransformParticle : IParticle
    {
        public Transform transform;
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }
        public Vector3 PreviousPosition { get; set; }
        public float Mass { get; set; }
    }
}
