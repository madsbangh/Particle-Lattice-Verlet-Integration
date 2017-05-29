using UnityEngine;

namespace Mads
{
    public class LinkedTransformParticle : IParticle
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

    public class IntegerLinkedTransformParticle
    {
        public Transform transform;
        private int[] position;
        public int[] Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                transform.position = new Vector3(value[0], value[1], value[2]) * 0.01f;
            }
        }
        public int[] PreviousPosition { get; set; }
        public int Mass { get; set; }
    }
}
