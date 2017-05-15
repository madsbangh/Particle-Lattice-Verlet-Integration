using System.Collections.Generic;

namespace Mads
{
    public class VerletIntegrator<TParticle> : IIntegrator<TParticle> where TParticle : IParticle
    {
        public IEnumerable<TParticle> Particles { get; set; }

        public void Initialize(IEnumerable<TParticle> particles)
        {
            Particles = particles;
        }

        public void StepForward()
        {

        }

        public void StepBackward()
        {

        }
    }
}