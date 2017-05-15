using System;
using UnityEngine;

namespace Mads
{
    public static class Formulas
    {
        /// <summary>
        /// Calculates the pull at distance x from a particle.
        /// It is an approximation based on exponential decay for repulsion
        /// and an offset gaussian curve for attraction.
        /// </summary>
        /// <param name="x">The distance from the other particle.</param>
        /// <param name="mass">The product of the masses of the two particles</param>
        /// <param name="attraction">Attraction strength multiplier.</param>
        /// <param name="repulsion">Repulsion strength multiplier</param>
        /// <param name="decay">The repulsive exponential decay factor (should be in the range [0, 1]).</param>
        /// <param name="sweetSpot">The center of the attractive gaussion bell curve.</param>
        /// <param name="width">The width of the attractive gaussian bell curve.</param>
        /// <returns></returns>
        public static float PushPullForceExp(float x, float mass, float attraction, float repulsion, float decay, float sweetSpot, float width)
        {
            return (repulsion * Mathf.Pow(decay, x) - attraction * Gaussian(x, sweetSpot, width)) * mass;
        }

        /// <summary>
        /// Calculates the gaussian function at x.
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="x0">The center of the bell curve</param>
        /// <param name="width">The standadrd deviation</param>
        /// <returns></returns>
        public static float Gaussian(float x, float x0, float width)
        {
            return Mathf.Exp(-((x - x0) * (x - x0)) / width);
        }

        /// <summary>
        /// Calculates the pull grdient at distance x from a particle.
        /// It is an approximation based on exponential decay for repulsion
        /// and an offset gaussian curve for attraction.
        /// </summary>
        /// <param name="x">The distance from the other particle.</param>
        /// <param name="mass">The product of the masses of the two particles</param>
        /// <param name="attraction">Attraction strength multiplier.</param>
        /// <param name="repulsion">Repulsion strength multiplier</param>
        /// <param name="decay">The repulsive exponential decay factor (should be in the range [0, 1]).</param>
        /// <param name="sweetSpot">The center of the attractive gaussion bell curve.</param>
        /// <param name="width">The width of the attractive gaussian bell curve.</param>
        public static float PushPullForceExpGrad(float x, float mass, float attraction, float repulsion, float decay, float sweetSpot, float width)
        {
            return (repulsion * PowDerivative(decay, x) - attraction * GaussianDerivative(x, sweetSpot, width)) * mass;
        }

        /// <summary>
        /// Calculates the first order derivative of f raised to the power p.
        /// </summary>
        /// <param name="f">The value</param>
        /// <param name="p">The exponent</param>
        public static float PowDerivative(float f, float p)
        {
            return Mathf.Log(f) * Mathf.Pow(f, p);
        }

        /// <summary>
        /// Calculates the first order derivative of the gaussian function.
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="x0">The center of the bell curve</param>
        /// <param name="width">The standard deviation</param>
        /// <returns></returns>
        public static float GaussianDerivative(float x, float x0, float width)
        {
            return 2f * (x - x0) * Mathf.Exp(-((x - x0) * (x - x0)) / width) / width;
        }

        /// <summary>
        /// Calculates the pull gradient at distance x from a particle.
        /// It is an approximation based on a hyperbolic equation for repulsion
        /// and an offset gaussian curve for attraction.
        /// </summary>
        /// <param name="x">The distance from the other particle.</param>
        /// <param name="mass">The product of the masses of the two particles</param>
        /// <param name="attraction">Attraction strength multiplier.</param>
        /// <param name="repulsion">Repulsion strength multiplier</param>
        /// <param name="sweetSpot">The center of the attractive gaussion bell curve.</param>
        /// <param name="std">The width of the attractive gaussian bell curve.</param>
        public static float PushPullForceHyperbolic(float x, float mass, float attraction, float repulsion, float sweetSpot, float std)
        {
            return (repulsion / Mathf.Max(Mathf.Epsilon, x) - attraction * Mathf.Exp(-((x - sweetSpot) * (x - sweetSpot)) / std)) * mass;
        }
    }
}