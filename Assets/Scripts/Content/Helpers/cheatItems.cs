using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheatItems : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(3000, HPHandler.ressources.Wood));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(3000, HPHandler.ressources.Stone));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(300, HPHandler.ressources.OreIron));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(300, HPHandler.ressources.Gold));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(600, HPHandler.ressources.Iron));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(1000, HPHandler.ressources.Water));
		this.GetComponent<inventory>().add(new HPHandler.ressourceStack(100, HPHandler.ressources.Uranium));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
