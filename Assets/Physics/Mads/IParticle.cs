using UnityEngine;

namespace Mads
{
    public interface IParticle
    {
        Vector3 Position { get; set; }
        Vector3 PreviousPosition { get; set; }
    }
}
