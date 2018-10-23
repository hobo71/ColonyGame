using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryRoutes : MonoBehaviour {

    private static List<routeSolotype> routes = new List<routeSolotype>();
    private static List<GameObject> workers = new List<GameObject>();
    private static int counter = 100;

    public static void addRoute(GameObject origin, GameObject target, HPHandler.ressources type) {
        routes.Add(new routeSolotype(origin, target, type));
        Debug.Log("created new route from " + origin.name + " to" + target.name + " with kind " + type);
    }

    public static bool deleteRoute(GameObject origin, GameObject target, HPHandler.ressources type) {
        Debug.Log("deleting route...");
        foreach (routeSolotype route in routes) {
            if (route.isSame(origin, target, type)) {
                routes.Remove(route);
            Debug.Log("succesfully deleted route");
                return true;
            }
        }

        return false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        /*if (counter%100 == 0) {
            counter = 0;
            workers = new List<GameObject>(GameObject.FindGameObjectsWithTag("mover"));
        }*/
		counter++;

        //actual logic
        foreach(routeSolotype route in routes) {

            checkDestroyed(route);

            route.updateWorkers();
            if (route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) > 0.4f) {
                if (route.getWorkerCount() < 1) {
                    GameObject worker = getIdleMover(route);
                    if (worker != null) {
                        route.addWorker(worker);
                        workers.Add(worker);
                    }
                }
            }

            if (route.getWorkerCount() >= 1 && route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) < 0.05f) {
                route.reduceWorkerCount();
            }

            if (route.getWorkerCount() >= 2 && route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) < 0.6f) {
                route.reduceWorkerCount();
            }
            
            if (route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) > 0.7f && route.getWorkerCount() >= 1) {
                if (route.getWorkerCount() <= 3) {
                    GameObject worker = getIdleMover(route);
                    if (worker != null) {
                        route.addWorker(worker);
                        workers.Add(worker);
                    }
                }
            }

            if (route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) > 0.85f && route.getWorkerCount() >= 1) {
                GameObject worker = getIdleMover(route);
                if (worker != null) {
                    route.addWorker(worker);
                    workers.Add(worker);
                }
            }
            
        }
	}

    private void checkDestroyed(routeSolotype route) {
        if (route.getOrigin() == null || route.getTarget() == null) {
            routes.Remove(route);
        }
    }

    public static Transform getClosest(string tag, GameObject from) {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = from.transform.position;

        foreach(GameObject potentialTarget in targets) {
            
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
     
        return bestTarget;
    }

    private GameObject getIdleMover(routeSolotype route) {
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        try {
            GameObject result = harvestableRessource.GetClosestMover(movers, true, route.getTarget().transform).gameObject;
            return result;
        } catch (NullReferenceException ex) {
            return null;
        }
    }

    public class routeSolotype {

        private GameObject origin;
        private GameObject target;
        private HPHandler.ressources type;
        private List<GameObject> workers = new List<GameObject>();

        public routeSolotype(GameObject origin, GameObject target, HPHandler.ressources type) {
            this.origin = origin;
            this.target = target;
            this.type = type;
        }

        public void reduceWorkerCount() {
            workers[0].GetComponent<ActionController>().stopDeliveryRoute();
            workers.RemoveAt(0);
        }
        
        public GameObject getOrigin() {
            return origin;
        }

        public GameObject getTarget() {
            return target;
        }

        public HPHandler.ressources getType() {
            return type;
        }

        public bool isSame(GameObject origin, GameObject target, HPHandler.ressources type) {
            if (origin == this.origin && target == this.target && type == this.type) {
                return true;
            }

            return false;
        }

        public int getWorkerCount() {
            return workers.Count;
        }

        public List<GameObject> getWorkers() {
            return workers;
        }

        public void addWorker(GameObject worker) {
            workers.Add(worker);
            worker.GetComponent<ActionController>().setRoute(this);
        }

        public void updateWorkers() {
            List<GameObject> toRemove = new List<GameObject>();

            foreach(GameObject worker in workers) {
                if (worker.GetComponent<ActionController>().getState() != ActionController.State.RouteDelivering) {
                    toRemove.Add(worker);
                }
            }

            workers = workers.Except(toRemove).ToList();
        }

    }
}
