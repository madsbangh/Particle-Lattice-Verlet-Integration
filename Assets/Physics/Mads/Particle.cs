using UnityEngine;

namespace Mads
{
    public struct SimpleTransformParticle : IParticle
    {
        public Transform transform;
        public Vector3 Position { get; set; }
        public Vector3 PreviousPosition { get; set; }
        public float Mass { get; set; }
    }
}
