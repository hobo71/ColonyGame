using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void deliverTo(GameObject to, bool important, HPHandler.ressources kind) {

        print("got deliver request: to=" + to + " important=" + important + " kind=" + kind);

        //get closest worker
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        Transform mover = harvestableRessource.GetClosestMover(movers, !important, to.transform);

        //stop if no worker has been found
        if (mover == null) {
            print("no mover found!");
            return;
        }

        //find container with the ressource kind
        var results = GameObject.FindGameObjectsWithTag("dropBase");

        foreach (var obj in results) {
            //check if res has kind, if true then order worker to deliver it and stop
            if (obj.GetComponent<inventory>().getAmount(kind) > 0) {
                var stack = new HPHandler.ressourceStack(obj.GetComponent<inventory>().getAmount(kind), kind);
                mover.GetComponent<ActionController>().deliverTo(obj, to, stack);
                print("handled delivery request successfully");
                return;
            }
        }

        print("unable to handle request!");
    }
}
