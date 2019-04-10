using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterpump : TogglableStructure {
    public override HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }
    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(50, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(20, HPHandler.ressources.Stone);
        cost[2] = new HPHandler.ressourceStack(20, HPHandler.ressources.Iron);
        return cost;
    }

    public override string getDesc() {
        return "The Water pump pumps out water from a nearby lake. Requires energy to work"
         + base.getDesc();
    }

    public override int getMaxEnergy() {
        return 1000;
    }

    public override int getMaxInput() {
        if (this.getMaxEnergy() - this.getCurEnergy() < 50) {
            return this.getMaxEnergy() - this.getCurEnergy();
        }
        return 50;
    }

    public override int getMaxOutput() {
        return 0;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    new void FixedUpdate() {
        base.FixedUpdate();
        if (this.getCurEnergy() > 5 && this.busy) {
            //working, get water!
            this.addEnergy(-5f * Time.deltaTime, this);
            var stack = new HPHandler.ressourceStack(10 * Time.deltaTime, HPHandler.ressources.Water);
            this.getInv().add(stack);
        }
    }
}
