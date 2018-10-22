using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFarm : DefaultStructure {
    public Sprite stopBut;
    public Sprite startBut;

    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(150, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(200, HPHandler.ressources.Stone);
        return cost;
    }

    public override int getMaxEnergy() {
        return 1000;
    }

    public override int getMaxInput() {
        if (this.getMaxEnergy() - this.getCurEnergy() < 50) {
            return this.getMaxEnergy() - this.getCurEnergy();
        }
        return 30;
    }

    public override int getMaxOutput() {
        return 0;
    }

    public override PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption stop = new PopUpCanvas.popUpOption("doStop", stopBut);
        PopUpCanvas.popUpOption start = new PopUpCanvas.popUpOption("doStart", startBut);

        if (this.getCurEnergy() < 100 || this.busy) {
            start.setEnabled(false);
        }

        if (!this.busy) {
            stop.setEnabled(false);
        }

        options = new PopUpCanvas.popUpOption[] { info, stop, start };
        return options;
    }
    public override int getPriority() {
        return 5;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        if (salvaging) {
            return;
        }

        switch (option) {
            case "doStop":
                Debug.Log("Clicked stop button");
                doStop();
                break;
            case "doStart":
                Debug.Log("Clicked start button");
                doStart();
                break;
            default:
                displayInfo();
                break;
        }
    }

    public override void handleClick() {
        Debug.Log("clicked TreeFarm");

        if (salvaging)
            return;

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
        
        if (this.busy && this.getCurEnergy() > 3)
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "Working...", Color.green, true);
        if (!this.busy && this.getCurEnergy() > 100) {
            this.busy = true;
            Notification.createNotification(this.gameObject, Notification.sprites.Starting, "Starting", Color.green, true);

        } else if (!this.busy && this.getCurEnergy() <= 100)
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low, "Not enough Energy", Color.red, false);

    }

    private void doStop() {
        this.GetComponent<Animator>().SetBool("working", false);
        Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Stopped", Color.red);
        this.busy = false;
    }

    private void doStart() {
        Notification.createNotification(this.gameObject, Notification.sprites.Working, "Starting", Color.green, true);
        this.busy = true;

        Debug.Log("starting Tree farm");
        //TODO deliver to nearby wood reprocessor
        //DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, HPHandler.ressources.Stone);
    }

    public virtual int getEnergyDrainRate() {
        return 10;
    }

    private float treesPerSecond() {
        return 4f;
    }
    
    float timePassed = 0f;
    public override void FixedUpdate() {
        base.FixedUpdate();
        
        if (!this.busy) return;

        if (this.GetComponent<inventory>().isFull() || this.getCurEnergy() < 3) {
            doStop();
            return;
        }

        if (this.getCurEnergy() > 3 && this.busy) {
            this.addEnergy(-getEnergyDrainRate() * Time.deltaTime, this);
            this.GetComponent<Animator>().SetBool("working", true);
            
            this.GetComponent<inventory>().add(new HPHandler.ressourceStack(treesPerSecond() * Time.deltaTime, HPHandler.ressources.Trees));
        }
    }
    public new SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(storedEnergy, busy, salvaging, ownResource);
    }

    public new void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData)info;
        this.storedEnergy = data.storedEnergy;
        this.busy = data.busy;
        this.ownResource = data.ownResource;
        this.salvaging = data.salvaging;

        if (salvaging) {
            GameObject.Instantiate(ParticleHelper.getInstance().salvageParticles, this.gameObject.transform);
        }

        //reloadConnections();
    }

    public override HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        public float storedEnergy;
        public bool built;
        public bool busy;
        public bool salvaging;
        public HPHandler.ressourceStack[] ownResource;

        public serializationData(float storedEnergy, bool busy, bool salvaging, HPHandler.ressourceStack[] ownResource) {
            this.storedEnergy = storedEnergy;
            this.busy = busy;
            this.ownResource = ownResource;
            this.salvaging = salvaging;
        }

        public override string scriptTarget {
            get {
                return "TreeFarm";
            }
        }
    }
}
