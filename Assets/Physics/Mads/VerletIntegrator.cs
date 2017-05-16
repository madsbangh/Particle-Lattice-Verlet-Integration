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

        public List<TParticle> Particles { get; set; }

        public void StepForward()
        {
            var acceleration = new Vector3[Particles.Count];

            for (int i = 0; i < Particles.Count - 1; i++)
            {
                for (int j = i + 1; j < Particles.Count; j++)
                {
                    var selfToOther = Particles[j].Position - Particles[i].Position;

                    // HACK: Mass is not implemented correctly yet
                    var a = selfToOther.normalized
                        * Formulas.PushPullForceExpGrad(selfToOther.magnitude,
                        Particles[i].Mass * Particles[j].Mass,
                        attraction, repulsion, decay, sweetSpot, width);
                    acceleration[i] -= a;
                    acceleration[j] += a;
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = 2f * Particles[i].Position - Particles[i].PreviousPosition + acceleration[i] * Time.fixedDeltaTime * Time.fixedDeltaTime;
                Particles[i].PreviousPosition = Particles[i].Position;
                Particles[i].Position = nextPosition;
            }
        }

        public void StepBackward()
        {
            var acceleration = new Vector3[Particles.Count];

            for (int i = 0; i < Particles.Count - 1; i++)
            {
                for (int j = i + 1; j < Particles.Count; j++)
                {
                    var selfToOther = Particles[j].Position - Particles[i].Position;

                    // HACK: Mass is not implemented correctly yet
                    var a = selfToOther.normalized
                        * Formulas.PushPullForceExpGrad(selfToOther.magnitude,
                        Particles[i].Mass * Particles[j].Mass,
                        attraction, repulsion, decay, sweetSpot, width);
                    acceleration[i] += a;
                    acceleration[j] -= a;
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = 2f * Particles[i].Position - Particles[i].PreviousPosition + acceleration[i] * Time.fixedDeltaTime * Time.fixedDeltaTime;
                Particles[i].PreviousPosition = Particles[i].Position;
                Particles[i].Position = nextPosition;
            }
        }
    }
}