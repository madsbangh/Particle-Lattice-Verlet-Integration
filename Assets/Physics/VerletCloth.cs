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

    private GameObject[] square;
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
	
		//vertices = new List<int> (); 

		int gridSize = (int)(clothSize.x * clothSize.y); 
		square = new GameObject[gridSize];
        int gridLength = (int)(clothSize.x);

		for (int y = 0; y < gridSize; y++)
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

//		triangles = new int[square.Length * 6]; 
//			
//
//		for (int ti = 0, vi = 0, y = 0; y < 4; y++, vi++) {
//			for (int x = 0; x < gridLength; x++, ti += 6, vi++) {
//				triangles [ti] = vi; 
//				triangles [ti + 3] = triangles [ti + 2] = vi + 1; 
//				triangles [ti + 4] = triangles [ti + 1] = vi + gridLength + 1; 
//				triangles [ti + 5] = vi + gridLength + 2; 	
//			}
//		}
//
//		mf = GetComponent<MeshFilter> ();
//		mr = GetComponent<MeshRenderer> ();
//		mf.mesh = mesh = new Mesh ();
//		mesh.name = "Procedural Grid"; 
//
//		mesh.triangles = triangles; 

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
		Interact (); 
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

	private void Interact()
	{
		if (Input.GetMouseButton(0))
		{
			Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 fwd = Vector3.forward;

			RaycastHit hit; 

			if (Physics.Raycast(mPos, fwd, out hit))
			{
				Debug.Log ("I hit " + hit.transform.name); 
			}

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

		int y = (int)(clothSize.x) - 1;
		int gridLength = (int)(clothSize.x); 
        m_x[y] = new Vector3((int)(y % (gridLength)), (int)(y / gridLength), 0);
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

		if(applyGravity)
			tmp += Physics.gravity; 

		return tmp;
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
