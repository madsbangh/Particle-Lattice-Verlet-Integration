using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletCloth2 : MonoBehaviour 
{
	class Constraints 
	{
		public int[] triangles; 
		public float restLength = 1f; 
	}

	class Point 
	{
		public Vector3 curPos = Vector3.zero; 
		public Vector3 prevPos = Vector3.zero; 
		public Constraints con; 
		//public bool moveable; 
	}

	[Header("Cloth Variables")]
	[SerializeField]
	private Vector2 clothSize;
	private float restLength = 2f; 

	[Header("Particles")]
	private Point[] m_points; 
	private Constraints m_constraints; 
	private Constraints[] m_const;
	private GameObject[] go_particles; 


	[Header("Grid Mesh")]
	private Mesh mesh;
	MeshFilter mf; 
	MeshRenderer mr; 
	private Vector3[] vertices; 

	private void Awake()
	{
		mf = GetComponent<MeshFilter> ();
		mr = GetComponent<MeshRenderer> ();

		//Init ();
		Generate (); 
	}

	private void Generate()
	{
		mf.mesh = mesh = new Mesh ();
		mesh.name = "Procedural Grid";

		vertices = new Vector3[(int)((clothSize.x + 1) * (clothSize.y + 1))]; 

		m_points = new Point[vertices.Length]; 

		for (int i = 0, y = 0; y < clothSize.y; y++) 
		{
			for (int x = 0; x <= clothSize.x; x++, i++) 
			{
				m_points [i] = new Point (); 
				vertices [i] = m_points[i].curPos = m_points[i].prevPos =  new Vector3 (x, y); 
			}	
		}

		mesh.vertices = vertices; 

		int xSize = (int)clothSize.x; 
		int ySize = (int)clothSize.y; 

		int[] triangles = new int[xSize * ySize * 6];
		//m_const = new Constraints[triangles.Length]; 


		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				int i = GetI (x, y);
				Constraints c  = new Constraints (); 
				c.triangles = new int[6];
				c.triangles [0] = vi; 
				c.triangles [3] = c.triangles [2] = vi + 1; 
				c.triangles [4] = c.triangles [1] = vi + xSize + 1; 
				c.triangles [5] = vi + xSize + 2; 

 				triangles [ti] = vi; 
				triangles [ti + 3] = triangles [ti + 2] = vi + 1; 
				triangles [ti + 4] = triangles [ti + 1] = vi + xSize + 1; 
				triangles [ti + 5] = vi + xSize + 2; 	

				m_points [i].con = c; 
			}
		}

		//m_constraints = new Constraints ();
		//m_constraints.triangles = triangles; 
		mesh.triangles = triangles; 
	}

	private void OnDrawGizmos()
	{
		if (vertices == null)
		{
			return; 
		}

		Gizmos.color = Color.black; 

		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere (vertices [i], 0.1f); 
		}
	}

	// Use this for initialization
	void Start () 
	{
		//Init (); 
		//SpawnClothParticles ();
		//DrawConstraints ();
	}

	void Update()
	{
		TimeStep (); 
	}



	private void TimeStep()
	{
		ClothVerlet ();
		SatisfyConstraints ();
		UpdateVertices (); 
	}

	private void ClothVerlet()
	{
		for (int i = 0; i < m_points.Length; i++)
		{
			if (m_points [i] == null)
				return; 
			
			Vector3 _newPos = (m_points [i].curPos -  m_points [i].prevPos) + Physics.gravity * Time.deltaTime * Time.deltaTime;
			m_points [i].prevPos = m_points[i].curPos; 
			m_points [i].curPos += _newPos; 
		}
	}

	private void SatisfyConstraints()
	{
		int numIterations = 1; 
		//int xSize = (int)clothSize.x; 
		//int ySize = (int)clothSize.y; 

		for (int k = 0; k < numIterations; k++) 
		{
			for (int i = 0; i < m_points.Length; i++)
			{
				Constraints c = m_points[i].con; 

				m_points [i].curPos = Vector3.Min (Vector3.Max (m_points[k].curPos, Vector3.zero), new Vector3 (1000, 1000, 1000)); 
				
				Vector3 delta = m_points[c.triangles[3]].curPos - m_points[c.triangles[0]].curPos; 
				delta *= c.restLength*c.restLength/ (Vector3.Dot(delta,delta)+c.restLength*c.restLength) - 0.5f; 
				m_points [c.triangles [0]].curPos -= delta;
				m_points [c.triangles [3]].curPos += delta;

//				m_constraints.triangles [ti] = vi; 
//				m_constraints. [ti + 3] = m_constraints.triangles [ti + 2] = vi + 1; 
//				m_constraints. [ti + 4] = m_constraints.triangles [ti + 1] = vi + xSize + 1; 
//				m_constraints. [ti + 5] = vi + xSize + 2; 	
//				
//				//Vector3 delta = 
//				for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
//					for (int x = 0; x < xSize; x++, ti += 6, vi++) {
//						int i = GetI(x, y); 
//						Constraints c = m_points[i].con; 
//
//						Vector3 delta = m_points[c.triangles[1]].curPos - m_points[c.triangles[0]].curPos; 
//						delta *= c.restLength*c.restLength/ (Vector3.Dot(delta,delta)+c.restLength*c.restLength) - 0.5f; 
//						m_constraints.triangles [ti] = vi; 
//						m_constraints. [ti + 3] = m_constraints.triangles [ti + 2] = vi + 1; 
//						m_constraints. [ti + 4] = m_constraints.triangles [ti + 1] = vi + xSize + 1; 
//						m_constraints. [ti + 5] = vi + xSize + 2; 		
//					}
//				}
			}
		}

		m_points [0].curPos = Vector3.zero; 
	}

	private void UpdateVertices()
	{
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = m_points [i].curPos; 
		}
	}


	// OLD

	private void SpawnClothParticles()
	{
		for (int i = 0; i < m_points.Length; i++) {
			int x = GetX (i);
			int y = GetY (i); 

			GameObject tmp = GameObject.CreatePrimitive (PrimitiveType.Cube); 
			tmp.transform.position = new Vector3 (x * restLength, y * restLength, 0);
			tmp.transform.SetParent (this.transform); 
			tmp.AddComponent<LineRenderer> ();

			go_particles [i] = tmp; 
		}
	}

	private void DrawConstraints()
	{
		for (int i = 0; i < go_particles.Length; i++) {
			int x = GetX (i); 
			int y = GetY (i); 

			LineRenderer lr = go_particles [i].GetComponent<LineRenderer> ();
			lr.positionCount = 2; 
			lr.startWidth = 0.3f; 
			lr.endWidth = 0.3f; 

			lr.SetPosition (0, go_particles [i].transform.position);

			if ((y == 0 || y == clothSize.y - 1) && x < clothSize.x - 1 && x > 1)
			{
				lr.SetPosition (1, go_particles [GetI (x + 1, y)].transform.position);
			}
			else if (y < clothSize.y - 1) {
				Debug.Log ("I is " + i + "\n x:" + x + "\n y: " + y); 
				lr.SetPosition (1, go_particles [GetI (x, y + 1)].transform.position);
			}

		}
	}

	// Grid handling stuff... 

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
