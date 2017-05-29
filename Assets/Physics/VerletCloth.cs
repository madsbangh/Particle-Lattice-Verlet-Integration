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
    public bool showBuildProc = false;
    public Vector3 wind;
    public bool applyGravity = true;

    [Header("Cloth Variables")]
    [SerializeField]
    private Vector2 clothSize;
    [SerializeField]
    private float restLength = 1f;

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

    private GameObject[] square;
    private Dictionary<GameObject, int> vertexToken;
    private Dictionary<int, bool> lockedVertices;
    public Material tmpMat;
    private List<Vector3> m_x;
    private List<Vector3> m_oldx;


    struct Constraints
    {
        public int particleA;
        public int particleB;
    }

    List<Constraints> mCon;
    int[] triangles;
    //List<int> vertices; 

    private IEnumerator GenerateGrid()
    {
        mCon = new List<Constraints>();
        m_x = new List<Vector3>();
        m_oldx = new List<Vector3>();
        vertexToken = new Dictionary<GameObject, int>();
        lockedVertices = new Dictionary<int, bool>();

        //vertices = new List<int> (); 

        int gridSize = (int)(clothSize.x * clothSize.y);
        square = new GameObject[gridSize];
        int gridLength = (int)(clothSize.x);

        for (int y = 0; y < gridSize; y++)
        {
            Vector3 newPos = new Vector3((int)(y % (gridLength)), (int)(y / gridLength), 0);
            square[y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square[y].transform.position = newPos;
            square[y].name = "(" + (int)(y % (gridLength)) + ", " + (int)(y / gridLength) + ")";
            square[y].transform.SetParent(this.transform);
            square[y].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            square[y].AddComponent<LineRenderer>().material = tmpMat;
            square[y].GetComponent<LineRenderer>().positionCount = 4;
            m_x.Add(newPos);
            m_oldx.Add(newPos);
            vertexToken.Add(square[y], y);
            lockedVertices.Add(y, false); 
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

            if (showBuildProc)
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

            if (showBuildProc)
                yield return new WaitForSeconds(0.1f);
        }

        simulate = Simulate.ENABLE;

        yield return null;
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
        Interact();
    }

    private void ClothVerlet()
    {
        float tDelta = Time.deltaTime * Time.deltaTime;

        for (int i = 0; i < m_x.Count; i++)
        {
            Vector3 _newPos = (m_x[i] - m_oldx[i]) + a() * tDelta;
            m_oldx[i] = m_x[i];
            m_x[i] += _newPos;
        }
    }


    private bool dragVertex = false;
    int vertexToDrag = 0;
    int vertexToLock = 0;

    private void Interact()
    {
        Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 fwd = Vector3.forward;

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(mPos, fwd, out hit))
            {
                vertexToLock = vertexToken[hit.transform.gameObject];
                lockedVertices[vertexToLock] = !lockedVertices[vertexToLock];

                if (lockedVertices[vertexToLock])
                {
                    square[vertexToLock].GetComponent<Renderer>().material.color = Color.red;
                } 
                else
                {
                    square[vertexToLock].GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (!dragVertex)
            {
                RaycastHit hit;
                if (Physics.Raycast(mPos, fwd, out hit))
                {
                    vertexToDrag = vertexToken[hit.transform.gameObject];
                    dragVertex = true;
                }
            }
            else
            {
                m_x[vertexToDrag] = square[vertexToDrag].transform.position = new Vector3(mPos.x, mPos.y, 0);
            }

        }
         
        
        if (Input.GetMouseButtonUp(0))
        {
            dragVertex = false;
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

        for (int i = 0; i < lockedVertices.Count; i++)
        {
            if (lockedVertices[i])
            {
                m_x[i] = square[i].transform.position; 
            }
        }
    }

    private void UpdateVertices()
    {
        for (int i = 0; i < square.Length; i++)
        {
            square[i].transform.position = m_x[i];
        }

        int gridLength = (int)(clothSize.x);

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
            }
            else
            {
                lr.SetPosition(0, square[i].transform.position);
                lr.SetPosition(1, square[i].transform.position);
            }
        }

        for (int i = 0; i < square.Length - gridLength; i++)
        {
            LineRenderer lr = square[i].GetComponent<LineRenderer>();

            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;

            lr.SetPosition(2, square[i].transform.position);
            lr.SetPosition(3, square[i + gridLength].transform.position);
        }
    }

    private Vector3 a()
    {
        Vector3 tmp = Vector3.zero;

        if (wind.magnitude > 0)
            tmp += wind;

        if (applyGravity)
            tmp += Physics.gravity;

        return tmp;
    }

    private int CalculatePoint(Vector3 pos)
    {
        int result = 0;

        result = (int)(pos.y * (clothSize.x * restLength) + pos.x);

        return result;
    }

    public void UpdateCloth()
    {

    }


    // Button Functions
    public void SetGravity(bool isActive)
    {
        applyGravity = isActive;
    }

    public void SetRestLength(float newLength)
    {
        restLength = newLength;
    }

    public void SetSizeX(int value)
    {
        wind.x = value;
    }

    public void SetSizeY(int value)
    {
        wind.z = value;
    }

    public void ResetSimulation()
    {
        Debug.Log("Reset button");
        mCon.Clear();
        m_x.Clear();
        m_oldx.Clear();
        vertexToken.Clear();
        lockedVertices.Clear();

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        StartCoroutine(GenerateGrid()); 
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
