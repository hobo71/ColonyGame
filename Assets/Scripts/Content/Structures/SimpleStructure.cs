using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStructure : MonoBehaviour, Structure {

	private HPHandler.ressourceStack[] ownCost;
    public HPHandler.ressourceStack[] getCost() {
		return ownCost;
    }

	public void setCost(HPHandler.ressourceStack[] cost) {
		ownCost = cost;
	}

    public GameObject getGameobject() {
        return this.gameObject;
    }
	

    public HPHandler.ressourceStack[] getResources() {
        return new HPHandler.ressourceStack[] { new HPHandler.ressourceStack(1000, HPHandler.ressources.Scrap) };
    }

    public HPHandler getHP() {
        return this.GetComponent<HPHandler>();
    }

    public inventory getInv() {
        return this.GetComponent<inventory>();
    }

    public bool isSalvaging() {
        return false;
    }

    public bool isWorking() {
		return true;
    }

    public void salvage() {
		//do Nothing
    }
}
