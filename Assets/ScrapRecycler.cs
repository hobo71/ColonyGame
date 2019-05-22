using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapRecycler : ProductionStructure {
	public override List<ProduceData> getProduceData() {
		var list = new List<ProduceData> {
			new ProduceData() {
				consume = new List<ressourceStack>() {new ressourceStack(5, ressources.Scrap)},
				outcome = new List<ressourceStack>() {
					new ressourceStack(2.5f, ressources.Stone),
					new ressourceStack(1f, ressources.Wood),
					new ressourceStack(0.3f, ressources.Iron),
				},
				energyCost = 2f
			}
		};
		return list;
	}

	private static ressourceStack[] getPrice() {
		ressourceStack[] cost = new ressourceStack[3];

		cost[0] = new ressourceStack(100, ressources.Wood);
		cost[1] = new ressourceStack(100, ressources.Stone);
		cost[2] = new ressourceStack(100, ressources.Scrap);
		return cost;
	}

	public override ressourceStack[] getCost() {
		return getPrice();
	}
}
