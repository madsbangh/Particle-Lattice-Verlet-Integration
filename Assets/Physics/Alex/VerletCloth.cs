using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alex
{
	public class VerletCloth : MonoBehaviour 
	{
		class Constraints
		{
			public int index0;
			public int index1; 
			public float restLength = 2f; 
		};

		class Point 
		{
			public Vector3 curPos;
			public Vector3 oldPos; 
			//public Constraints c; 
			public bool moveable; 
		};



		[SerializeField]
		private Transform[] bones; 
		private Transform[] ctrls; 
		[SerializeField]
		private Transform obj; 

		Point[] m_points; 
		Constraints[] m_constraints; 

		[SerializeField]
		[Range(0f,1f)]
		private float softness = 0.5f; 

		bool run = false; 


		// Take two
		int accuracy = 5; 
		int clothY = 8;
		int clothX = 15; 
		int spacing = 8; 
		int tearDist = 60; 
		float friction = 0.99f; 
		float bounce = 0.5f; 



		// Use this for initialization
		IEnumerator Start () 
		{
			CreateGrid ();
			CreateBones ();
			run = true; 
		//	Debug.Log ("I run"); 
			yield break; 
		}

		
		// Update is called once per frame
		void Update () 
		{
			if (run)
				TimeStep (); 	
		}

		int m_numpoints;
		int m_numconstraints; 


		private void TimeStep()
		{
			//AccumulateForce();
			ClothVerlet(); 
			SatisfyConstraints (); 
			UpdateVertices ();
		}

		/*	void AccumulateForces()
		{
			for (int i = 0; i < m_numpoints; i++) {
				
			}
		}*/

		void ClothVerlet()
		{
			for (int i = 0; i < m_numpoints; i++) 
			{
				//Vector3 temp = m_points [i].curPos; 
				Vector3 _newPos = (m_points [i].curPos -  m_points [i].oldPos) + Physics.gravity * Time.deltaTime * Time.deltaTime;
				m_points [i].oldPos = m_points[i].curPos; 
				m_points [i].curPos += _newPos; 
			}
		}

		void SatisfyConstraints()
		{
			int numIterations = 1; 

			for (int i = 0; i < numIterations; i++) 
			{
				for (int k = 0; k < m_numpoints; k++) 
				{
					m_points [k].curPos = Vector3.Min (Vector3.Max (m_points[k].curPos, Vector3.zero), new Vector3 (1000, 1000, 1000)); 

					//if (m_points [k].moveable)
					//{
						Constraints c = m_constraints [k]; 
						//Constraints c = m_points[k].c; 
						//float deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta)); 
						//float diff = (deltaLength - c.restLength) / deltaLength; 
						//x [0] -= delta;// * 0.5f * diff; 
						//x[1] += delta;// * 0.5f * diff; 

						Vector3 delta = m_points [c.index1].curPos - m_points [c.index0].curPos;
						delta *= c.restLength*c.restLength/(Vector3.Dot(delta, delta)+c.restLength*c.restLength)-softness; 
						m_points [c.index0].curPos -= delta;// * 0.5f * diff; 
						m_points [c.index1].curPos += delta;// * 0.5f * diff; 
					//}

				}	
				m_points [0].curPos = new Vector3 (0, 30, 0); 
				m_points [clothX].curPos = new Vector3 (30, 30, 0);
			}
		}

		void UpdateVertices()
		{
			for (int i = 0; i < m_numpoints; i++) 
			{
				bones [i].transform.position = m_points [i].curPos; 
			}
		}

		void CreateGrid()
		{
			List<Transform> tmpList = new List<Transform> (); 
	//		float startX = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width,0,0)).x / 2 - clothX * spacing / 2; 
			for (int y = 0; y < clothY; y++) {
				for (int x = 0; x < clothX; x++) {
					
					GameObject tmp = GameObject.CreatePrimitive (PrimitiveType.Cube);
					tmp.transform.position = new Vector3 (x, y, 0); 
					tmp.transform.SetParent (this.transform); 
					tmpList.Add (tmp.transform); 
				}
			}

			bones = tmpList.ToArray (); 
		}

		void CreateBones()
		{

			//List<Transform> tmpList = new List<Transform> (); 
			List<Point> nPoints = new List<Point> (); 
			//bones = tmpList.ToArray (); 
			
			if (bones == null)
			{
				bones = GetComponentsInChildren<Transform> (); 
				Debug.Log ("I found " + bones.Length + " bones."); 
			} 


			//else 
			//{
				//Debug.Log ("Already done and I found " + bones.Length + " bones."); 
			//}

			m_numpoints = bones.Length; 

			float startX = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width,0,0)).x / 2 - clothX * spacing / 2; 
			for (int y = 0; y < clothY; y++) {
				for (int x = 0; x < clothX; x++) {
					Point newPoint = new Point(); 
					newPoint.curPos = new Vector3(startX + x * spacing, 20 + y * spacing, 0f);
					newPoint.oldPos = newPoint.curPos; 

					//newPoint.c.index0 = x + clothX * y; 

					if (y == 0)
						newPoint.moveable = false; 
					
				//	if (x != 0)
				//		newPoint.c.index1 = m_numpoints - 1; 

				//	if (y != 0)
				//		newPoint.c.index1 = x + (y - 1) * (clothX +1 );

					
					//GameObject tmp = GameObject.CreatePrimitive (PrimitiveType.Cube);
					//tmp.transform.position = new Vector3 (x, y, 0); 
					//tmp.transform.SetParent (this.transform); 
					//tmpList.Add (tmp.transform); 
					nPoints.Add(newPoint); 
				}
			}

			m_points = nPoints.ToArray (); //new Point[m_numpoints]; 
/*			for (int i = 0; i < m_numpoints; i++) 
			{
				float len = (m_points[m_points[i].c.index1].curPos - m_points[m_points[i].c.index0].curPos).magnitude;
				m_points[i].c.restLength = len; 
				//m_constraints[i].restLength = len;
				
			}*/
			
			m_numconstraints = clothX * clothY * 2 - clothX - clothY; //(m_numpoints * m_numpoints) - m_numpoints; 
			//Debug.Log(m_numconstraints);
			m_constraints = new Constraints[m_numconstraints]; 


			//TODO: Continue working from here!!!
/*			for (int k = 0; k < m_numpoints; k++) 
			{
				int x = k % clothX; // 10 is the width
				int y = (int)(k / clothX); 

				//int y; 
				//if (i < m_numpoints)
				//	y = (int)(i / 10); // Fix this so it wraps around instead. 
			}*/

			for (int i = 0; i < m_constraints.Length; i++) 
			{
				int x = i % clothX; 
				int y = (int)(i / clothX); 

				m_constraints [i] = new Constraints (); 
				m_constraints [i].index0 = i; 

				if (x == clothX && y != clothY)
				{
					m_constraints [i].index1 = ((y - 1) * clothX) + (x); 
				} 
				else if (y == 0 || y == clothX)
				{
					m_constraints [i].index1 = ((y + 1) * clothX) + (x); 
				} else 
				{
					m_constraints [i].index1 = (y * clothX) + (x + 1); 
				}

			}

//			int i = 0; 
//			for (int y = 0; y < clothY; y++) {
//				for (int x = 0; x < clothX; x++) {
//					if ((x+1) < clothX)
//					{
//						m_constraints [i] = new Constraints (); 
//						m_constraints [i].index0 = x + y * clothX; 
//						m_constraints [i].index1 = x + 1 + y * clothX; 
//						float len = (m_points[x].curPos - m_points[x + 1].curPos).magnitude;
//						i++;
//					}
//					if ((y+1) < clothY)
//					{
//						m_constraints [i] = new Constraints (); 
//						m_constraints [i].index0 = x + y * clothX; 
//						m_constraints [i].index1 = x + clothX + y * clothX; 
//						float len = (m_points[x].curPos - m_points[x + clothX].curPos).magnitude;
//						i++;
//					}
//
//					if (i > m_numconstraints)
//						return; 
//				}
//			}
			//Debug.Log (i); 
			/*
			for (int i = 0; i < m_numpoints; i++) {
				Vector3 pos = bones [i].transform.position; 
				m_points [i].curPos = pos; 
				m_points [i].oldPos = pos; 
				m_points [i].moveable = true; 
			}
			// create constraints
			for (int i = 0; i < m_numpoints;i++)
			{
				//int x = i % 10; // 10 is the width
				//int y; 
				//if (i < m_numpoints - 1)
				//	y = (int)(i / 10); // Fix this so it wraps around instead. 
				//else
				//	y = (int)(i / 10); 
				
				int i1 = i + 1; // x + 10 * y; 

				//Debug.Log ("i1 is: " + i1 + " at: " + i); 

				float len;
				if (i < m_numpoints - 1)
					len = (m_points[i].curPos - m_points[i1].curPos).magnitude;
				else 
					len = (m_points[i].curPos - m_points[0].curPos).magnitude;
				
				m_constraints[i].restLength = len;
				m_constraints[i].index0 = i;
				m_constraints[i].index1 = i1;
				//c++;	
			}
*/

/*			int c = 0;

			for (int i=0; i<m_numpoints; i++)
			{
				for (int k=0; k<m_numpoints; k++)
				{
					if (i!=k)
					{
						float len = (m_points[i].curPos - m_points[k].curPos).magnitude;

						m_constraints[c].restLength = len;
						m_constraints[c].index0 = i;
						m_constraints[c].index1 = k;
						c++;
					}
				}
			}*/

		}
	}
}