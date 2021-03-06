﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SteamBoiler : DefaultStructure {
    public Sprite buildCoolBut;
    public GameObject coolPrefab;
    public GameObject coolPlacement;
    public Sprite buildHeatBut;
    public GameObject heatPrefab;
    public GameObject heatPlacement;
    public float steamAmount = 0f;
    private float energy = 0f;

    public override string getDesc() {
        return "The cooling controller and steam generate, typically used with a reactor"
               + Environment.NewLine
               + this.GetComponent<inventory>().ToString();
    }

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[4];

        cost[0] = new ressourceStack(300, ressources.Wood);
        cost[1] = new ressourceStack(50, ressources.Stone);
        cost[2] = new ressourceStack(300, ressources.Iron);
        cost[3] = new ressourceStack(150, ressources.Gold);
        return cost;
    }

    public override PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption buildHeat = new PopUpCanvas.popUpOption("BuildHeatReflector", buildHeatBut);
        PopUpCanvas.popUpOption buildCooling = new PopUpCanvas.popUpOption("BuildCoolingGrid", buildCoolBut);

        options = new PopUpCanvas.popUpOption[] {info, buildHeat, buildCooling};
        return options;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);
        BuildingManager.structureData data;

        switch (option) {
            case "Info":
                displayInfo();
                break;
            case "BuildHeatReflector":
                data = new BuildingManager.structureData(heatPrefab, heatPlacement, null, "reactorHeat", "reactorHeat",
                    new List<ressourceStack> {
                        new ressourceStack(250, ressources.Stone),
                        new ressourceStack(15, ressources.Gold),
                    },
                    isNearReactor);
                GameObject.Find("Terrain").GetComponent<Building>().buildClicked(data);
                GameObject.Find("Terrain").GetComponent<Building>().setOverrideCost(data.cost.ToArray());
                break;
            case "BuildCoolingGrid":
                data = new BuildingManager.structureData(coolPrefab, coolPlacement, null, "reactorCool", "reactorCool",
                    new List<ressourceStack> {
                        new ressourceStack(250, ressources.Stone),
                        new ressourceStack(300, ressources.Iron)
                    },
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

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    new void FixedUpdate() {
        base.FixedUpdate();
        if (this.salvaging && this.getHP().HP < 3) {
            print("structure salvaged!");
            var pickup = Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().pickupBox,
                this.transform.position, Quaternion.identity);
            pickup.GetComponent<inventory>().add(new ressourceStack(this.getHP().getInitialHP(), ressources.Scrap));
            GameObject.Destroy(this.gameObject);
        }

        if (salvaging) {
            this.getHP().HP -= 2.5f;
            return;
        }

        if (steamAmount > 10) {
            this.addEnergy(10 * Time.deltaTime, this);
            steamAmount -= 10 * Time.deltaTime;
            this.getInv().add(ressources.Water, -10f * Time.deltaTime);
        }

        if ((this.getInv().getAmount(ressources.Water) < 50 || this.steamAmount > 10) && counter % 60 == 0 &&
            !this.GetComponent<inventory>().isFull()) {
            RessourceHelper.deliverTo(this.gameObject, false, ressources.Water);
        }
        
        if (counter % 5 == 0) {
            handleConnections();
        }
        
    }
    
    
    public new void handleConnections() {

        if (network == null) {
            return;
        }

        float minEnergy = float.MaxValue;
        EnergyContainer target = null;

        foreach (EnergyContainer connection in base.network) {
            if (connection.getMaxInput() < 1) {
                continue;
            }

            if (connection.getCurEnergy() < minEnergy) {
                target = connection;
                minEnergy = connection.getCurEnergy();
            }
        }
        
        if (target == null) {
            return;
        }

        float transferAmount = (target.getMaxInput() > this.getMaxOutput()) ? target.getMaxInput() : this.getMaxOutput();
        if (target.getMaxInput() < this.getMaxOutput()) {
            transferAmount = target.getMaxInput();
        }
        if (transferAmount > this.getCurEnergy()) {
            transferAmount = this.getCurEnergy();
        }

        target.addEnergy(transferAmount, this);
        this.addEnergy(-transferAmount, this);
    }

    public bool addSteam(float amount) {
        if (this.getInv().getAmount(ressources.Water) < 10f * Time.deltaTime) {
            return false;
        }
        else {
            this.steamAmount += amount;
            return true;
        }
    }

    public override int getMaxEnergy() {
        return 3000;
    }

    public override int getMaxOutput() {
        return 50;
    }

    public override int getMaxInput() {
        return 0;
    }

    public override int getPriority() {
        return 5;
    }

    public override ressourceStack[] getCost() {
        return getPrice();
    }
}