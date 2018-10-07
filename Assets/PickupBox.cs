using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBox : MonoBehaviour {
    
    private bool hasWorker = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per logical step
	void FixedUpdate () {

        if (hasWorker == true) {
            return;
        }

		GameObject worker = getIdleMover();
        if (worker != null) {
            hasWorker = true;
            worker.GetComponent<movementController>().setTarget(this.transform);
        }
	}

    private GameObject getIdleMover() {
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        try {
            GameObject result = harvestableRessource.GetClosestMover(movers, true, this.transform).gameObject;
            return result;
        } catch (NullReferenceException ex) {
            return null;
        }
    }
}
