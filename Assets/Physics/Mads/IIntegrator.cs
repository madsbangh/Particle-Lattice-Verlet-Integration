using System.Collections.Generic;

namespace Mads
{
    public interface IIntegrator<TParticle> where TParticle : IParticle
    {
        List<TParticle> Particles { get; set; }
        void StepForward();
        void StepBackward();
    }
}
