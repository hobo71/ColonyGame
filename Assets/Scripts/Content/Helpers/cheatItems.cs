using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheatItems : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<inventory>().add(new ressourceStack(3000, ressources.Wood));
		this.GetComponent<inventory>().add(new ressourceStack(3000, ressources.Stone));
		this.GetComponent<inventory>().add(new ressourceStack(300, ressources.OreIron));
		this.GetComponent<inventory>().add(new ressourceStack(300, ressources.Gold));
		this.GetComponent<inventory>().add(new ressourceStack(600, ressources.Iron));
		this.GetComponent<inventory>().add(new ressourceStack(1000, ressources.Water));
		this.GetComponent<inventory>().add(new ressourceStack(100, ressources.Uranium));
		this.GetComponent<inventory>().add(new ressourceStack(1000, ressources.Scrap));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
