using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alex.Maybe 
{
	public class Point 
	{
		
		private float x; 
		private float y; 
		private float z;
		private float px;
		private float py;
		private float pz;
		private float vx = 0f;
		private float vy = 0f;
		private float vz = 0f;
		private bool pinX = false;
		private bool pinY = false;
		private bool pinZ = false;

		private float dx;
		private float dy;
		private float dz; 
		private float dist; 

		public Point(float x, float y, float z)
		{
			this.x = this.px = x;
			this.y = this.py = y; 
			this.z = this.pz = z; 
		}

		public void Update(float delta)
		{
			//if (pinX && pinY && pinZ)
			//	continue; 


		}
	}
}
