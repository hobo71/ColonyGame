using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterpump : TogglableStructure {
    public override ressourceStack[] getCost() {
        return getPrice();
    }
    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[3];

        cost[0] = new ressourceStack(50, ressources.Wood);
        cost[1] = new ressourceStack(20, ressources.Stone);
        cost[2] = new ressourceStack(20, ressources.Iron);
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
    
    public static bool isNearWater(GameObject holoPlacement, bool terrainCheck) {

        if (!terrainCheck) {
            return false;
        }

        var parts = GameObject.FindGameObjectsWithTag("water");
        foreach (var item in parts) {
            //check if distance between colliders is low enough
            //TODO make animation of pipe expand towards water
            
            var tarColl = item.gameObject.GetComponent<Collider>();

            var sampleTar = tarColl.ClosestPoint(holoPlacement.gameObject.transform.position);

            //check if close enough
            if (Vector3.Distance(holoPlacement.gameObject.transform.position, sampleTar) < 3) {
                return true;
            }

            if (true)
            {
                
            }
        }

        return false;
    }
    
    new void FixedUpdate() {
        base.FixedUpdate();
        
        //stop updating if not working
        if (this.getCurEnergy() <= 5 || !this.busy) return;
        
        //working, get water!
        this.addEnergy(-5f * Time.deltaTime, this);
        this.getInv().add(ressources.Water, 10 * Time.deltaTime);
    }
}
