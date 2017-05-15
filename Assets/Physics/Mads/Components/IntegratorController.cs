using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegratorController : MonoBehaviour
    {
        [SerializeField]
        private LatticeSpawner spawner = null;

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

        private void Start()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            Step();

            foreach (var particle in integrator.Particles)
            {
                particle.transform.position = particle.Position;
            }
        }

        public void Initialize()
        {
            var transforms = spawner.SpawnLattice();
            var particles = new List<SimpleTransformParticle>(transforms.Count);
            foreach (var t in transforms)
            {
                var particle = new SimpleTransformParticle()
                {
                    transform = t,
                    Position = t.position,
                    PreviousPosition = t.position,
                    mass = 1f
                };
                particles.Add(particle);
            }
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
