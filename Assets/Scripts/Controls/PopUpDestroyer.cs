using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDestroyer : MonoBehaviour {

    private bool destroying = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (destroying) {
            this.transform.localScale *= 0.75f;
            if (this.transform.localScale.magnitude < 0.1f) {
                GameObject.Destroy(this.gameObject);
                ClickOptions.UIOpen = false;
            }
        }
	}

    public void destroy() {
        destroying = true;
    }
}
