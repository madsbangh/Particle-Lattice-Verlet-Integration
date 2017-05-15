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

		public int nrTimeSteps = 10; 
		private int curTimeStep = 0; 

		// Use this for initialization
		void Start () 
		{
			int num_particles = objs.Length;
			 
			x = new Vector3[num_particles];
			x_old = new Vector3[num_particles];
			a = new Vector3[num_particles]; 

			for (int i = 0; i < num_particles; i++) {
				x [i] = start_x; 
				a [i] = start_a; 
				x_old [i] = start_old_x; 
				objs [i].transform.position = x [i]; 
			}
			
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (curTimeStep < nrTimeSteps)
			{
				TimeStep (); 
				curTimeStep++; 
			}
		}

		private void TimeStep()
		{
			AccumulateForces (); 
			Verlet ();
			CalcConstraints (); 
		}

		private void Verlet()
		{
			for (int i = 0; i < x.Length; i++) 
			{
				Vector3 temp = x [i]; 
				x [i] += x [i] - Gange(x_old [i], a [i]) * Time.deltaTime * Time.deltaTime; 
				x_old [i] = temp; 
				objs [i].transform.position = x [i]; 
				GameObject.CreatePrimitive (PrimitiveType.Sphere); 
			}
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
			
		}

		private Vector3 Gange(Vector3 a, Vector3 b)
		{
			return new Vector3 (a.x * b.x, a.y * b.y, a.z * b.z);
		}

/*		private Vector3 Acceleration()
		{
			return Vector3.one; 
		}*/
	}
}