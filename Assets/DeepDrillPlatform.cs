using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepDrillPlatform : TogglableStructure {

    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(100, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(100, HPHandler.ressources.Stone);
        return cost;
    }

    public override HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }

    public override string getDesc() {
        return "Mines Ores from the Cluster under it" + base.getDesc();
    }

	public override void doStart() {
		base.doStart();
	}
	
	public override void doStop() {
		base.doStop();
	}
}
