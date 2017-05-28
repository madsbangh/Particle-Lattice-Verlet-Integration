using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletCloth : MonoBehaviour
{
    public enum Simulate
    {
        ENABLE,
        DISABLE
    };

    [Header("Simulation")]
    public Simulate simulate = Simulate.ENABLE;

    [Header("Cloth Variables")]
    [SerializeField]
    private Vector2 clothSize;
    [SerializeField]
    private float restLength = 0.5f;

    [Header("Grid Mesh")]
    private Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;
    private Vector3[] vertices;

    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        StartCoroutine(GenerateGrid());
    }

    private GameObject[] square = new GameObject[40];
    public Material tmpMat;
    private List<Vector3> m_x;
    private List<Vector3> m_oldx;


    struct Constraints
    {
        public int particleA;
        public int particleB;
    }

    List<Constraints> mCon;

    private IEnumerator GenerateGrid()
    {
        mCon = new List<Constraints>();
        m_x = new List<Vector3>();
        m_oldx = new List<Vector3>();

        int gridLength = (int)(square.Length / 4);

        for (int y = 0; y < square.Length; y++)
        {
            Vector3 newPos = new Vector3((int)(y % (gridLength)), (int)(y / gridLength), 0);
            square[y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square[y].transform.position = newPos;
            square[y].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            square[y].AddComponent<LineRenderer>().material = tmpMat;
            square[y].GetComponent<LineRenderer>().positionCount = 4;
            m_x.Add(newPos);
            m_oldx.Add(newPos);
        }

        for (int i = 0; i < square.Length - 1; i++)
        {
            LineRenderer lr = square[i].GetComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;

            if (i % gridLength != gridLength - 1)
            {
                lr.SetPosition(0, square[i].transform.position);
                lr.SetPosition(1, square[i + 1].transform.position);
                lr.SetPosition(2, square[i].transform.position);
                lr.SetPosition(3, square[i].transform.position);

                Constraints tmp;
                tmp.particleA = i;
                tmp.particleB = i + 1;
                mCon.Add(tmp);
            }
            else
            {
                lr.SetPosition(0, square[i].transform.position);
                lr.SetPosition(1, square[i].transform.position);
            }
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < square.Length - gridLength; i++)
        {
            LineRenderer lr = square[i].GetComponent<LineRenderer>();

            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;

            Constraints tmp;
            tmp.particleA = i;
            tmp.particleB = i + gridLength;
            mCon.Add(tmp);

            lr.SetPosition(2, square[i].transform.position);
            lr.SetPosition(3, square[i + gridLength].transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        simulate = Simulate.ENABLE;
    }

    void Update()
    {
        if (simulate == Simulate.ENABLE)
            TimeStep();
    }

    private void TimeStep()
    {
        ClothVerlet();
        SatisfyConstraints();
        UpdateVertices();
    }

    private void ClothVerlet()
    {
        for (int i = 0; i < m_x.Count; i++)
        {
            Vector3 _newPos = (m_x[i] - m_oldx[i]) + Physics.gravity * Time.deltaTime * Time.deltaTime;
            m_oldx[i] = m_x[i];
            m_x[i] += _newPos;
        }
    }

    private void SatisfyConstraints()
    {
        int numIterations = 1;

        for (int k = 0; k < numIterations; k++)
        {
            for (int i = 0; i < mCon.Count; i++)
            {
                Constraints c = mCon[i];

                Vector3 delta = m_x[c.particleB] - m_x[c.particleA];
                delta *= restLength * restLength / (Vector3.Dot(delta, delta) + restLength * restLength) - 0.5f;
                m_x[c.particleA] -= delta;
                m_x[c.particleB] += delta;
            }
        }
        m_x[0] = Vector3.zero;

        int y = (int)(square.Length / 4) - 1;
        int gridLength = (int)(square.Length / 4); 
        m_x[y] = new Vector3((int)(y % (gridLength)), (int)(y / gridLength), 0);
    }

    private void UpdateVertices()
    {
        for (int i = 0; i < square.Length; i++)
        {
            square[i].transform.position = m_x[i];
        }
    }

    private int GetY(int i)
    {
        return (int)(i / clothSize.x);
    }

    private int GetX(int i)
    {
        return (int)(i % clothSize.x);
    }

    private int GetI(int x, int y)
    {
        return (int)(y * clothSize.x + x);
    }

    private int GetI(Vector2 pos)
    {
        return (int)(pos.y * clothSize.x + pos.x);
    }
}
