using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStructure : MonoBehaviour, Structure {

	private HPHandler.ressourceStack[] ownCost;
    private bool salvaging = false;
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
        return salvaging;
    }

    public bool isWorking() {
		return true;
    }

    public void salvage() {
        this.salvaging = true;
        Salvaging.displayIndicator(this.gameObject);
    }
    
    void FixedUpdate() {
        if (this.salvaging && this.getHP().HP < 3) {
            print("structure salvaged!");
            var pickup = Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().pickupBox, this.transform.position, Quaternion.identity);
            pickup.GetComponent<inventory>().add(new HPHandler.ressourceStack(this.getHP().getInitialHP(), HPHandler.ressources.Scrap));
            GameObject.Destroy(this.gameObject);
        }

        if (salvaging) {
            this.getHP().HP -= 2.5f;
            return;
        }
    }
}
