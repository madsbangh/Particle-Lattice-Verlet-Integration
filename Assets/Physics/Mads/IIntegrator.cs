using System.Collections.Generic;

namespace Mads
{
    public interface IIntegrator<TParticle> where TParticle : IParticle
    {
        IList<TParticle> Particles { get; set; }
        void Initialize(IList<TParticle> particles);
        void StepForward();
        void StepBackward();
    }
}
