using System;
using UnityEngine;

namespace Mads
{
    public static class Formulas
    {
        /// <summary>
        /// Calculates the pull gradient at distance x from a particle.
        /// It is an approximation based on exponential decay for repulsion
        /// and an offset gaussian curve for attraction.
        /// </summary>
        /// <param name="x">The distance from the other particle.</param>
        /// <param name="mass">The product of the masses of the two particles</param>
        /// <param name="attraction">Attraction strength multiplier.</param>
        /// <param name="repulsion">Repulsion strength multiplier</param>
        /// <param name="decay">The repulsive exponential decay factor (should be in the range [0, 1]).</param>
        /// <param name="mu">The center of the attractive gaussion bell curve.</param>
        /// <param name="sigmaSquared">The width of the attractive gaussian bell curve.</param>
        /// <returns></returns>
        public static float PushPullForceExp(float x, float mass, float attraction, float repulsion, float decay, float mu, float sigmaSquared)
        {
            return (repulsion * Mathf.Pow(decay, x) - attraction * Mathf.Exp(mu * mu / sigmaSquared)) * mass;
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
        /// <param name="mu">The center of the attractive gaussion bell curve.</param>
        /// <param name="sigmaSquared">The width of the attractive gaussian bell curve.</param>
        /// <returns></returns>
        public static float PushPullForceHyperbolic(float x, float mass, float attraction, float repulsion, float mu, float sigmaSquared)
        {
            return (repulsion / Mathf.Max(Mathf.Epsilon, x) - attraction * Mathf.Exp(mu * mu / sigmaSquared)) * mass;
        }

        internal static Vector3 PushPullForceExp(float v1, float v2, object attraction, object repulsion, object decay, object mu, object sigmaSquared)
        {
            throw new NotImplementedException();
        }
    }

}