using System.Collections.Generic;
using UnityEngine;

namespace Mads
{
    public class IntegerVerletIntegrator
    {
        public int width = 963;
        public int sweetSpot = 406;
        public int decay = 051;
        public int repulsion = 524;
        public int attraction = 483;

        public List<IntegerLinkedTransformParticle> Particles { get; set; }

        public void StepForward()
        {
            var acceleration = new int[Particles.Count * 3];

            for (int i = 0; i < Particles.Count; i++)
            {
                for (int j = 0; j < Particles.Count; j++)
                {
                    var selfToOther = new int[]
                    {
                        Particles[j].Position[0] - Particles[i].Position[0],
                        Particles[j].Position[1] - Particles[i].Position[1],
                        Particles[j].Position[2] - Particles[i].Position[2]
                    };

                    // HACK: Mass is not implemented correctly yet
                    var selfToOtherLength = Mathf.FloorToInt(Mathf.Sqrt(selfToOther[0] * selfToOther[0] + selfToOther[1] * selfToOther[1] + selfToOther[2] * selfToOther[2]));
                    int[] selfToOtherNormalized;
                    if (selfToOtherLength > 0)
                    {
                        selfToOtherNormalized = new int[]
                        {
                            selfToOther[0] / selfToOtherLength,
                            selfToOther[1] / selfToOtherLength,
                            selfToOther[2] / selfToOtherLength
                        };
                    }
                    else
                    {
                        selfToOtherNormalized = new int[3];
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        acceleration[i + k] -= selfToOtherNormalized[k]
                                    * Mathf.FloorToInt(Formulas.PushPullForceExpGrad(selfToOtherLength * 0.01f,
                                    Particles[k].Mass * Particles[j].Mass / 10000f,
                                    attraction * 0.01f, repulsion * 0.01f, decay * 0.01f, sweetSpot * 0.01f, width * 0.01f));
                    }
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    nextPosition[j] = 2 * Particles[i].Position[j] - Particles[i].PreviousPosition[j] + acceleration[i + j];
                    Particles[i].PreviousPosition = Particles[i].Position;
                    Particles[i].Position = nextPosition;
                }
            }
        }

        public void StepBackward()
        {
            var acceleration = new int[Particles.Count * 3];

            for (int i = 0; i < Particles.Count; i++)
            {
                for (int j = 0; j < Particles.Count; j++)
                {
                    var selfToOther = new int[]
                    {
                        Particles[j].Position[0] - Particles[i].Position[0],
                        Particles[j].Position[1] - Particles[i].Position[1],
                        Particles[j].Position[2] - Particles[i].Position[2]
                    };

                    // HACK: Mass is not implemented correctly yet
                    var selfToOtherLength = Mathf.FloorToInt(Mathf.Sqrt(selfToOther[0] * selfToOther[0] + selfToOther[1] * selfToOther[1] + selfToOther[2] * selfToOther[2]));
                    var selfToOtherNormalized = new int[]
                    {
                        selfToOther[0] / selfToOtherLength,
                        selfToOther[1] / selfToOtherLength,
                        selfToOther[2] / selfToOtherLength
                    };
                    for (int k = 0; k < 3; k++)
                    {
                        acceleration[i + k] -= selfToOtherNormalized[k]
                                    * Mathf.FloorToInt(Formulas.PushPullForceExpGrad(selfToOtherLength * 0.01f,
                                    Particles[k].Mass * Particles[j].Mass / 10000f,
                                    attraction * 0.01f, repulsion * 0.01f, decay * 0.01f, sweetSpot * 0.01f, width * 0.01f));
                    }
                }
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                var nextPosition = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    nextPosition[j] = 2 * Particles[i].PreviousPosition[j] - Particles[i].Position[j] + acceleration[i + j];
                    Particles[i].Position = Particles[i].PreviousPosition;
                    Particles[i].PreviousPosition = nextPosition;
                }
            }
        }
    }
}