﻿using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegratorController : MonoBehaviour
    {
        [SerializeField]
        private LatticeSpawner spawner = null;
        [SerializeField]
        private Transform projectileSpawn = null;
        [SerializeField]
        private Vector3 projectoleVelocity = Vector3.zero;
        [SerializeField]
        private GameObject projectilePrefab = null;

        [Header("Parameters")]
        [SerializeField]
        private float width = 9.63f;
        [SerializeField]
        private float sweetSpot = 4.06f;
        [SerializeField]
        [Range(0.01f, 1f)]
        private float decay = 0.51f;
        [SerializeField]
        private float repulsion = 5.24f;
        [SerializeField]
        private float attraction = 4.83f;
        [SerializeField]
        [Range(0f, 1f)]
        private float energyConservation = 1f;
        [SerializeField]
        private float timestep = 0.01f;

        private bool dirty = true;
        private VerletIntegrator<LinkedTransformParticle> integrator;

        public enum Simulate
        {
            Forward,
            Disabled,
            Backward
        }
        [SerializeField]
        private Simulate simulate = Simulate.Forward;

        private void OnValidate()
        {
            width = Mathf.Max(0f, width);
            sweetSpot = Mathf.Max(0f, sweetSpot);
            dirty = true;
        }

        private void Awake()
        {
            integrator = new VerletIntegrator<LinkedTransformParticle>();
        }

        private void Start()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            if (dirty)
            {
                integrator.width = width;
                integrator.sweetSpot = sweetSpot;
                integrator.decay = decay;
                integrator.repulsion = repulsion;
                integrator.attraction = attraction;
                integrator.energyConservation = energyConservation;
                integrator.timestep = timestep;
                dirty = false;
            }
            Step();
        }

        public void Initialize()
        {
            var transforms = spawner.SpawnLattice();
            var particles = new List<LinkedTransformParticle>(transforms.Count);
            foreach (var t in transforms)
            {
                var particle = new LinkedTransformParticle()
                {
                    transform = t,
                    Position = t.position,
                    PreviousPosition = t.position,
                    Mass = 1f
                };
                particles.Add(particle);
            }

            var projectile = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity).transform;
            var projectileParticle = new LinkedTransformParticle()
            {
                transform = projectile,
                Position = projectile.position,
                PreviousPosition = projectile.position - projectoleVelocity * timestep,
                Mass = 5f
            };
            particles.Add(projectileParticle);

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
