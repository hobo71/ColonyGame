using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dome_Basic : DefaultStructure {
    public Sprite cloneBut;
    public GameObject toClone;

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(50, ressources.Wood);
        cost[1] = new ressourceStack(50, ressources.Stone);

        return cost;
    }

    public override ressourceStack[] getCost() {
        return Dome_Basic.getPrice();
    }

    public override PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption goTo = new PopUpCanvas.popUpOption("DoClone", cloneBut);

        if (this.getCurEnergy() < 500 || this.busy) {
            goTo.setEnabled(false);
        }

        options = new PopUpCanvas.popUpOption[] {info, goTo};
        return options;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        if (salvaging) {
            return;
        }

        switch (option) {
            case "DoClone":
                Debug.Log("Clicked clone button");
                doClone();
                Notification.createNotification(this.gameObject, Notification.sprites.Starting, "", Color.green);
                break;
            default:
                displayInfo();
                break;
        }
    }

    private float lastClick;

    public override void handleClick() {
        Debug.Log("clicked dome");

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }

        if (salvaging) {
            return;
        }

        if (Time.time - lastClick < 0.2f) {
            return;
        }

        lastClick = Time.time;

        if (this.getCurEnergy() >= 500 && !this.busy) {
            doClone();
            Notification.createNotification(this.gameObject, Notification.sprites.Starting,
                this.getCurEnergy() + 500 + "/500", Color.green);
        }
        else if (this.busy) {
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "", Color.blue, true);
        }
        else {
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low,
                this.getCurEnergy() + "/500", Color.red);
        }
    }

    public override string getDesc() {
        return "A small, very basic Dome" + base.getDesc();
    }

    public override int getMaxEnergy() {
        return 2000;
    }

    public override int getMaxInput() {
        if (this.busy) {
            return 0;
        }

        if (this.getMaxEnergy() - this.getCurEnergy() < 50) {
            return this.getMaxEnergy() - this.getCurEnergy();
        }

        return 50;
    }

    public override int getMaxOutput() {
        return 40;
    }

    public override int getPriority() {
        return 5;
    }

    private void doClone() {
        this.addEnergy(-500, this);
        this.busy = true;
        Debug.Log("Beginning to clone thing...");
        this.Invoke("cloneDone", 10);
    }

    private void cloneDone() {
        Debug.Log("Cloned succesfully!");
        this.busy = false;
        GameObject cloned = GameObject.Instantiate(toClone, this.transform.position, this.transform.rotation);
        cloned.GetComponent<movementController>().moveRand();

        Notification.createNotification(this.gameObject, Notification.sprites.Starting, "", Color.green, true);
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
            counter = 0;
            //reloadConnections();
        }

        if (counter % 20 == 0) {
            handleConnections();
        }
        
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

    public int getMaxEnergy() {
        return 2000;
    }

    public int getCurEnergy() {
        return (int) storedEnergy;
    }

    public int getMaxOutput() {
        return 50;
    }

    public int getMaxInput() {
        if (this.getMaxEnergy() - this.getCurEnergy() < 50) {
            return this.getMaxEnergy() - this.getCurEnergy();
        }
        return 50;
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
        sortListByPriority(available);

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

    private void sortListByPriority(List<GameObject> list) {
        for (int i = 0; i < list.Count; i++) {

            bool changed = false;
            for (int j = i; j < list.Count - 1; j++) {
                EnergyContainer contA = list[j].GetComponent<EnergyContainer>();
                EnergyContainer contB = list[j + 1].GetComponent<EnergyContainer>();
                if (contA == null || contB == null) {
                    continue;
                }

                if (contA.getPriority() < contB.getPriority()) {
                    changed = true;
                    list.Remove(list[j + 1]);
                    list.Insert(j, contB.GetGameObject());
                    Debug.Log("changed list position of " + contA.GetGameObject().name + " and " + contB.GetGameObject().name);
                } else if (contA.getPriority() == contB.getPriority()) {
                    if (Vector3.Distance(contB.GetGameObject().transform.position, this.transform.position) < Vector3.Distance(contA.GetGameObject().transform.position, this.transform.position)) {
                        changed = true;
                        list.Remove(list[j + 1]);
                        list.Insert(j, contB.GetGameObject());
                        Debug.Log("changed list position of " + contA.GetGameObject().name + " and " + contB.GetGameObject().name);
                    }
                }
            }

            if (!changed) {
                break;
            }
        }
    }

    private bool inNetwork(EnergyContainer container) {

        foreach (EnergyContainer connected in connections) {
            foreach (EnergyContainer item in connected.getConnections()) {
                if (item.getConnections().Contains(container)) {
                    return true;
                }
            }
            if (connected.getConnections().Contains(container)) {
                return true;
            }
        }

        return false;
    }

    public int getPriority() {
        return 5;
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