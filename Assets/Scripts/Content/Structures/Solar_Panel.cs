using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solar_Panel : DefaultStructure {
    
    public bool generating = true;
    public float generateAmount = 10;   //per second

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(20, ressources.Wood);
        cost[1] = new ressourceStack(200, ressources.Stone);

        return cost;
    }

    public override ressourceStack[] getCost() {
        return Solar_Panel.getPrice();
    }

    public override string getDesc() {
        return "A basic Solar Panel. Generates 10 Energy per second"
            +  Environment.NewLine
            + "Kind: " + this.GetComponent<HPHandler>().niceText();

    }

    public override int getMaxEnergy() {
        return 500;
    }

    public override int getMaxInput() {
        return 0;
    }

    public override int getMaxOutput() {
        return 50;
    }

    public override int getPriority() {
        return 0;
    }

    public new void FixedUpdate() {
        base.FixedUpdate();

        if (counter % 5 == 0) {
            handleConnections();
        }

        if (generating && this.getCurEnergy() < this.getMaxEnergy()) {
            this.storedEnergy += Time.deltaTime * generateAmount * SunLightRotation.getIntensity();
            if (this.storedEnergy > this.getMaxEnergy()) {
                this.storedEnergy = this.getMaxEnergy();
            }
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

    public override PopUpCanvas.popUpOption[] getOptions() {
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
         //PopUpCanvas.popUpOption goTo = new  PopUpCanvas.popUpOption("DoClone", cloneBut);
        
        options = new PopUpCanvas.popUpOption[]{info};
        return options;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        switch (option) {
            default:
                displayInfo();
                break;
        }
    }

    /*private HPHandler.ressourceStack[] ownResource = new HPHandler.ressourceStack[2];
    private List<EnergyContainer> connections = new List<EnergyContainer>();
    private bool built = false;
    public float storedEnergy = 0;
    private int counter = 0;


    // Use this for initialization
    void Start () {
        reloadConnections();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

        counter++;
        if (counter >= 500) {
            reloadConnections();
            counter = 0;
        }

        if (generating && this.getCurEnergy() < this.getMaxEnergy()) {
            this.storedEnergy += Time.deltaTime * generateAmount;
            if (this.storedEnergy > this.getMaxEnergy()) {
                this.storedEnergy = this.getMaxEnergy();
            }
        }

        if (counter % 10 == 0) {
            handleConnections();
        }
    }

    public bool isWorking() {
        return true;
    }

    public HPHandler.ressourceStack[] getCost() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(20, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(200, HPHandler.ressources.Stone);

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

    public int getMaxEnergy() {
        return 500;
    }

    public int getCurEnergy() {
        return (int) storedEnergy;
    }

    public int getMaxOutput() {
        return 50;
    }

    public int getMaxInput() {
        return 0;
    }

    public GameObject GetGameObject() {
        return this.gameObject;
    }

    public List<EnergyContainer> getConnections() {
        return connections;
    }

    public void reloadConnections() {
        Debug.Log("reloading Connections for: " + this.gameObject);

        List<GameObject> available = Piping.getInRange(20.0f, this.transform);
        //Debug.Log("available: " + available);
        foreach(GameObject energyTarget in available) {
            if (connections.Contains(energyTarget.GetComponent<EnergyContainer>()) || inNetwork(energyTarget.GetComponent<EnergyContainer>())) {
                continue;
            } else {
                connections.Add(energyTarget.GetComponent<EnergyContainer>());
                energyTarget.GetComponent<EnergyContainer>().addConnection(this);
                Piping.createPipes(this.transform, energyTarget.transform);
            }
        }

    }

    private bool inNetwork(EnergyContainer container) {

        foreach (EnergyContainer connected in connections) {
            if (connected.getConnections().Contains(container)) {
                return true;
            }
        }

        return false;
    }

    public int getPriority() {
        return 0;
    }

    public void handleConnections() {
        foreach (EnergyContainer connection in connections) {
            if (connection.getMaxInput() < 1) {
                continue;
            }
            if (connection.getCurEnergy() < this.getCurEnergy() || connection.getPriority() > this.getPriority()) {
                float transferAmount = (connection.getMaxInput() > this.getMaxOutput()) ? connection.getMaxInput() : this.getMaxOutput();
                if (transferAmount > this.getCurEnergy()) {
                    transferAmount = this.getCurEnergy();
                }

                connection.addEnergy(transferAmount);
                this.addEnergy(-transferAmount);
            }
        }
    }

    public void addConnection(EnergyContainer container) {
        this.connections.Add(container);
    }

    public void addEnergy(float amount) {
        this.storedEnergy += amount;
    }*/
}
