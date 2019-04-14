using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralProcessingPlant : DefaultStructure {

    public Sprite stopBut;
    public Sprite startBut;

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(200, ressources.Wood);
        cost[1] = new ressourceStack(100, ressources.Stone);
        return cost;
    }

    public override ressourceStack[] getCost() {
        return getPrice();
    }

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
                //this.GetComponent<Animator>().SetBool("working", false);
                //this.transform.Find("CFX3_Fire_Shield").GetComponent<ParticleSystem>().Stop();
                this.transform.Find("ProcessingPlant_Anim").GetComponent<ParticleSystem>().Stop();
                break;
            case "doStart":
                Debug.Log("Clicked start button");
                DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject);
                this.busy = true;
                Notification.createNotification(this.gameObject, Notification.sprites.Starting, "Processing...", Color.green, true);
                break;
            default:
                displayInfo();
                break;
        }
    }
	
    public override void handleClick() {
        Debug.Log("clicked Mineralthingi");

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
        DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, ressources.Iron);
        DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject, ressources.Gold);

	}

    public override string getDesc() {
        return "Used to Process Iron and Gold ore. Results in Ingots" + base.getDesc() + " " + this.GetComponent<inventory>();
    }
	
    private int OrePerSecond = 10;
    private int IngotsPerSecond = 5;
    private int energyPerSecond = 5;
    public override void FixedUpdate() {

        base.FixedUpdate();

        if (this.isBusy()) {
            //ProcessingPlant_Anim
			if (this.getCurEnergy() > 3) {

                //request both ores
				if (counter % 180 == 0 && !this.GetComponent<inventory>().isFull()) {
					RessourceHelper.deliverTo(this.gameObject, false, ressources.OreIron);
					RessourceHelper.deliverTo(this.gameObject, false, ressources.OreGold);
				}

				//select right ore
				var ore = ressources.OreIron;
				if (this.GetComponent<inventory>().getAmount(ressources.OreIron) < 0.2f) {
					ore = ressources.OreGold;
					if (this.GetComponent<inventory>().getAmount(ressources.OreGold) < 0.2f) {
                        this.transform.Find("ProcessingPlant_Anim").GetComponent<ParticleSystem>().Stop();
						return;
					}
				}

                
                if (!this.transform.Find("ProcessingPlant_Anim").GetComponent<ParticleSystem>().isPlaying)
                    this.transform.Find("ProcessingPlant_Anim").GetComponent<ParticleSystem>().Play();
				
				this.addEnergy(-energyPerSecond * Time.deltaTime, this);
				this.GetComponent<inventory>().remove(new ressourceStack(OrePerSecond * Time.deltaTime, ore));

				var ingot = ore.Equals(ressources.OreIron) ? ressources.Iron : ressources.Gold;
				this.GetComponent<inventory>().add(new ressourceStack(IngotsPerSecond * Time.deltaTime, ingot));

			} else {
                this.transform.Find("ProcessingPlant_Anim").GetComponent<ParticleSystem>().Stop();
            }
		}
        
    }
}
