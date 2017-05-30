using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickScript : MonoBehaviour 
{
	public Transform otherObj; 
	private LineRenderer lr; 

	// Use this for initialization
	void Start () 
	{
		lr = GetComponent<LineRenderer> (); 
		lr.positionCount = 2; 
	}
	
	// Update is called once per frame
	void Update () 
	{
		lr.SetPosition (0, this.transform.position); 
		lr.SetPosition (1, otherObj.position); 
	}
}
