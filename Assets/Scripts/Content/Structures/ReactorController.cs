﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReactorController : MonoBehaviour, clickable, Structure {

    public Sprite infoBut;
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

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption buildReactor = new PopUpCanvas.popUpOption("BuildReactor", buildCoreBut);
        PopUpCanvas.popUpOption buildBoiler = new PopUpCanvas.popUpOption("BuildBoiler", buildBoilerBut);
        PopUpCanvas.popUpOption buildWall = new PopUpCanvas.popUpOption("BuildWall", buildWallBut);

        options = new PopUpCanvas.popUpOption[] { info, buildReactor, buildBoiler, buildWall};
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
                data = new BuildingManager.structureData(corePrefab, corePlacement, null, "reactorcore", "reactorcore", new List<HPHandler.ressourceStack> {
                    new HPHandler.ressourceStack(250, HPHandler.ressources.Stone),
                    new HPHandler.ressourceStack(50, HPHandler.ressources.Wood),
                    new HPHandler.ressourceStack(20, HPHandler.ressources.Gold),
                    new HPHandler.ressourceStack(200, HPHandler.ressources.Iron)});
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildBoiler":
                data = new BuildingManager.structureData(boilerPrefab, boilerPlacement, null, "reactorboiler", "reactorboiler", new List<HPHandler.ressourceStack> {
                    new HPHandler.ressourceStack(200, HPHandler.ressources.Stone),
                    new HPHandler.ressourceStack(200, HPHandler.ressources.Wood),
                    new HPHandler.ressourceStack(500, HPHandler.ressources.Iron)});
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildWall":
                data = new BuildingManager.structureData(wallPrefab, wallPlacement, null, "reactorwall", "reactorwall", new List<HPHandler.ressourceStack> {
                    new HPHandler.ressourceStack(100, HPHandler.ressources.Stone),
                    new HPHandler.ressourceStack(50, HPHandler.ressources.Iron)});
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            default:
                break;
        }

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
