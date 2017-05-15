using System.Collections.Generic;

namespace Mads
{
    public interface IIntegrator<TParticle> where TParticle : IParticle
    {
        IEnumerable<TParticle> Particles { get; set; }
        void Initialize(IEnumerable<TParticle> particles);
        void StepForward();
        void StepBackward();
    }
}
