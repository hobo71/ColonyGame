using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStructure : MonoBehaviour, Structure {

	private ressourceStack[] ownCost;
    private bool salvaging = false;
    public ressourceStack[] getCost() {
		return ownCost;
    }

	public void setCost(ressourceStack[] cost) {
		ownCost = cost;
	}

    public GameObject getGameobject() {
        return this.gameObject;
    }
	

    public ressourceStack[] getResources() {
        return new ressourceStack[] { new ressourceStack(1000, ressources.Scrap) };
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
            pickup.GetComponent<inventory>().add(new ressourceStack(this.getHP().getInitialHP(), ressources.Scrap));
            GameObject.Destroy(this.gameObject);
        }

        if (salvaging) {
            this.getHP().HP -= 2.5f;
            return;
        }
    }
}
