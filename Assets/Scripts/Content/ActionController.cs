using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour, SaveLoad.SerializableInfo {

    public int APS = 2;
    public float AD = 5.0f;
    public float range = 1.5f;

    private float timer = 0f;
    private int idleTimer = 0;
    private int idleDeliveryTimer = 0;
    public State curState = State.Idle;
    private HPHandler.ressourceStack delivery = null;
    private GameObject target = null;
    private GameObject lastTarget = null;
    private GameObject deliverTarget = null;
    private GameObject deliverFrom = null;
    private DeliveryRoutes.route route = null;
    private bool stopDelivering = false;

    public enum State {Idle, Walking, Attacking, ConstructionDelivering, RouteDelivering};

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (curState == State.ConstructionDelivering && (this.GetComponent<movementController>().agent.isStopped)) {
            this.stop();
            this.setState(State.Idle);
        }

        if (curState == State.ConstructionDelivering && this.GetComponent<movementController>().agent.velocity.magnitude <= 0.1f) {
            idleTimer++;
            if (idleTimer > 20) {
                stop();
                this.curState = State.Idle;
                clearDelivery();
            }
        } else if (curState == State.ConstructionDelivering) {
            idleTimer = 0;
        }

        if (curState == State.ConstructionDelivering || curState == State.RouteDelivering) {
            return;
        }

        if (curState == State.Idle && this.GetComponent<inventory>().getAmount() > 0) {
            idleDeliveryTimer++;
            if (idleDeliveryTimer > 30) {
                Debug.Log("Inventory of " + transform.gameObject.name + " is full, going to return point");
                goToBase();
                target = null;
            }
        }

        if (this.GetComponent<movementController>().agent.velocity.magnitude > 0.1f && target == null) {
            curState = State.Walking;
        } else if (target != null && isAttackTarget(target)) {
            curState = State.Attacking;
        } else {
            curState = State.Idle;
        }

        if (target == null || !isAttackTarget(target)) {
            return;
        }

		timer += Time.deltaTime;

        if (timer >= 1f / (float) APS) {
            if (!inRange()) {
                stop();
            }
            timer = 0f;
            attack();
        }
	}

    private bool isAttackTarget(GameObject obj) {
        if (obj.GetComponent<HPHandler>() == null) return false;
        if (obj.GetComponent<harvestableRessource>() != null) return true;
        return false;
    }

    private bool inRange() {

        return inRange(target.transform);
    }

    private bool inRange(Transform transform) {
        
        Vector3 closestPoint = transform.gameObject.GetComponent<Collider>().ClosestPoint(this.transform.position);
        return Vector3.Distance(closestPoint, this.transform.position) < range;
    }

    public void stop() {
        target = null;
        //Debug.Log("done attacking " + GetComponent<inventory>());

    }

    private void clearDelivery() {
        this.lastTarget = null;
        this.deliverFrom = null;
        this.deliverTarget = null;
        this.delivery = null;
    }

    private void checkFull() {
        if (GetComponent<inventory>().isFull()) {
            Debug.Log("Inventory of " + transform.gameObject.name + " is full, going to return point");
            goToBase();
            target = null;
        }
    }

    private void attack() {

        //Debug.Log("starting attack anim");
        if (target.GetComponent<HPHandler>().HP >= AD) {
            GetComponent<Animator>().SetTrigger("attack");
        }
        
    }

    private void goToBase() {
        Transform target = GetComponent<movementController>().getClosest("dropBase");
        GetComponent<movementController>().setTarget(target);
        if (inRange(target)) {
            handleTarget(target);
        }
    }

    public void handleTarget(Transform target) {

        if (target == null) {
            return;
        }

        Debug.Log("reached new target!" + GetComponent<inventory>());
        if (!inRange(target)) {
            return;
        }

        if (curState == State.RouteDelivering) {
            Debug.Log("reached routedelivery target!");
            if (target.gameObject.Equals(route.getTarget())) {
                //arrived to target to drop off ressouces
                this.GetComponent<inventory>().transferAll(route.getTarget().GetComponent<inventory>());

                if (stopDelivering) {
                    this.stopDelivering = false;
                    this.setState(State.Idle);
                    return;
                }

                this.GetComponent<movementController>().setTarget(route.getOrigin().transform);


            } else if (target.gameObject.Equals(route.getOrigin())) {
                //arrived at source to take ressources
                if (route is DeliveryRoutes.routeSolotype) {     //solotype
                    float transferAmount = inventory.getMaxTransfer(route.getOrigin(), this.gameObject, ((DeliveryRoutes.routeSolotype) route).getType());
                
                    Debug.Log("transfer Amount:" + transferAmount);
                    route.getOrigin().GetComponent<inventory>().transferTo(this.GetComponent<inventory>(), ((DeliveryRoutes.routeSolotype) route).getType(), transferAmount);
                } else {    //allType
                    route.getOrigin().GetComponent<inventory>().transferAllSafe(this.GetComponent<inventory>());
                }
                this.GetComponent<movementController>().setTarget(route.getTarget().transform);
                
            }

            return;
        }

        if (target.gameObject.Equals(deliverTarget)) {
            deliverTargetReached();
            return;
        }

        if (target.CompareTag("dropBase") && curState == State.ConstructionDelivering) {
            Debug.Log("reached dropBase for delivery");
            deliveryFromReached();
            return;
        } else if (target.CompareTag("dropBase")) {
            BaseReached(target);
            Debug.Log("reached dropbase");
            return;
        }

        if (target.gameObject.name.Contains("Recycler")) {
            Debug.Log("reached recycling platform, destroying self now");
            if (target.GetComponent<WorkerRecycler>().hasEnoughEnergy()) {
                target.GetComponent<WorkerRecycler>().workerArrived(this.gameObject);
                Destroy(this.gameObject);
            }
            return;
        }

        if (target.gameObject.name.Contains("PickupBox")) {
            print("reached Pickup, taking all content");
            target.GetComponent<inventory>().transferAll(this.GetComponent<inventory>());
            goToBase();
            GameObject.Destroy(target.gameObject);
        }

        Vector3 toTarget = target.position - transform.position;
        this.transform.rotation = Quaternion.LookRotation(toTarget);
        lastTarget = null;

        setTarget(target.gameObject);
    }

    private void deliverTargetReached() {
        this.GetComponent<inventory>().transferAll(deliverTarget.GetComponent<inventory>());
        this.curState = State.Idle;

        stop();

        try {
            deliverTarget.GetComponent<building_marker>().deliveryArrived(delivery);
        } catch (NullReferenceException ex) {
            print("delivery target doesnt have building_marker component!");
        }
        
    }

    private void deliveryFromReached() {

        if (!deliverFrom.GetComponent<inventory>().canTake(delivery)) {
            print("unable to take from target inventory (too few ressources?)");
            return;
        }

        this.deliverFrom.GetComponent<inventory>().remove(delivery);
        this.GetComponent<inventory>().add(delivery);

        GetComponent<movementController>().setTarget(deliverTarget.transform, "deliver");
        this.setTarget(deliverTarget);
    }

    private void BaseReached(Transform target) {
        GetComponent<inventory>().transferAll(target.GetComponent<inventory>());
        if (lastTarget == null) {
            stop();
            return;
        }
        GetComponent<movementController>().setTarget(lastTarget.transform);
    }

    private void setTarget(GameObject target) {
        timer = 0f;
        this.target = target;
    }

    public void setState(State state) {
        /*if (curState.Equals(State.Delivering) && !state.Equals(State.Delivering)) {
            Debug.Log("delivery cancelled, removing from ordered list");
            deliverTarget.GetComponent<building_marker>().removeOrdered(this.delivery);
        }*/
        if (curState == State.RouteDelivering && state != State.RouteDelivering) {
            stopDelivering = false;
        }
        this.curState = state;
    }

    public void Hit() {
        
        if (target == null || target.GetComponent<HPHandler>().HP <= 0) {
            lastTarget = null;
            stop();
            return;
        }

        HPHandler.ressourceStack gain = target.GetComponent<HPHandler>().inflictDamage(AD, this);
        GetComponent<inventory>().add(gain);

        lastTarget = target;

        checkFull();
    }

    public State getState() {
        return curState;
    }

    public void deliverTo(GameObject takeFrom, GameObject bringTo, HPHandler.ressourceStack stack) {
        
        print(this.name + " is delivering to: " + bringTo);

        lastTarget = null;

        if (stack.getAmount() > this.GetComponent<inventory>().maxSize) {
            stack.setAmount(this.GetComponent<inventory>().maxSize);
        }

        if (this.GetComponent<inventory>().maxSize - this.GetComponent<inventory>().getAmount() < stack.getAmount()) {
            Debug.Log("mover is idle and not enough inv space to deliver, emptying now (inv:)" + this.GetComponent<inventory>());
            goToBase();
            target = null;
            return;
        }

        curState = State.ConstructionDelivering;
        this.setTarget(takeFrom);
        deliverTarget = bringTo;
        deliverFrom = takeFrom;
        delivery = stack;
        idleTimer = 0;

        GetComponent<movementController>().setTarget(takeFrom.transform);
    }

    public GameObject getDeliverTarget() {
        return deliverTarget;
    }

    public HPHandler.ressourceStack getDelivery() {
        return this.delivery;
    }

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(idleTimer, idleDeliveryTimer, curState, delivery, target, lastTarget, deliverTarget, deliverFrom);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData) info;
        print("deserilazing Action controller...");
        this.idleTimer = data.idleTimer;
        this.idleDeliveryTimer = data.idleDeliveryTimer;
        this.curState = data.curState;
        this.delivery = data.delivery;
        this.target = atPos(data.target);
        this.lastTarget = atPos(data.lastTarget);
        this.deliverTarget = atPos(data.deliverTarget);
        this.deliverFrom = atPos(data.deliverFrom);

    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        
        public int idleTimer;
        public int idleDeliveryTimer;
        public State curState;
        public HPHandler.ressourceStack delivery;
        public SaveLoad.SerializableVector3 target;
        public SaveLoad.SerializableVector3 lastTarget;
        public SaveLoad.SerializableVector3 deliverTarget;
        public SaveLoad.SerializableVector3 deliverFrom;

        public serializationData(int idleTimer, int idleDeliveryTimer, State curState, HPHandler.ressourceStack delivery, GameObject target, GameObject lastTarget, GameObject deliverTarget, GameObject deliverFrom) {
            this.idleTimer = idleTimer;
            this.idleDeliveryTimer = idleDeliveryTimer;
            this.curState = curState;
            if (curState == State.RouteDelivering)
                this.curState = State.Idle;
            this.delivery = delivery;
            if (target != null) {
                this.target = target.transform.position;
            } else {
                this.target = Vector3.zero;
            }
            if (lastTarget != null) {
                this.lastTarget = lastTarget.transform.position;
            } else {
                this.lastTarget = Vector3.zero;
            }
            if (deliverTarget != null) {
                this.deliverTarget = deliverTarget.transform.position;
            } else {
                this.deliverTarget = Vector3.zero;
            }
            if (deliverFrom != null) {
                this.deliverFrom = deliverFrom.transform.position;
            } else {
                this.deliverFrom = Vector3.zero;
            }
        }

        
        public override string scriptTarget {
            get {
                return "ActionController";
            }
        }
    }

    private static GameObject[] objs = null;

    public static GameObject atPos(Vector3 position) {

        print("searching for GameObject at: " + position);

        if (position == Vector3.zero) {
            return null;
        }

        if (objs == null) {
            objs = (GameObject[]) FindObjectsOfType(typeof(GameObject));
        }

        GameObject myObject = null;
        foreach (GameObject go in objs) {
            if (!go.name.Contains("Clone")) {
                continue;
            }
            print("distance: " + Vector3.Distance(go.transform.position, position) + " go pos=" + go.transform.position + " name=" + go.name);
            if (Vector3.Distance(go.transform.position, position) < 0.5) {
                myObject = go;
                break;
            }
        }

        print("found: " + myObject);

        return myObject;
    }

    public void setRoute(DeliveryRoutes.route route) {
        Debug.Log("Got Route: " + route + " self: " + this.gameObject.name);
        this.setState(State.RouteDelivering);
        this.route = route;
        this.GetComponent<movementController>().setTarget(route.getTarget().transform);
    }

    public void stopDeliveryRoute() {
        this.stopDelivering = true;
    }
}
