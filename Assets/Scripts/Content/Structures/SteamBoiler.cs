using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SteamBoiler : MonoBehaviour, clickable, Structure {

    public Sprite infoBut;
    public Sprite buildCoolBut;
    public GameObject coolPrefab;
    public GameObject coolPlacement;
    public Sprite buildHeatBut;
    public GameObject heatPrefab;
    public GameObject heatPlacement;
    bool salvaging = false;

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();

        controller.setTitle(this.gameObject.name);
        controller.setDesc(getDesc());
    }

    private string getDesc() {
        return "The cooling controller and steam generate, typically used with a reactor"
            + Environment.NewLine
            + this.GetComponent<inventory>().ToString();

    }

    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[4];

        cost[0] = new HPHandler.ressourceStack(300, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(50, HPHandler.ressources.Stone);
        cost[2] = new HPHandler.ressourceStack(300, HPHandler.ressources.Iron);
        cost[3] = new HPHandler.ressourceStack(150, HPHandler.ressources.Gold);
        return cost;
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption buildHeat = new PopUpCanvas.popUpOption("BuildHeatReflector", buildHeatBut);
        PopUpCanvas.popUpOption buildCooling = new PopUpCanvas.popUpOption("BuildCoolingGrid", buildCoolBut);

        options = new PopUpCanvas.popUpOption[] { info, buildHeat, buildCooling};
        return options;
    }

    public void handleClick() {
        Debug.Log("clicked steam boiler");

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
        BuildingManager.structureData data;

        switch (option) {
            case "Info":
                displayInfo();
                break;
            case "BuildHeatReflector":
                data = new BuildingManager.structureData(heatPrefab, heatPlacement, null, "reactorHeat", "reactorHeat", new List<HPHandler.ressourceStack> {
                    new HPHandler.ressourceStack(250, HPHandler.ressources.Stone),
                    new HPHandler.ressourceStack(15, HPHandler.ressources.Gold),},
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildCoolingGrid":
                data = new BuildingManager.structureData(coolPrefab, coolPlacement, null, "reactorCool", "reactorCool", new List<HPHandler.ressourceStack> {
                    new HPHandler.ressourceStack(250, HPHandler.ressources.Stone),
                    new HPHandler.ressourceStack(300, HPHandler.ressources.Iron)},
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            default:
                break;
        }

    }

    private bool isNearReactor(GameObject holoPlacement, bool terrainCheck) {

        if (!terrainCheck) {
            return false;
        }

        var parts = GameObject.FindGameObjectsWithTag("reactorPart");
        foreach (var item in parts) {
            //check if an object with the "reactorPart" tag is within x meters of the placement structure
            if (Vector3.Distance(item.transform.position, holoPlacement.transform.position) < 8) {
                return true;
            }
        }

        return false;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

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
}
