using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepDrillPlatform : TogglableStructure {

    public GameObject light;

    private HPHandler.ressources ingot = HPHandler.ressources.OreIron;
    private GameObject mineralPatch = null;
    public static HPHandler.ressourceStack[] getPrice() {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(100, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(100, HPHandler.ressources.Stone);
        return cost;
    }

    public override HPHandler.ressourceStack[] getCost() {
        return getPrice();
    }

    private void init() {

    }

    public override string getDesc() {
        return "Mines Ores from the Cluster under it" + base.getDesc();
    }

    public override void doStart() {
        base.doStart();
        startAnim();
    }

    public override void doStop() {
        base.doStop();
        stopAnim();
    }

    private void startAnim() {
        this.GetComponent<Animator>().SetBool("working", true);
        if (!this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().isPlaying)
            this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().Play();
        light.SetActive(true);
    }

    private void stopAnim() {
        this.GetComponent<Animator>().SetBool("working", false);
        this.transform.Find("DrillSparks").GetComponent<ParticleSystem>().Stop();
        light.SetActive(false);
    }

    public void setIngot(HPHandler.ressources ingot, GameObject patch) {
        this.ingot = ingot;
        this.mineralPatch = patch;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public override void Start() {
        base.Start();
        var patches = GameObject.FindObjectsOfType<deepRessource>();

        GameObject res = null;
        var dist = float.MaxValue;

        foreach (var elem in patches) {
            var d = Vector3.Distance(elem.gameObject.transform.position, this.gameObject.transform.position);
            if (d < dist) {
                d = dist;
                res = elem.gameObject;
            }
        }

        ingot = res.GetComponent<HPHandler>().type;
        if (res.GetComponent<deepRessource>().activeDrill != null) {
            return;
        }
        mineralPatch = res;
        res.GetComponent<deepRessource>().activeDrill = this.gameObject;
    }

    private float EnergyPerSecond = 30f;
    private float OrePerSecond = 20f;

    override public void FixedUpdate() {
        base.FixedUpdate();

        if (this.isBusy()) {
            if (this.getCurEnergy() >= 3) {
                startAnim();
                this.addEnergy(-EnergyPerSecond * Time.deltaTime, this);
                var stack = new HPHandler.ressourceStack(OrePerSecond * Time.deltaTime, ingot);
                this.GetComponent<inventory>().add(stack);
                try {
                    mineralPatch.GetComponent<HPHandler>().HP -= OrePerSecond * Time.deltaTime;
                } catch (NullReferenceException ex) {
                    doStop();
                }
            } else {
                stopAnim();
            }
        }
    }
}
