﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class building_marker : MonoBehaviour, SaveLoad.SerializableInfo, clickable {

    public GameObject buildTo = null;
    private Structure Info = null;
    public ressourceStack[] overrideCost = null;
    private bool inited = false;
    private List<ressourceStack> missing = new List<ressourceStack>();
    private List<ressourceStack> ordered = new List<ressourceStack>();

    // Use this for initialization
    void Init() {

        Debug.Log("initing building marker that will be built to: " + buildTo);
        fixScale();

        Info = buildTo.GetComponent<Structure>();
        inited = true;

        if (overrideCost != null) {
            print("using override cost!");
            foreach (ressourceStack stack in overrideCost) {
                missing.Add(stack);
            }
        } else {
            foreach (ressourceStack stack in Info.getCost()) {
                missing.Add(stack);
            }
        }


        orderIdle();
    }

    private void fixScale() {

        this.transform.localScale = this.buildTo.transform.localScale;
        Vector3 childScale = new Vector3(1, 1, 1);

        childScale.x = 1 / this.buildTo.transform.localScale.x;
        childScale.y = 1 / this.buildTo.transform.localScale.y;
        childScale.z = 1 / this.buildTo.transform.localScale.z;

        this.transform.GetChild(0).localScale = childScale;

        if (buildTo.name.Contains("DeepDrillingPlatform")) {
            this.transform.localScale = new Vector3(3f, 3f, 3f);
        }
    }

    // Update is called once per frame
    long count = 0;
    void FixedUpdate() {
        if (buildTo == null) {
            return;
        }

        if (!inited) {
            Init();
        }
        count++;

        if (count % 60 == 0) {
            orderIdle();
        }

        if (checkDone()) {
            hasAll();
        }

    }

    private bool checkDone() {

        inventory own = this.gameObject.GetComponent<inventory>();

        var iterateList = Info.getCost();
        if (overrideCost != null) {
            iterateList = overrideCost;
        }

        foreach (ressourceStack elem in iterateList) {
            if (!own.canTake(elem)) {
                return false;
            }
        }

        return true;
    }

    private void hasAll() {
        Debug.Log("got all required ressources, building structure now!");
        GameObject done = GameObject.Instantiate(buildTo, this.transform.position, this.transform.rotation);

        var pos = done.transform.position;

        if (buildTo.name.Contains("SteamBoiler")) {
            pos.y -= 2.5f;
        }

        if (buildTo.name.Contains("CoolingGrid") || buildTo.name.Contains("HeatReflector")) {
            pos.y -= 0.2f;
        }
        done.transform.position = pos;
        GameObject.Destroy(this.gameObject);
    }

    /*public void removeOrdered(HPHandler.ressourceStack stack) {
        foreach(HPHandler.ressourceStack elem in ordered) {
            if(elem.Equals(stack)) {
                ordered.Remove(elem);
                missing.Add(elem);
            }
        }

    }*/


    private void reloadMissing() {
        missing.Clear();
        var iterateList = Info.getCost();
        if (overrideCost != null) {
            iterateList = overrideCost;
        }

        foreach (ressourceStack stack in iterateList) {
            inventory inv = this.GetComponent<inventory>();
            ressourceStack stillMissing = stack;
            stillMissing.addAmount(-inv.getAmount(stack.getRessource()));
            missing.Add(stillMissing);
        }
    }

    private void reloadOrdered() {

        reloadMissing();
        ordered.Clear();

        foreach (GameObject elem in GameObject.FindGameObjectsWithTag("mover")) {
            ActionController controller = elem.GetComponent<ActionController>();

            //check if it is delivering to this
            if (controller.getState().Equals(ActionController.State.ConstructionDelivering) && controller.getDeliverTarget().Equals(this.gameObject)) {
                bool added = false;
                foreach (ressourceStack stack in ordered) {
                    if (stack.getRessource().Equals(controller.getDelivery().getRessource())) {
                        stack.addAmount(controller.getDelivery().getAmount());
                        added = true;
                        break;
                    }
                }
                if (!added) {
                    this.ordered.Add(controller.getDelivery().clone());
                }
            }
        }

        foreach (ressourceStack order in ordered) {
            foreach (ressourceStack miss in missing) {
                if (miss.getRessource().Equals(order.getRessource())) {
                    miss.addAmount(-order.getAmount());
                }
            }
        }
    }

    private void orderIdle() {

        //Debug.Log("finding idle unit to deliver things");

        reloadOrdered();
        removeEmpty(missing);
        Debug.Log("searching idles, already ordered: " + printList(ordered) + " missing: " + printList(missing));
        if (missing.Count <= 0) {
            return;
        }

        foreach (GameObject elem in GameObject.FindGameObjectsWithTag("dropBase")) {
            foreach (ressourceStack stack in missing) {

                if (stack.getAmount() <= 0) {
                    continue;
                }

                if (elem.GetComponent<inventory>().getAmount(stack.getRessource()) > .0f) {
                    Transform mover = GetClosestMover(GameObject.FindGameObjectsWithTag("mover"));

                    if (mover == null) {
                        return;
                    }

                    Debug.Log("found valid idle mover");
                    ressourceStack take = stack.clone();

                    float takeable = elem.GetComponent<inventory>().getAmount(stack.getRessource());
                    if (takeable > mover.gameObject.GetComponent<inventory>().maxSize) {
                        takeable = mover.gameObject.GetComponent<inventory>().maxSize;
                    }

                    if (stack.getAmount() <= takeable) {
                        stack.setAmount(0);
                        //ordered.Add(stack);
                    } else {
                        stack.addAmount(-takeable);
                        take.setAmount(takeable);
                    }

                    Debug.Log("delivering to marker: " + take);
                    mover.gameObject.GetComponent<ActionController>().deliverTo(elem, this.gameObject, take);
                }
            }
        }
    }

    private void removeEmpty(List<ressourceStack> list) {

        List<ressourceStack> remove = new List<ressourceStack>();

        foreach (ressourceStack stack in list) {
            if (stack.getAmount() <= 0) {
                remove.Add(stack);
            }
        }

        foreach (ressourceStack stack in remove) {
            list.Remove(stack);
        }

    }

    private Transform GetClosestMover(GameObject[] movers) {

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in movers) {

            if (!potentialTarget.GetComponent<ActionController>().getState().Equals(ActionController.State.Idle)) {
                continue;
            }

            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }

    public void deliveryArrived(ressourceStack stack) {
        Debug.Log("Delivery arrived: " + stack);
        /*foreach(HPHandler.ressourceStack elem in ordered) {
            if (elem.Equals(stack)) {
                ordered.Remove(elem);
            }

        }*/

        if (checkDone()) {
            hasAll();
            return;
        }

        Debug.Log("Total ressources that arrived: " + this.GetComponent<inventory>() + " ordered: " + printList(ordered) + " missing" + printList(missing));

        orderIdle();
    }

    private string printList(List<ressourceStack> list) {
        string toReturn = "";
        foreach (ressourceStack stack in list) {
            toReturn += stack;
        }
        return toReturn;
    }


    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(buildTo, inited, missing, ordered, overrideCost);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        print("got deserialization for: " + info.scriptTarget);

        serializationData data = (serializationData)info;
        print("deserilazing building_marker...");

        List<GameObject> prefabs = GameObject.Find("Terrain").GetComponent<SaveLoad>().prefabs;
        GameObject prefabFound = null;
        foreach (GameObject prefab in prefabs) {
            if (prefab.name.Equals(data.buildTo)) {
                prefabFound = prefab;
                break;
            }
        }

        this.buildTo = prefabFound;
        this.inited = data.inited;
        this.Info = buildTo.GetComponent<Structure>();
        this.overrideCost = data.overrideCost;
        this.missing = data.missing;
        this.ordered = data.ordered;
        this.GetComponent<MeshFilter>().sharedMesh = buildTo.GetComponent<MeshFilter>().sharedMesh;
        this.GetComponent<MeshCollider>().sharedMesh = buildTo.GetComponent<MeshFilter>().sharedMesh;
        fixScale();

    }

    public void handleClick() {
        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }
    }

    public void handleLongClick() {
        //do nothing
    }

    public void handleOption(string option) {
        //do notghing
    }

    public void displayInfo() {
        //TODO
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        return null;
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        public string buildTo;
        public bool inited;
        public List<ressourceStack> missing;
        public List<ressourceStack> ordered;
        public ressourceStack[] overrideCost;

        public serializationData(GameObject buildTo, bool inited, List<ressourceStack> missing, List<ressourceStack> ordered, ressourceStack[] overrideCost) {
            string prefabName = buildTo.name;
            prefabName = prefabName.Replace("Clone", "");
            prefabName = prefabName.Replace(" ", "");
            prefabName = prefabName.Replace("(", "");
            prefabName = prefabName.Replace(")", "");
            prefabName = System.Text.RegularExpressions.Regex.Replace(prefabName, @"[\d-]", string.Empty);
            this.buildTo = prefabName;
            this.inited = inited;
            this.missing = missing;
            this.ordered = ordered;
            this.overrideCost = overrideCost;
        }

        public override string scriptTarget {
            get {
                return "building_marker";
            }
        }
    }
}
