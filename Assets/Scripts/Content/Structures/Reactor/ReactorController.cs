using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReactorController : MonoBehaviour, clickable, Structure {

    public Sprite infoBut;
    public Sprite activateBut;
    public Sprite deactivateBut;
    public Sprite buildCoreBut;
    public GameObject corePrefab;
    public GameObject corePlacement;
    public Sprite buildBoilerBut;
    public GameObject boilerPrefab;
    public GameObject boilerPlacement;
    public Sprite buildWallBut;
    public GameObject wallPrefab;
    public GameObject wallPlacement;
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

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[4];

        cost[0] = new ressourceStack(200, ressources.Wood);
        cost[1] = new ressourceStack(200, ressources.Stone);
        cost[2] = new ressourceStack(500, ressources.Iron);
        cost[3] = new ressourceStack(100, ressources.Gold);
        return cost;
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption buildReactor = new PopUpCanvas.popUpOption("BuildReactor", buildCoreBut);
        PopUpCanvas.popUpOption buildBoiler = new PopUpCanvas.popUpOption("BuildBoiler", buildBoilerBut);
        PopUpCanvas.popUpOption buildWall = new PopUpCanvas.popUpOption("BuildWall", buildWallBut);
        PopUpCanvas.popUpOption activate = new PopUpCanvas.popUpOption("activate", activateBut);
        PopUpCanvas.popUpOption deactivate = new PopUpCanvas.popUpOption("deactivate", deactivateBut);

        options = new PopUpCanvas.popUpOption[] { info, activate, deactivate, buildReactor, buildBoiler, buildWall};
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
        BuildingManager.structureData data;

        switch (option) {
            case "Info":
                displayInfo();
                break;
            case "BuildReactor":
                data = new BuildingManager.structureData(corePrefab, corePlacement, null, "reactorcore", "reactorcore", new List<ressourceStack> {
                    new ressourceStack(250, ressources.Stone),
                    new ressourceStack(50, ressources.Wood),
                    new ressourceStack(20, ressources.Gold),
                    new ressourceStack(200, ressources.Iron)},
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildBoiler":
                data = new BuildingManager.structureData(boilerPrefab, boilerPlacement, null, "reactorboiler", "reactorboiler", new List<ressourceStack> {
                    new ressourceStack(200, ressources.Stone),
                    new ressourceStack(200, ressources.Wood),
                    new ressourceStack(500, ressources.Iron)},
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildWall":
                data = new BuildingManager.structureData(wallPrefab, wallPlacement, null, "reactorwall", "reactorwall", new List<ressourceStack> {
                    new ressourceStack(100, ressources.Stone),
                    new ressourceStack(50, ressources.Iron)},
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "activate":
                this.gameObject.GetComponent<ReactorLogic>().activate();
                break;
            case "deactivate":
                this.gameObject.GetComponent<ReactorLogic>().deactivate();
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
            pickup.GetComponent<inventory>().add(new ressourceStack(this.getHP().getInitialHP(), ressources.Scrap));
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

    public ressourceStack[] getCost() {
        return getPrice();
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
}
