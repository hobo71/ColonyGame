using UnityEngine;

public class PlanetBase : MonoBehaviour, Structure {
    
    private HPHandler.ressourceStack[] ownResource = new HPHandler.ressourceStack[2];
    private bool built = false;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

    }

    public bool isWorking() {
        return true;
    }

    public HPHandler.ressourceStack[] getCost() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(100, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(50, HPHandler.ressources.Stone);

        return cost;
    }

    public HPHandler.ressourceStack[] getResources() {
        return this.ownResource;
    }

    public HPHandler getHP() {
        return this.GetComponent<HPHandler>();
    }

    public inventory getInv() {
        return this.GetComponent<inventory>();
    }
    
    public GameObject getGameobject() {
        return this.gameObject;
    }

    public void salvage() {
        throw new System.NotImplementedException();
    }

    public bool isSalvaging() {
        return false;
    }
}
