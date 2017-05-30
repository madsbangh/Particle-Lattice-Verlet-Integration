using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class VerletIntegrator
    {
        // Parameters
        public float width;
        public float sweetSpot;
        public float decay;
        public float repulsion;
        public float attraction;
        public float timestep;

        // Particle states
        public List<Transform> Transforms { get; set; }
        public List<Vector3> Positions { get; set; }
        public List<Vector3> PreviousPositions { get; set; }
        public List<float> Masses { get; set; }

        private List<Vector3> accelerations = new List<Vector3>();

        public void StepForward()
        {
            // Allocate if needed
            while (accelerations.Count < Positions.Count)
            {
                accelerations.Add(Vector3.zero);
            }

            // Reset acceleration
            for (int i = 0; i < accelerations.Count; i++)
            {
                accelerations[i] = Vector3.zero;
            }

            // Accumulate acceleration
            for (int i = 0; i < Positions.Count; i++)
            {
                for (int j = 0; j < Positions.Count; j++)
                {
                    var selfToOther = Positions[j] - Positions[i];

                    accelerations[i] -= selfToOther.normalized
                        * Formulas.PushPullExpDerivative(selfToOther.magnitude,
                        Masses[i] * Masses[j],
                        attraction, repulsion, decay, sweetSpot, width);
                }
            }
            for (int i = 0; i < Positions.Count; i++)
            {
                var nextPosition = 2f * Positions[i] - PreviousPositions[i] + accelerations[i] * timestep * timestep;
                PreviousPositions[i] = Positions[i];
                Positions[i] = nextPosition;
                Transforms[i].position = nextPosition;
            }
        }

        public void StepBackward()
        {

        }
    }
}