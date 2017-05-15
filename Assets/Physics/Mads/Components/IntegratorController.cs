using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegratorController : MonoBehaviour
    {
        private IIntegrator<SimpleTransformParticle> integrator;

        public enum Simulate
        {
            Forward,
            Disabled,
            Backward
        }
        [SerializeField]
        private Simulate simulate;

        private void Awake()
        {
            integrator = new VerletIntegrator<SimpleTransformParticle>();
        }

        private void FixedUpdate()
        {
            Step();

            foreach (var particle in integrator.Particles)
            {
                particle.transform.position = particle.Position;
            }
        }

        public void Initialize(IEnumerable<SimpleTransformParticle> particles)
        {
            integrator.Initialize(particles);
        }

        private void Step()
        {
            switch (simulate)
            {
                case Simulate.Disabled:
                    return;
                case Simulate.Forward:
                    integrator.StepForward();
                    break;
                case Simulate.Backward:
                    integrator.StepBackward();
                    break;
            }
        }
    }
}
