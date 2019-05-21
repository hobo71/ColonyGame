using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreWashPlant : ProductionStructure {
    public override List<ProduceData> getProduceData() {
        var list = new List<ProduceData>();
        list.Add(new ProduceData() {
            consume = new List<ressourceStack>() {
                new ressourceStack(2, ressources.Stone)
            }, outcome = new List<ressourceStack>() {
            new ressourceStack(0.05f, ressources.Uranium), 
            new ressourceStack(0.3f, ressources.OreIron),
            new ressourceStack(0.1f, ressources.OreGold),
            new ressourceStack(0.01f, ressources.OreIridium),
            }, energyCost = 5f
        });
        return list;
    }

    private static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(100, ressources.Wood);
        cost[1] = new ressourceStack(250, ressources.Stone);
        return cost;
    }

    public override ressourceStack[] getCost() {
        return getPrice();
    }
}