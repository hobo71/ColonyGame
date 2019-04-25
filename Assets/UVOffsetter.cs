using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class UVOffsetter : MonoBehaviour {
	private MeshRenderer meshRenderer;
	public Vector2 offset;

	// Use this for initialization
	void Start () {
		meshRenderer = this.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		var material = meshRenderer.material.mainTextureOffset += offset;
	}
}
