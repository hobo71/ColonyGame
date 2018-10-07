using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUVOffset : MonoBehaviour {

    private Material ribbonMat;
    private ParticleSystem ps;
    public float totalTime = 150f;
    private float timeDone = 0;

	// Use this for initialization
	void Start () {
		ribbonMat = GetComponent<ParticleSystemRenderer>().materials[1];
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		ribbonMat.mainTextureOffset -= new Vector2(0.008f - 0.004f * (timeDone / totalTime), 0f);
	}

    void FixedUpdate() {
        timeDone += 1;
        var main = ps.main;
        var trail = ps.trails;

        if (ps.main.startLifetime.constant != totalTime) {
            main.startLifetime = totalTime;
        }

        trail.lifetime = 0.01f * (timeDone / totalTime) + 0.002f;

    }
}
