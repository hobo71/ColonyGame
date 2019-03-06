using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class movementController : MonoBehaviour, SaveLoad.SerializableInfo {
    
    private Vector3 target = Vector3.zero;
    private Transform targetObject;
    public GameObject trailEmitter;
    public NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}

    void Update() {

        GetComponent<Animator>().SetFloat("moveSpeed", agent.velocity.magnitude);

        if (target == Vector3.zero) {
            return;
        }

        agent.SetDestination(target);
        
        if (agent.remainingDistance < 0.5f && agent.hasPath) {
            target = Vector3.zero;
            agent.isStopped = true;
            Debug.Log("stopping navmesh agent!");

            GetComponent<ActionController>().handleTarget(targetObject);
        }
    }

    public void moveRand() {
        Vector3 goTo = this.transform.forward * UnityEngine.Random.Range(1, 5) + this.transform.right * UnityEngine.Random.Range(-3f, 3f);
        moveTo(goTo + this.transform.position, false);
    }

    public void moveTo(Vector3 target, bool displayTarget = true) {

        if (agent == null) {
            Start();
        }
        
        this.target = target;
        agent.SetDestination(this.target);
        if (displayTarget) {
            displayPath(this.target, "move");
        }
        agent.isStopped = false;

        GetComponent<ActionController>().stop();

        Debug.Log("setting target for movement controller: " + this.name);
        if (this.target == Vector3.zero) {
            Debug.Log("Error while setting target");
        }
    }

    public void setTarget(Transform target) {
        this.setTarget(target, "move");
    }

    public void setTarget(Transform target, string mode) {
        this.targetObject = target;

        Vector3 closestPoint = target.gameObject.GetComponent<Collider>().ClosestPoint(this.transform.position);

        if (Vector3.Distance(closestPoint, this.transform.position) <= 0.5f) {
            Debug.Log("navmesh agent is already very close to target");
            this.target = Vector3.zero;
            agent.isStopped = true;

            GetComponent<ActionController>().handleTarget(targetObject);
            return;
        }

        //closestPoint.y = target.transform.position.y;

        /*NavMeshHit hit;
        bool blocked = NavMesh.SamplePosition(closestPoint, out hit, 20.0f, NavMesh.AllAreas);

        if (blocked) {
            this.target = hit.position;
        } else {
            this.target = closestPoint;
        }*/
        this.target = closestPoint;
        agent.SetDestination(this.target);
        displayPath(this.target, mode);
        agent.isStopped = false;

        GetComponent<ActionController>().stop();

        Debug.Log("setting target for movement controller: " + target.name);
        if (this.target == Vector3.zero) {
            Debug.Log("Error while setting target");
        }
    }

    private void displayPath(Vector3 target, string mode) {

        if (Vector3.Distance(transform.position, target) < 1.0f) {
            return;
        }

        Vector3 spawnAt = transform.position;
        spawnAt.y += 1f;
        Vector3 targetAt = target;
        //targetAt.y += 2f;

        GameObject emitter = Instantiate(trailEmitter, spawnAt, transform.rotation);
        emitter.GetComponent<TrailPathMover>().setPath(targetAt);
        emitter.GetComponent<TrailPathMover>().setColor(mode);
    }

    public Transform getClosest(string tag) {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

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
    
    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(target, targetObject);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        print("got deserialization for: " + info.scriptTarget);

        serializationData data = (serializationData) info;
        print("deserilazing movement controller...");
        this.target = data.target;
        try {
            this.targetObject = atPos(data.targetObject).transform;
        } catch (NullReferenceException ex) {
            this.targetObject = null;
        }
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        public SaveLoad.SerializableVector3 target;
        public SaveLoad.SerializableVector3 targetObject;

        public serializationData(Vector3 target, Transform targetObject) {
            this.target = target;

            try {
                this.targetObject = targetObject.position;
            } catch (Exception ex) {
                this.targetObject = Vector3.zero;
            }
        }

        public override string scriptTarget {
            get {
                return "movementController";
            }
        }
    }
    private GameObject atPos(Vector3 position) {
        return ActionController.atPos(position);
    }



    /*public float speed = 6.0F;
    public float jumpSpeed = 16.0F;
    public float gravity = 20.0F;
    //public float rotateSpeed = 3.0F;
    private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        CharacterController controller = GetComponent<CharacterController>();
        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("Vertical");
        controller.SimpleMove(forward * curSpeed);

		CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
            
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
	}*/
}
