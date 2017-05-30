using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atoms
{
    public class VerletIntegrator
    {
        // Parameters
        public float width;
        public float gaussianCenter;
        public float decay;
        public float expContribution;
        public float gaussianContribution;
        public float timestep;

        // Particle states
        public List<Transform> Transforms { get; set; }
        public List<Vector3> Positions { get; set; }
        public List<Vector3> PreviousPositions { get; set; }
        public List<float> Influences { get; set; }

        private List<Vector3> accelerations = new List<Vector3>();

        public void StepForward()
        {
            AccumulateAcceleration(Positions);

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
            AccumulateAcceleration(PreviousPositions);

            for (int i = 0; i < Positions.Count; i++)
            {
                var nextPosition = 2f * PreviousPositions[i] - Positions[i] + accelerations[i] * timestep * timestep;
                Positions[i] = PreviousPositions[i];
                PreviousPositions[i] = nextPosition;
                Transforms[i].position = nextPosition;
            }
        }

        private void AccumulateAcceleration(List<Vector3> positions)
        {
            // Allocate if needed
            while (accelerations.Count < positions.Count)
            {
                accelerations.Add(Vector3.zero);
            }

            // Reset acceleration
            for (int i = 0; i < accelerations.Count; i++)
            {
                accelerations[i] = Vector3.zero;
            }

            // Accumulate acceleration
            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = 0; j < positions.Count; j++)
                {
                    var selfToOther = positions[j] - positions[i];

                    accelerations[i] -= selfToOther.normalized
                        * Formulas.PushPullExpDerivative(selfToOther.magnitude,
                        Influences[i] * Influences[j],
                        gaussianContribution, expContribution, decay, gaussianCenter, width);
                }
            }
        }
    }
}