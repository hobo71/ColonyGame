using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour {

    public GameObject display;

    private float frames = 0;
    private float lastSec = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - lastSec > 0.25) {
            display.GetComponent<UnityEngine.UI.Text>().text = (frames * 4).ToString();
            lastSec = Time.time;
            frames = 0;
        }
        frames++;
		
	}
}
