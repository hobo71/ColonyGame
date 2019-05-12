using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRotation : MonoBehaviour {

	public Vector3 axis;
	public float angle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.Rotate(axis, angle);
	}
}
