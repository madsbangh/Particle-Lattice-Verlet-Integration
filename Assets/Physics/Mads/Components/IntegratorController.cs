using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegratorController : MonoBehaviour
    {
        [SerializeField]
        private LatticeSpawner latticeSpawner = null;
        [SerializeField]
        private Transform projectileSpawn = null;
        [SerializeField]
        private Vector3 projectoleVelocity = Vector3.zero;
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private float projectileMass = 1f;

        [Header("Parameters")]
        [SerializeField]
        private int iterations = 1;
        [SerializeField]
        private float width = 1f;
        [SerializeField]
        private float gaussianCenter = 1f;
        [SerializeField]
        [Range(0.01f, 1f)]
        private float decay = 0.5f;
        [SerializeField]
        private float expContribution = 1f;
        [SerializeField]
        private float gaussianContribution = 1f;
        [SerializeField]
        private float timestep = 0.01f;

        private bool dirty = true;
        private VerletIntegrator integrator;

        private void OnValidate()
        {
            width = Mathf.Max(0f, width);
            gaussianCenter = Mathf.Max(0f, gaussianCenter);
            iterations = Mathf.Clamp(iterations, 1, 24);
            dirty = true;
        }

        private void Awake()
        {
            // Create integrator instance
            integrator = new VerletIntegrator();
            // Force parameter update
            dirty = true;
        }

        private void Start()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            if (dirty)
            {
                // Update integrator parameters
                integrator.width = width;
                integrator.gaussianCenter = gaussianCenter;
                integrator.decay = decay;
                integrator.expContribution = expContribution;
                integrator.gaussianContribution = gaussianContribution;
                integrator.timestep = timestep;
                dirty = false;
            }
            Step();
        }

        public void Initialize()
        {
            // Spawn lattice
            var transforms = latticeSpawner.SpawnLattice();
            var positions = new List<Vector3>(transforms.Count);
            var previousPositions = new List<Vector3>(transforms.Count);
            var masses = new List<float>(transforms.Count);
            foreach (var transform in transforms)
            {
                positions.Add(transform.position);
                previousPositions.Add(transform.position);
                masses.Add(1f);
            }

            // Spawn projectile
            var projectile = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.identity).transform;
            transforms.Add(projectile);
            positions.Add(projectile.position);
            previousPositions.Add(projectile.position - projectoleVelocity * timestep);
            masses.Add(5f);

            // Set integrator state
            integrator.Transforms = transforms;
            integrator.Positions = positions;
            integrator.PreviousPositions = previousPositions;
            integrator.Masses = masses;
        }

    private void Step()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            for (int i = 0; i < iterations; i++)
            {
                integrator.StepForward();
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            for (int i = 0; i < iterations; i++)
            {
                integrator.StepBackward();
            }
        }
    }
}
}
