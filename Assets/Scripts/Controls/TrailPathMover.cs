using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrailPathMover : MonoBehaviour {
    
    Vector3 target;
    Vector3 start;

    void Start () {
        start = this.transform.position;
    }

    void FixedUpdate () {
        
        float totalDistance = Vector3.Distance(start, target);
        float done = Vector3.Distance(transform.position, start) / totalDistance;

        if (done >= 0.95f) {
            return;
        }

        Vector3 moveBy = target - start;
        //moveBy.Normalize();
        moveBy *= Time.deltaTime * 2;
        
        moveBy.y += getY(done) * 0.5f;
        //moveBy = transform.rotation * moveBy;

        this.transform.Translate(moveBy, Space.World);

    }

    private float getY(float x) {
        //return (4f * x) - (4.0f * (x * x));
        return -1.9f * x + 1;
    }
    /*NavMeshAgent agent;
    Vector3 target;

	// Use this for initialization
	void Start () {
		Debug.Log("created trail emitter");
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {

        if (target == null || Vector3.Distance(transform.position, target) < 3.0f) {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;
        
        agent.SetDestination(target);
	}*/

    public void setPath(Vector3 target) {
        this.target = target;
    }

    public void setColor(string mode) {
        TrailRenderer renderer = this.GetComponent<TrailRenderer>();

        if (mode.Equals("deliver")) {
            Debug.Log("setting delivery color!");
            Gradient grad = renderer.colorGradient;
            GradientColorKey[] keys = grad.colorKeys;
            Debug.Log("found keys: " + keys.Length);
            keys[0].color = new Color(1.0f, 72f / 255f, 4f / 255f);
            keys[1].color = new Color(1.0f, 72f / 255f, 4f / 255f);
            grad.SetKeys(keys, grad.alphaKeys);
            renderer.colorGradient = grad;
        }

        if (mode.Equals("salvage")) {
            Debug.Log("setting salvage color!");
            Gradient grad = renderer.colorGradient;
            GradientColorKey[] keys = grad.colorKeys;
            Debug.Log("found keys: " + keys.Length);
            keys[0].color = new Color(1.0f, 100 / 255f, 100f / 255f);
            keys[1].color = new Color(1.0f, 150f / 255f, 4f / 255f);
            grad.SetKeys(keys, grad.alphaKeys);
            renderer.colorGradient = grad;
        }
    }
}
