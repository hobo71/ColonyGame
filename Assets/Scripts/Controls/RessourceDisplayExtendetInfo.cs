using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourceDisplayExtendetInfo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void toggled() {
        ResourceDisplay.toggleElem(this.gameObject.transform.GetChild(2).GetComponent<Text>().text);
    }
}
