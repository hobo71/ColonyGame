using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TogglableStructure : DefaultStructure {

	/* pricing template:
	
    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(100, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(100, HPHandler.ressources.Stone);
        return cost;
    }

    public override HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }

	 */

    public Sprite stopBut;
    public Sprite startBut;
    public override int getMaxEnergy() {
		return 1000;
    }

    public override int getMaxInput() {
		return 50;
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
        return 4;
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
        Debug.Log("clicked Thing");

        if (salvaging)
            return;

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
        
        if (this.busy && this.getCurEnergy() > 3)
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "Working...", Color.green, true);
        if (!this.busy && this.getCurEnergy() > 100) {
            doStart();
        } else if (!this.busy && this.getCurEnergy() <= 100)
            Notification.createNotification(this.gameObject, Notification.sprites.Energy_Low, "Not enough Energy", Color.red, false);

    }

    public virtual void doStop() {
        //this.GetComponent<Animator>().SetBool("working", false);
        Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Stopped", Color.red);
        this.busy = false;
    }

    public virtual void doStart() {
        //this.GetComponent<Animator>().SetBool("working", true);
        Notification.createNotification(this.gameObject, Notification.sprites.Working, "Starting", Color.green, true);
        this.busy = true;
        //DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, HPHandler.ressources.Trees);
    }
}
