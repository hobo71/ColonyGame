﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillPlatformBasic : DefaultStructure {

    public Sprite stopBut;
    public Sprite startBut;

    private Transform curTarget;
    private bool rotated = false;
    private bool displayWorking = false;
    private GameObject BeamDisplay = null;
    private float maxRange = 80;
    private GameObject partSystem = null;

    public static ressourceStack[] getPrice() {
        ressourceStack[] cost = new ressourceStack[2];

        cost[0] = new ressourceStack(50, ressources.Wood);
        cost[1] = new ressourceStack(200, ressources.Stone);
        return cost;
    }
    public override ressourceStack[] getCost() {
        return DrillPlatformBasic.getPrice();
    }

    public override string getDesc() {
        return "A basic Drill dhat can automatically harvest nearby stones, ores and minerals (can also be focused by selected harvestable things)" + base.getDesc();
    }

    public override int getMaxEnergy() {
        return 3000;
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

    private void doStart() {
        doStart(searchTarget().transform);
    }

    private void doStart(Transform target) {

        Notification.createNotification(this.gameObject, Notification.sprites.Working, "Starting", Color.green, true);
        this.busy = true;

        curTarget = target;
        Debug.Log("setting harvest target to: " + curTarget);
        DeliveryRoutes.addRoute(this.gameObject, DeliveryRoutes.getClosest("dropBase", this.gameObject).gameObject);
    }

    private GameObject searchTarget() {
        GameObject[] clickables = GameObject.FindGameObjectsWithTag("clickable");
        List<GameObject> stones = new List<GameObject>();
        foreach (GameObject obj in clickables) {
            if (obj.GetComponent<HPHandler>() != null && (obj.GetComponent<HPHandler>().type == ressources.Stone || obj.GetComponent<HPHandler>().type == ressources.OreIron || obj.GetComponent<HPHandler>().type == ressources.OreGold || obj.GetComponent<HPHandler>().type == ressources.OreIridium)) {
                if (obj.GetComponent<deepRessource>() == null)
                    stones.Add(obj);
            }
        }

        Debug.Log("Found stones: " + stones.Count);
        GameObject closest = null;
        float dist = float.MaxValue;

        foreach (GameObject obj in stones) {
            float curDist = Vector3.Distance(obj.transform.position, this.transform.position);
            if (curDist < dist && curDist <= maxRange) {
                closest = obj;
                dist = curDist;
            }
        }

        return closest;
    }

    private void doStop() {
        Notification.createNotification(this.gameObject, Notification.sprites.Stopping, "Stopped", Color.red);
        this.busy = false;
        displayWorking = false;
        curTarget = null;
    }


    private float lastClick;

    public override void handleClick() {
        Debug.Log("clicked drill");

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

        if (this.busy) {
            Notification.createNotification(this.gameObject, Notification.sprites.Working, "Working", Color.cyan, true);
        } else {
            this.doStart();
        }
    }

    private void handleHarvest(Transform harvesting) {
        var amount = this.getHarvestRate() * Time.deltaTime * harvesting.GetComponent<harvestableRessource>().getHarvestMultiplier();
        ressourceStack stack = new ressourceStack(amount, harvesting.GetComponent<HPHandler>().type);
        this.GetComponent<inventory>().add(stack);
        harvesting.GetComponent<HPHandler>().HP -= amount;
    }

    public virtual int getEnergyDrainRate() {
        return 60;
    }

    public virtual float getHarvestRate() {
        return 10f;
    }

    // Use this for initialization
    public override void Start() {
        base.Start();
    }

    private float rotationProg;
    private Quaternion initialRotation = Quaternion.identity;

    private void setRotation() {
        Transform meshes = this.transform.Find("DrillMeshes");
        Transform rotatingPart = meshes.Find("CenterPillar");
        Transform elevationPart = rotatingPart.Find("Barrels").Find("MainDrill");


        Vector3 toTarget = curTarget.transform.position - elevationPart.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(toTarget);
        targetRotation *= Quaternion.Euler(new Vector3(1, 0, 0) * -90f);

        if (initialRotation == Quaternion.identity) {
            rotationProg = 0.0f;
            initialRotation = rotatingPart.transform.rotation;
        }

        if (rotationProg >= 0.99) rotationProg = 1.0f;

        Quaternion res = Quaternion.Lerp(initialRotation, targetRotation, rotationProg);
        rotationProg += 0.025f;

        rotatingPart.transform.rotation = res;
        //rotatingPart.transform.Rotate(new Vector3(1, 0, 0), -90f);

        if (rotationProg >= 1f) {
            rotated = true;
            initialRotation = Quaternion.identity;
            initBeam();
        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();


        if (this.GetComponent<inventory>().isFull() && curTarget != null) {
            doStop();
        }

        if (this.getCurEnergy() > 3 && this.busy) {
            this.addEnergy(-this.getEnergyDrainRate() * Time.deltaTime, this);
            if (curTarget != null && curTarget.GetComponent<HPHandler>().HP > 0) {
                if (rotated) {
                    handleHarvest(curTarget);
                    displayWorking = true;
                } else {
                    setRotation();
                    displayWorking = false;
                }

            } else {
                GameObject tar = searchTarget();
                if (tar == null) {
                    doStop();
                } else {
                    curTarget = tar.transform;
                }
                rotated = false;
                displayWorking = false;
            }
        } else if (this.getCurEnergy() <= 3 && this.busy) {
            this.doStop();
        }
    }

    private void initBeam() {
        Transform meshes = this.transform.Find("DrillMeshes");
        Transform rotatingPart = meshes.Find("CenterPillar");
        Transform elevationPart = rotatingPart.Find("Barrels");

        Vector3 tpos = curTarget.position;
        Ray ray = new Ray(elevationPart.position, curTarget.position - elevationPart.position);
        var collider = curTarget.GetComponent<Collider>();
        var hitinfo = new RaycastHit();
        collider.Raycast(ray, out hitinfo, 120f);

        var rayHit = hitinfo.point;

        float length = Vector3.Distance(elevationPart.position, rayHit);

        Debug.Log("Translating by: " + length / 2);
        /*Vector3 offset = new Vector3(0f, length / 3, 0f);
        Quaternion rotation = BeamDisplay.transform.rotation;
        offset = rotation * offset;
        //BeamDisplay.transform.Translate(offset, Space.World);*/
        BeamDisplay.transform.localScale = new Vector3(1f, 1f, length * 1.53f);

        GameObject.Destroy(partSystem);
        partSystem = GameObject.Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().harvestParticle, rayHit, Quaternion.identity);
    }

    //Update is called once per frame
    void Update() {
        if (BeamDisplay == null) {
            Transform meshes = this.transform.Find("DrillMeshes");
            Transform rotatingPart = meshes.Find("CenterPillar");
            Transform elevationPart = rotatingPart.Find("Barrels");
            BeamDisplay = elevationPart.Find("MainDrill").Find("BeamDisplay").gameObject;
            Debug.Log("Found BeamDisplay" + BeamDisplay);
        }

        if (displayWorking) {
            BeamDisplay.SetActive(true);
            BeamDisplay.GetComponent<LineRenderer>().material.mainTextureOffset += new Vector2(-2.0f * Time.deltaTime, 0);
            if (partSystem != null)
                partSystem.SetActive(true);
        } else {
            BeamDisplay.SetActive(false);
            if (partSystem != null)
                partSystem.SetActive(false);
        }
    }

    public bool setTarget(GameObject target) {
        if (Vector3.Distance(this.transform.position, target.transform.position) > maxRange) {
            return false;
        }

        doStart(target.transform);
        return true;
    }

    public new SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(storedEnergy, busy, salvaging, ownResource, curTarget);
    }

    public new void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData)info;
        this.storedEnergy = data.storedEnergy;
        this.busy = data.busy;
        this.ownResource = data.ownResource;
        this.salvaging = data.salvaging;
        this.curTarget = data.target;

        if (salvaging) {
            Debug.Log("got salvaging info, creating particles....");
            //TODO, this gets called before terrain scripts are loaded
            GameObject.Instantiate(ParticleHelper.getInstance().salvageParticles, this.gameObject.transform);
        }

        //reloadConnections();
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {

        public Transform target;
        public float storedEnergy;
        public bool built;
        public bool busy;
        public bool salvaging;
        public ressourceStack[] ownResource;

        public serializationData(float storedEnergy, bool busy, bool salvaging, ressourceStack[] ownResource, Transform target) {
            this.storedEnergy = storedEnergy;
            this.busy = busy;
            this.ownResource = ownResource;
            this.salvaging = salvaging;
            this.target = target;
        }

        public override string scriptTarget {
            get {
                return "DrillPlatformBasic";
            }
        }
    }
}
