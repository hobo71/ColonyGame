using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapBurner : DefaultStructure {

    public Sprite stopBut;
    public Sprite startBut;

    private GameObject workLight = null;

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(100, ressources.Wood);
        cost[1] = new ressourceStack(250, ressources.Stone);
        //TODO
        //cost[2] = new HPHandler.ressourceStack(20, HPHandler.ressources.Scrap);
        return cost;
    }

    public override ressourceStack[] getCost() {
        return getPrice();
    }

    public override int getMaxEnergy() {
        return 3000;
    }

    public override int getMaxInput() {
        return 0;
    }

    public override int getMaxOutput() {
        return 50;
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

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption stop = new PopUpCanvas.popUpOption("doStop", stopBut);
        PopUpCanvas.popUpOption start = new PopUpCanvas.popUpOption("doStart", startBut);

        if (this.getCurEnergy() >= this.getMaxEnergy() || this.busy) {
            start.setEnabled(false);
        }

        if (!this.busy) {
            stop.setEnabled(false);
        }

        options = new PopUpCanvas.popUpOption[] { info, stop, start };
        return options;
    }

    public override int getPriority() {
        return 0;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        if (salvaging) {
            return;
        }

        switch (option) {
            case "doStop":
                Debug.Log("Clicked stop button");
                this.busy = false;
                Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Stopping", Color.red, false);
                this.GetComponent<Animator>().SetBool("working", false);
                workLight.GetComponent<Light>().enabled = false;
                this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().Stop();
                break;
            case "doStart":
                Debug.Log("Clicked start button");
                this.busy = true;
                Notification.createNotification(this.gameObject, Notification.sprites.Working, "Burning...", Color.green, true);
                break;
            default:
                displayInfo();
                break;
        }
    }

    public override void handleClick() {
        Debug.Log("clicked scrapburner");

        if (salvaging)
            return;

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
        
        this.busy = true;
        Notification.createNotification(this.gameObject, Notification.sprites.Working, "Burning...", Color.green, true);

    }

    public override string getDesc() {
        return "The Scrap Burner can, as you thought, burn scrap! By doing so, it generates large amounts of energy, and it burns through scrap rather fast." + base.getDesc();
    }

    private float energyPerSecond = 30f;
    private float scrapPerSecond = 5f;

    new void FixedUpdate() {

        base.FixedUpdate();

        if (workLight == null) {
            workLight = this.transform.Find("DrillSparks").transform.Find("Point Light").gameObject;
        }
        
        if (counter % 5 == 0) {
            handleConnections();
        }

        if (this.isBusy()) {
            //check if it has scrap, if true then recycle INFO: 1 Scrap = 3 Energy
            if (this.GetComponent<inventory>().getAmount(ressources.Scrap) >= 1) {
                this.addEnergy(energyPerSecond * Time.deltaTime, this);
                this.GetComponent<inventory>().remove(new ressourceStack(scrapPerSecond * Time.deltaTime, ressources.Scrap));
            }

            if (this.GetComponent<inventory>().getAmount(ressources.Scrap) >= 1) {
                this.GetComponent<Animator>().SetBool("working", true);
                workLight.GetComponent<Light>().enabled = true;
                this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().Play();
            } else {
                this.GetComponent<Animator>().SetBool("working", false);
                workLight.GetComponent<Light>().enabled = false;
                this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().Stop();
            }
            //send for moar scrap!!
            if (counter % 60 == 0 && !this.GetComponent<inventory>().isFull()) {
                RessourceHelper.deliverTo(this.gameObject, false, ressources.Scrap);
            }
        }

    }
}
