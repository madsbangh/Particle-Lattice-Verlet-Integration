using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alex
{
	public class VerletIntegration : MonoBehaviour 
	{
		public GameObject[] objs; 
		public Vector3 start_x; 
		public Vector3 start_old_x;
		public Vector3 start_a; 

		Vector3[] x; // Current particle positions
		Vector3[] x_old; // Old particle positions
		Vector3[] a; // Force Accumulators 
		float deltaTime_old; 


		[SerializeField]
		[Range(0f,1f)]
		private float softness = 0.5f; 

		public float restLength = 10f; 

		public int nrTimeSteps = 20; 
		private int curTimeStep = 0; 

		// Use this for initialization
		void Start () 
		{
			int num_particles = objs.Length;
			 
			x = new Vector3[num_particles];
			x_old = new Vector3[num_particles];
			a = new Vector3[num_particles]; 

			for (int i = 0; i < num_particles; i++) 
			{
				x [i] = objs[i].transform.position; 
				a [i] = start_a; 
				x_old [i] = x[i] - start_old_x; 
				//objs [i].transform.position = x [i]; 
			}

			deltaTime_old = Time.fixedDeltaTime; 
		}
		
		// Update is called once per frame
		void Update () 
		{
			TimeStep (); 
				//curTimeStep++; 
		}

		private void TimeStep()
		{
			AccumulateForces (); 
			Verlet ();
			//TimeCorrVerlet();
			CalcConstraints ();

			for (int i = 0; i < objs.Length; i++) 
			{
				objs [i].transform.position = x [i]; 
			}
		}

		private void Verlet()
		{
			for (int i = 0; i < x.Length; i++) 
			{
				Vector3 temp = x [i]; 
				Vector3 _newPos = (x [i] -  x_old[i]) + a[i] * Time.deltaTime * Time.deltaTime;
				//x [i] += x [i] -  Vector3.Scale(x_old[i], a[i]) * Time.fixedDeltaTime * Time.fixedDeltaTime; 
				x_old [i] = temp; 
				x [i] += _newPos; 
				//GameObject.CreatePrimitive (PrimitiveType.Sphere).transform.position = x[i]; 
			}
		}

		private void TimeCorrVerlet()
		{
			for (int i = 0; i < x.Length; i++) 
			{
				Vector3 temp = x [i]; 
				Vector3 _newPos = (x [i] -  x_old[i]) * (Time.fixedDeltaTime/deltaTime_old) + a[i] * Time.fixedDeltaTime * Time.fixedDeltaTime;
				x_old [i] = temp; 
				x [i] += _newPos; 
			}
			deltaTime_old = Time.fixedDeltaTime; 
		}

		private void AccumulateForces()
		{
			for (int i = 0; i < x.Length; i++) 
			{
				a [i] = Physics.gravity; 
			}
		}

		private void CalcConstraints()
		{
			for (int j = 0; j < 10; j++) 
			{
				
				for (int i = 0; i < x.Length; i++) 
				{
					x [i] = Vector3.Min (Vector3.Max (x[i], Vector3.zero), new Vector3 (1000, 1000, 1000)); 
				}
					
				Vector3 delta = x[1] - x[0]; 
				//float deltaLength = Mathf.Sqrt(Vector3.Dot(delta,delta));
				//float diff = (deltaLength-restLength)/deltaLength; 
				delta *= restLength*restLength/(Vector3.Dot(delta, delta)+restLength*restLength)-softness; 
				x [0] -= delta;// * 0.5f * diff; 
				x [1] += delta;// * 0.5f * diff; 
			}
		}
			
	}
}