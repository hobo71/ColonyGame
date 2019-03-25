using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLightningAnimator : MonoBehaviour {

	private Material toAnim;

	// Use this for initialization
	void Start () {
		toAnim = this.gameObject.GetComponent<LineRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		var x = toAnim.mainTextureOffset;
		x.x += 0.1f;
		toAnim.mainTextureOffset = x;
	}
}
