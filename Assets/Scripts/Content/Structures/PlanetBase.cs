using UnityEngine;

public class PlanetBase : MonoBehaviour, Structure {
    
    private ressourceStack[] ownResource = new ressourceStack[2];
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

    public ressourceStack[] getCost() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(100, ressources.Wood);
        cost[1] = new ressourceStack(50, ressources.Stone);

        return cost;
    }

    public ressourceStack[] getResources() {
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
