using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReactorController : MonoBehaviour, clickable, Structure {

    public Sprite infoBut;
    bool salvaging = false;

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();

        controller.setTitle(this.gameObject.name);
        controller.setDesc(getDesc());
    }

    private string getDesc() {
        return "The Central control piece of the Nuclear Reactor. Uses uranium to turn water into steam"
            + Environment.NewLine
            + this.GetComponent<inventory>().ToString();

    }

    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[4];

        cost[0] = new HPHandler.ressourceStack(200, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(200, HPHandler.ressources.Stone);
        cost[2] = new HPHandler.ressourceStack(500, HPHandler.ressources.Iron);
        cost[3] = new HPHandler.ressourceStack(100, HPHandler.ressources.Gold);
        return cost;
    }

    public PopUpCanvas.popUpOption[] getOptions() {
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
        
        options = new PopUpCanvas.popUpOption[]{info};
        return options;
    }

    public void handleClick() {
        Debug.Log("clicked dome");

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }

        if (salvaging) {
            return;
        }
    }
    public void salvage() {
        Debug.Log("Got salvage request!");
        this.salvaging = true;
        Salvaging.displayIndicator(this.gameObject);
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
        return;
    }

    public void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            case "Info":
                displayInfo();
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
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

    public bool isWorking() {
        return !salvaging;
    }

    public GameObject getGameobject() {
        return this.gameObject;
    }

    public HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }

    public HPHandler.ressourceStack[] getResources() {
        return new HPHandler.ressourceStack[]{new HPHandler.ressourceStack(1000, HPHandler.ressources.Scrap)};
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
}
