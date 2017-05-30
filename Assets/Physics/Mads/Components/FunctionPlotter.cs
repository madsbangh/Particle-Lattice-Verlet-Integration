using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class FunctionPlotter : MonoBehaviour
{
    [SerializeField]
    private int steps = 100;
    [SerializeField]
    private float max = 20f;
    [SerializeField]
    private float scale = 1f;

    public enum Function
    {
        PPExp,
        PPExpDerivative,
        PPRecip,
        Gaussian,
        GaussianDerivative,
        PowDerivative
    }
    [SerializeField]
    Function function = Function.Gaussian;

    [Space]
    [Header("Formula Params")]
    [SerializeField]
    private float mass = 0f;
    [SerializeField]
    private float width = 0f;
    [SerializeField]
    private float repulsion = 0f;
    [SerializeField]
    private float attraction = 0f;
    [SerializeField]
    private float decay = 0f;
    [SerializeField]
    private float sweetSpot = 0f;

    private LineRenderer lineRenderer;
    private bool dirty = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (dirty)
        {
            var positions = new List<Vector3>();
            for (float x = 0f; x < max; x += max / Mathf.Max(1, steps))
            {
                float val;
                switch (function)
                {
                    case Function.PPExp:
                        val = Mads.Formulas.PushPullExp(x, mass, attraction, repulsion, decay, sweetSpot, width);
                        break;
                    case Function.PPExpDerivative:
                        val = Mads.Formulas.PushPullExpDerivative(x, mass, attraction, repulsion, decay, sweetSpot, width);
                        break;
                    case Function.PPRecip:
                        val = Mads.Formulas.PushPullHyperbolic(x, mass, attraction, repulsion, sweetSpot, width);
                        break;
                    case Function.Gaussian:
                        val = Mads.Formulas.Gaussian(x, sweetSpot, width) * attraction;
                        break;
                    case Function.GaussianDerivative:
                        val = Mads.Formulas.GaussianDerivative(x, sweetSpot, width) * attraction;
                        break;
                    case Function.PowDerivative:
                        val = Mads.Formulas.PowDerivative(decay, x) * repulsion;
                        break;
                    default:
                        val = 0f;
                        break;
                }
                positions.Add(new Vector3(x * scale, val * scale, 0f));
            }
            lineRenderer.SetPositions(positions.ToArray());
            lineRenderer.positionCount = steps;
            lineRenderer.loop = false;
        }

    }

    private void OnValidate()
    {
        dirty = true;
    }
}
