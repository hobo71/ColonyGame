using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHelper : MonoBehaviour {

    public GameObject salvageParticles;

    private static ParticleHelper instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}

    public static ParticleHelper getInstance() {
        return instance;
    }
}
