using UnityEngine;

public class TestThing : MonoBehaviour
{
    public float mass = 10f;
    public float attraction = 1f;
    public float repulsion = 4f;
    [Range(0.001f, 1f)]
    public float decay = 0.1f;
    public float sweetSpot = 0.2f;
    public float std = 1.6f;

    private void FixedUpdate()
    {
        foreach (var other in FindObjectsOfType<TestThing>())
        {
            if (other != this)
            {
                var f = Mads.Formulas.PushPullForceExp(
                    Vector3.Distance(transform.position, other.transform.position),
                    mass,
                    attraction, repulsion, decay, sweetSpot, std);
                transform.Translate((transform.position - other.transform.position).normalized * f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sweetSpot);
    }
}
