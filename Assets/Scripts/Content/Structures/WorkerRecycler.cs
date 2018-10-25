using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerRecycler : DefaultStructure {
    public Sprite salvageBut;
    public GameObject destroyAnim;

    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(50, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(10, HPHandler.ressources.Stone);
        return cost;
    }

    public override HPHandler.ressourceStack[] getCost() {
        return WorkerRecycler.getPrice();
    }

    public override string getDesc() {
        
        return "A small building to salvage workers" + base.getDesc();
    }

    public override int getMaxEnergy() {
        return 510;
    }

    public override int getMaxInput() {
        return 10;
    }

    public override int getMaxOutput() {
        return 5;
    }

    public override PopUpCanvas.popUpOption[] getOptions() {
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
         PopUpCanvas.popUpOption salvage = new  PopUpCanvas.popUpOption("doSalvage", salvageBut);

        if (this.getCurEnergy() < 490 || this.busy) {
            salvage.setEnabled(false);
        }
        
        options = new PopUpCanvas.popUpOption[]{info, salvage};
        return options;
    }

    public override int getPriority() {
        return 6;
    }

    public override void handleOption(string option) {
        Debug.Log("handling option: " + option);

        if (salvaging) {
            return;
        }

        switch (option) {
            case "doSalvage":
                Debug.Log("Clicked clone button");
                doSalvage();
                Notification.createNotification(this.gameObject, Notification.sprites.Starting, "", Color.green);
                break;
            default:
                displayInfo();
                break;
        }
    }

    private void doSalvage() {
        Debug.Log("looking for unit to salvage");

        GameObject[] movers = GameObject.FindGameObjectsWithTag("mover");
        Transform nearestMover = harvestableRessource.GetClosestMover(movers, true, this.transform);

        if (nearestMover == null) {
            Debug.Log("no idle mover available");
            nearestMover = harvestableRessource.GetClosestMover(movers, false, this.transform);
        }

        nearestMover.GetComponent<movementController>().setTarget(this.transform, "salvage");

    }

    public void workerArrived(GameObject workerArrived) {
        Debug.Log("got recycled worker!");
        this.storedEnergy -= 500;
        
        var effect = GameObject.Instantiate(destroyAnim, workerArrived.transform.position, workerArrived.transform.rotation);
        
        var pickup = Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().pickupBox, workerArrived.transform.position, Quaternion.identity);
        pickup.GetComponent<inventory>().add(new HPHandler.ressourceStack(50, HPHandler.ressources.Scrap));
    }

    public bool hasEnoughEnergy() {
        if (this.storedEnergy < 500) {  //in case multiple workers were called
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low, this.getCurEnergy() + "/500", Color.red);
        }
        return this.storedEnergy >= 500;
    }
    
    private float lastClick;

    public override void handleClick() {
        Debug.Log("clicked recycler");

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

        if (this.getCurEnergy() >= 490 && !this.busy) {
            doSalvage();
            Notification.createNotification(this.gameObject, Notification.sprites.Starting, this.getCurEnergy() + 500 + "/500", Color.green);
        } else if (this.busy) {
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "", Color.blue, true);
        } else {
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low, this.getCurEnergy() + "/500", Color.red);
        }
    }
}
