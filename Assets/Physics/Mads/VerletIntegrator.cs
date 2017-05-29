using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class VerletIntegrator<TParticle> : IIntegrator<TParticle> where TParticle : IParticle
    {
        public float width = 9.63f;
        public float sweetSpot = 4.06f;
        public float decay = 0.51f;
        public float repulsion = 5.24f;
        public float attraction = 4.83f;
        public float energyConservation;
        public float timestep = 0.01f;

        public List<TParticle> Particles { get; set; }

        public void StepForward()
        {
            var acceleration = new Vector3[Particles.Count];

            for (int i = 0; i < Particles.Count; i++)
            {
                for (int j = 0; j < Particles.Count; j++)
                {
                    var selfToOther = Particles[j].Position - Particles[i].Position;

                    // HACK: Mass is not implemented correctly yet
                    acceleration[i] -= selfToOther.normalized
                        * Formulas.PushPullForceExpGrad(selfToOther.magnitude,
                        Particles[i].Mass * Particles[j].Mass,
                        attraction, repulsion, decay, sweetSpot, width);
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = (1f + energyConservation) * Particles[i].Position - Particles[i].PreviousPosition * energyConservation + acceleration[i] * timestep * timestep;
                Particles[i].PreviousPosition = Particles[i].Position;
                Particles[i].Position = nextPosition;
            }
        }

        public void StepBackward()
        {
            var acceleration = new Vector3[Particles.Count];

            for (int i = 0; i < Particles.Count; i++)
            {
                for (int j = 0; j < Particles.Count; j++)
                {
                    var selfToOther = Particles[j].PreviousPosition - Particles[i].PreviousPosition;

                    // HACK: Mass is not implemented correctly yet
                    acceleration[i] -= selfToOther.normalized
                        * Formulas.PushPullForceExpGrad(selfToOther.magnitude,
                        Particles[i].Mass * Particles[j].Mass,
                        attraction, repulsion, decay, sweetSpot, width);
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = (1f + energyConservation) * Particles[i].PreviousPosition - Particles[i].Position * energyConservation + acceleration[i] * timestep * timestep;
                Particles[i].Position = Particles[i].PreviousPosition;
                Particles[i].PreviousPosition = nextPosition;
            }
        }
    }
}