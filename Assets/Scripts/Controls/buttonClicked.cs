using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonClicked : MonoBehaviour {

	public void onClick() {
        this.transform.parent.GetComponent<PopUpCanvas>().handleClick(this.gameObject.name);
    }
    
    public void onSalvage() {
        Salvaging.salvageTriggered();
    }
}
