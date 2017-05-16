using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegratorController : MonoBehaviour
    {
        [SerializeField]
        private LatticeSpawner spawner = null;

        private IIntegrator<BoundTransformParticle> integrator;

        public enum Simulate
        {
            Forward,
            Disabled,
            Backward
        }
        [SerializeField]
        private Simulate simulate = Simulate.Forward;

        private void Awake()
        {
            integrator = new VerletIntegrator<BoundTransformParticle>();
        }

        private void Start()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            Step();
        }

        public void Initialize()
        {
            var transforms = spawner.SpawnLattice();
            var particles = new List<BoundTransformParticle>(transforms.Count);
            foreach (var t in transforms)
            {
                var particle = new BoundTransformParticle()
                {
                    transform = t,
                    Position = t.position,
                    PreviousPosition = t.position,
                    Mass = 1f
                };
                particles.Add(particle);
            }
            integrator.Particles = particles;
        }

        private void Step()
        {
            switch (simulate)
            {
                case Simulate.Disabled:
                    break;
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
