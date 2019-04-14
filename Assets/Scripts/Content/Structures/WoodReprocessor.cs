using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodReprocessor : DefaultStructure {

    public Sprite stopBut;
    public Sprite startBut;

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(100, ressources.Wood);
        cost[1] = new ressourceStack(250, ressources.Stone);
        return cost;
    }

    public override ressourceStack[] getCost() {
        return getPrice();
    }

    public override int getMaxEnergy() {
        return 500;
    }

    public override int getMaxInput() {
        return 20;
    }

    public override int getMaxOutput() {
        return 0;
    }

    public override PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption info = new PopUpCanvas.popUpOption("Info", infoBut);
        PopUpCanvas.popUpOption stop = new PopUpCanvas.popUpOption("doStop", stopBut);
        PopUpCanvas.popUpOption start = new PopUpCanvas.popUpOption("doStart", startBut);

        if (this.busy || this.getCurEnergy() < 5) {
            start.setEnabled(false);
        }

        if (!this.busy) {
            stop.setEnabled(false);
        }

        options = new PopUpCanvas.popUpOption[] {info, stop, start};
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
                this.transform.Find("CFX3_Fire_Shield").GetComponent<ParticleSystem>().Stop();
                break;
            case "doStart":
                Debug.Log("Clicked start button");
                DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, ressources.Wood);
                this.busy = true;
                Notification.createNotification(this.gameObject, Notification.sprites.Starting, "Processing...", Color.green, true);
                break;
            default:
                displayInfo();
                break;
        }
    }

    public override void handleClick() {
        Debug.Log("clicked WoodReprocessor");

        if (salvaging)
            return;

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }

        if (this.getCurEnergy() < 5) {
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low, "Not enough energy", Color.red);
            return;
        }

        this.busy = true;
        Notification.createNotification(this.gameObject, Notification.sprites.Working, "Processing...", Color.green, true);
        DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, ressources.Wood);

    }

    public override string getDesc() {
        return "Used to make wood out of trees. It's that simple!" + base.getDesc();
    }

    private int treesPerSecond = 8;
    private int WoodPerSecond = 12;
    private int energyPerSecond = 5;

    public override void FixedUpdate() {

        base.FixedUpdate();

        if (this.isBusy()) {
            if (counter % 180 == 0 && !this.GetComponent<inventory>().isFull()) {
                RessourceHelper.deliverTo(this.gameObject, false, ressources.Trees);
            }
            //check if it has scrap, if true then recycle INFO: 1 Scrap = 3 Energy
            if (this.GetComponent<inventory>().getAmount(ressources.Trees) >= 1 && this.getCurEnergy() > 3) {
                this.addEnergy(-energyPerSecond * Time.deltaTime, this);
                this.GetComponent<inventory>().add(new ressourceStack(WoodPerSecond * Time.deltaTime, ressources.Wood));
                this.GetComponent<inventory>().remove(new ressourceStack(treesPerSecond * Time.deltaTime, ressources.Trees));
            }

            if (this.GetComponent<inventory>().getAmount(ressources.Trees) >= 1 && this.getCurEnergy() > 3) {
                this.GetComponent<Animator>().SetBool("working", true);
                if (!this.transform.Find("CFX3_Fire_Shield").GetComponent<ParticleSystem>().isPlaying)
                    this.transform.Find("CFX3_Fire_Shield").GetComponent<ParticleSystem>().Play();
                
            } else {
                this.GetComponent<Animator>().SetBool("working", false);
                this.transform.Find("CFX3_Fire_Shield").GetComponent<ParticleSystem>().Stop();
            }
        }
        
    }
}
