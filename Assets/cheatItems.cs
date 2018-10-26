using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheatItems : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(300, HPHandler.ressources.Wood));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(300, HPHandler.ressources.Stone));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
