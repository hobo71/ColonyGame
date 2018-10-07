using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class onClickHandler : MonoBehaviour {
    
	// Use this for initialization
	void Start () {

	}

    public void handleClick() {
        Debug.Log("i got clicked: " + this.name);
        switch (this.gameObject.tag) {
            case "dropBase":
                deliverIdles();
                break;
            case "clickable":
                moveHere();
                break;
            default:
                break;
        }
    }

    private void deliverIdles() {
        Debug.Log("setting units to deliver now");

        GameObject[] carriers = GameObject.FindGameObjectsWithTag("mover");

        foreach (GameObject carrier in carriers) {
            if (carrier.GetComponent<inventory>().getAmount() > 0) {
                carrier.GetComponent<movementController>().setTarget(this.transform);
            }
        }

    }

    public void moveHere() {
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        Transform nearestMover = GetClosestMover(movers, true);

        if (nearestMover == null) {
            Debug.Log("no idle mover available");
            nearestMover = GetClosestMover(movers, false);
        }

        nearestMover.GetComponent<movementController>().setTarget(this.transform);

    }

    public void handleLongClick() {
        Debug.Log("i got longclicked: " + this.name);
        
        this.GetComponent<ClickOptions>().Create();

        
    }

    Transform GetClosestMover (GameObject[] movers, bool notIdle) {

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach(GameObject potentialTarget in movers) {

            if (!potentialTarget.GetComponent<ActionController>().getState().Equals(ActionController.State.Idle) && notIdle) {
                continue;
            }

            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
     
        return bestTarget;
    }

}
