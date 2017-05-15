using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alex
{
	public class VerletCloth : MonoBehaviour 
	{
		struct Point 
		{
			public Vector3 curPos;
			public Vector3 oldPos; 
			public bool moveable; 
		};

		struct Constraints
		{
			public int index0;
			public int index1; 
			public float restLength; 
		}

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

		// Use this for initialization
		IEnumerator Start () 
		{
			CreateGrid ();
			CreateBones ();
			run = true; 
			Debug.Log ("I run"); 
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
					Constraints c = m_constraints [i]; 
					Vector3 delta = m_points [c.index1].curPos - m_points [c.index0].curPos;
					//float deltaLength = Mathf.Sqrt (Vector3.Dot (delta, delta)); 
					//float diff = (deltaLength - c.restLength) / deltaLength; 
					delta *= c.restLength*c.restLength/(Vector3.Dot(delta, delta)+c.restLength*c.restLength)-softness; 
					//x [0] -= delta;// * 0.5f * diff; 
					//x[1] += delta;// * 0.5f * diff; 

					m_points [c.index0].curPos -= delta;// * 0.5f * diff; 
					m_points [c.index1].curPos += delta;// * 0.5f * diff; 
				}	
				m_points [0].curPos = new Vector3 (0, 10, 0); 
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

			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					GameObject tmp = GameObject.CreatePrimitive (PrimitiveType.Cube);
					tmp.transform.position = new Vector3 (i, j, 0); 
					tmp.transform.SetParent (this.transform); 
					tmpList.Add (tmp.transform); 
				}
			}

			bones = tmpList.ToArray (); 
		}

		void CreateBones()
		{
			if (bones == null)
			{
				bones = GetComponentsInChildren<Transform> (); 
				Debug.Log ("I found " + bones.Length + " bones."); 
			} 
			else 
			{
				Debug.Log ("Already done and I found " + bones.Length + " bones."); 
			}

			m_numpoints = bones.Length; 
			m_points = new Point[m_numpoints]; 

			m_numconstraints = (m_numpoints * m_numpoints) - m_numpoints; 
			m_constraints = new Constraints[m_numconstraints]; 

			for (int i = 0; i < m_numpoints; i++) {
				Vector3 pos = bones [i].transform.position; 
				m_points [i].curPos = pos; 
				m_points [i].oldPos = pos; 
				m_points [i].moveable = true; 
			}

			// create constraints
			for (int i = 0; i < m_numpoints;i++)
			{
				int x = i % 10; // 10 is the width
				int y; 
				if (i < m_numpoints - 1)
					y = (i / 10) + 1; // Fix this so it wraps around instead. 
				else
					y = (i / 10); 
				
				int i1 = x + 10 * y; 

				Debug.Log ("i1 is: " + i1 + " at: " + i); 
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