using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour {
    
    public float[] totalRessources = new float[System.Enum.GetValues(typeof(HPHandler.ressources)).Length];
    
    private static ResourceHandler instance;

    private List<inventory> inventories = new List<inventory>();
	// Use this for initialization
	void Start () {
        instance = this;
		InvokeRepeating("reloadInvs", 1.0f, 5.0f);
        print("totalRessources size: " + totalRessources.Length + " origin: " + System.Enum.GetValues(typeof(HPHandler.ressources)).Length);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {
        //float curWood = 0.0f;
        //float curStone = 0.0f;

        float[] curRessources = new float[totalRessources.Length];
        
        foreach (inventory inv in inventories) {
            int i = 0;
            foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
                curRessources[i] += inv.getAmount(res);
                i++;
            }
            //curWood += inv.getAmount(HPHandler.ressources.Wood);
            //curStone += inv.getAmount(HPHandler.ressources.Stone);
        }

        //totalWood = curWood;
        //totalStone = curStone;
        //totalRessources = curRessources;
    }

    void reloadInvs() {
        inventories.Clear();
        foreach(GameObject elem in GameObject.FindGameObjectsWithTag("dropBase")) {
            inventories.Add(elem.GetComponent<inventory>());
        }
        /*foreach(GameObject elem in GameObject.FindGameObjectsWithTag("mover")) {
            inventories.Add(elem.GetComponent<inventory>());
        }*/
        //Debug.Log("reloaded inventories, " + this.ToString());
    }

    public override string ToString() {
        string content = "";

        print("ressources count: " + System.Enum.GetValues(typeof(HPHandler.ressources)).Length + " totalRessources size: " + totalRessources.Length);

        int i = 0;
        foreach (HPHandler.ressources res in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            content += res.ToString() + ": " + totalRessources[i] + ", ";
            i++;
        }
        return "Total items: " + content;
    }

    public static ResourceHandler getInstance() {
        return instance;
    }

    public static float getAmoumt(HPHandler.ressources res) {
        print("data: " + ResourceHandler.getInstance().ToString());
        int i = 0;
        foreach (HPHandler.ressources cur in System.Enum.GetValues(typeof(HPHandler.ressources))) {
            if (res.Equals(cur)) {
                return getInstance().totalRessources[i];
            }
            i++;
        }
        return 0;
    }
}
