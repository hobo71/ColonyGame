using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryRoutes : MonoBehaviour {

    private static readonly List<routeSolotype> routes = new List<routeSolotype>();
    private static readonly List<routeAllType> routesAllType = new List<routeAllType>();
    private static readonly List<GameObject> workers = new List<GameObject>();
    private static int counter = 100;

    public static void addRoute(GameObject origin, GameObject target, ressources type) {
        if (routeExists(origin, target)) {
            return;
            
        }
        
        routes.Add(new routeSolotype(origin, target, type));
        Debug.Log("created new route from " + origin.name + " to" + target.name + " with kind " + type);
    }

    public static void addRoute(GameObject origin, GameObject target) {
        if (routeExists(origin, target)) {
            return;
            
        }
        routesAllType.Add(new routeAllType(origin, target));
        Debug.Log("created new route from " + origin.name + " to" + target.name + " with kind all");
    }

    private static bool routeExists(GameObject origin, GameObject target) {
        foreach (var route in routes) {
            if (route.getOrigin().Equals(origin) && route.getTarget().Equals(target)) {
                return true;
            }
        }
        foreach (var route in routesAllType) {
            if (route.getOrigin().Equals(origin) && route.getTarget().Equals(target)) {
                return true;
            }
        }

        return false;
    }

    public static bool deleteRoute(GameObject origin, GameObject target, ressources type) {
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
            if (route.getOrigin().GetComponent<inventory>().getAmount(route.getType()) >= 20f) {
                if (route.getWorkerCount() < 1) {
                    GameObject worker = getIdleMover(route);
                    if (worker != null) {
                        route.addWorker(worker);
                        workers.Add(worker);
                    }
                }
            }

            if (route.getWorkerCount() >= 1 && route.getOrigin().GetComponent<inventory>().getFillPercent(route.getType()) <= 0.001f) {
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

        foreach(routeAllType route in routesAllType) {

            checkDestroyed(route);

            route.updateWorkers();
            if (route.getOrigin().GetComponent<inventory>().getAmount() >= 20f) {
                if (route.getWorkerCount() < 1) {
                    GameObject worker = getIdleMover(route);
                    if (worker != null) {
                        route.addWorker(worker);
                        workers.Add(worker);
                    }
                }
            }

            if (route.getWorkerCount() >= 1 && route.getOrigin().GetComponent<inventory>().getFillPercent() <= 0.001f) {
                route.reduceWorkerCount();
            }

            if (route.getWorkerCount() >= 2 && route.getOrigin().GetComponent<inventory>().getFillPercent() < 0.6f) {
                route.reduceWorkerCount();
            }
            
            if (route.getOrigin().GetComponent<inventory>().getFillPercent() > 0.7f && route.getWorkerCount() >= 1) {
                if (route.getWorkerCount() <= 3) {
                    GameObject worker = getIdleMover(route);
                    if (worker != null) {
                        route.addWorker(worker);
                        workers.Add(worker);
                    }
                }
            }

            if (route.getOrigin().GetComponent<inventory>().getFillPercent() > 0.85f && route.getWorkerCount() >= 1) {
                GameObject worker = getIdleMover(route);
                if (worker != null) {
                    route.addWorker(worker);
                    workers.Add(worker);
                }
            }
            
        }
	}

    private void checkDestroyed(route route) {
        if (route.getOrigin() == null || route.getTarget() == null) {
            if (route is routeSolotype)
                routes.Remove((routeSolotype) route);
            else
                routesAllType.Remove((routeAllType) route);
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

    private GameObject getIdleMover(route route) {
        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        try {
            GameObject result = harvestableRessource.GetClosestMover(movers, true, route.getTarget().transform).gameObject;
            return result;
        } catch (NullReferenceException ex) {
            return null;
        }
    }


    public class routeSolotype : route {

        private GameObject origin;
        private GameObject target;
        private ressources type;
        private List<GameObject> workers = new List<GameObject>();

        public routeSolotype(GameObject origin, GameObject target, ressources type) {
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

        public ressources getType() {
            return type;
        }

        public bool isSame(GameObject origin, GameObject target, ressources type) {
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

    public class routeAllType : route {

        private GameObject origin;
        private GameObject target;
        private List<GameObject> workers = new List<GameObject>();

        public routeAllType(GameObject origin, GameObject target) {
            this.origin = origin;
            this.target = target;
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

        public bool isSame(GameObject origin, GameObject target) {
            if (origin == this.origin && target == this.target) {
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

    public interface route {
        void reduceWorkerCount();
        void updateWorkers();
        void addWorker(GameObject worker);
        List<GameObject> getWorkers();
        int getWorkerCount();
        GameObject getTarget();
        GameObject getOrigin();

    }
}
