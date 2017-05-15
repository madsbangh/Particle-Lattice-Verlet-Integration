using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class VerletIntegrator<TParticle> : IIntegrator<TParticle> where TParticle : IParticle
    {
        public float sigmaSquared = 1f;
        public float mu = 1f;
        public float decay = 0.25f;
        public float repulsion = 10f;
        public float attraction = 1f;

        public IList<TParticle> Particles { get; set; }

        public void Initialize(IList<TParticle> particles)
        {
            Particles = particles;
        }

        public void StepForward()
        {
            var acceleration = new Vector3[Particles.Count];

            for (int i = 0; i < Particles.Count - 1; i++)
            {
                var thisParticle = Particles[i];
                for (int j = i + 1; j < Particles.Count; j++)
                {
                    var thatParticle = Particles[j];
                    var selfToOther = thatParticle.Position - thisParticle.Position;

                    // HACK: Mass is not implemented correctly yet
                    acceleration[i] += selfToOther.normalized
                                        * Formulas.PushPullForceExp(selfToOther.magnitude,
                                        thisParticle.Mass * thatParticle.Mass,
                                        attraction, repulsion, decay, mu, sigmaSquared);
                    acceleration[j] -= acceleration[i];
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                var nextPosition = 2f * particle.Position - particle.PreviousPosition + acceleration[i] * Time.fixedDeltaTime * Time.fixedDeltaTime;
                Particles[i].PreviousPosition = particle.Position;
                Particles[i].Position = nextPosition;
            }
        }

        public void StepBackward()
        {

        }
    }
}