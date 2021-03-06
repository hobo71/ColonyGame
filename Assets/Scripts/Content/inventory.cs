﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour, SaveLoad.SerializableInfo {

    private Dictionary<ressources, float> newContent = new Dictionary<ressources, float>();
    public float maxSize = 50f;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable() {
        foreach (ressources item in Enum.GetValues(typeof(ressources))) {
            newContent[item] = 0;
        }
    }

    public void remove(ressourceStack stack) {
        newContent[stack.getRessource()] -= stack.getAmount();

    }

    public float getAmount(ressources kind) {
        return newContent[kind];
    }

    public void takeAmount(ressources kind, float amount) {
        newContent[kind] -= amount;
    }

    public void transferAll(inventory target) {

        foreach (ressources item in Enum.GetValues(typeof(ressources))) {
            target.newContent[item] += this.newContent[item];
            this.newContent[item] = 0;
        }
    }

    public void transferSafe(inventory target, ressources item) {
        var amount = newContent[item];
        if (amount > target.getFreeSpace()) {
            amount = target.getFreeSpace();
        }

        target.newContent[item] += amount;
        this.newContent[item] -= amount;
    }

    public void transferAllSafe(inventory target) {
        foreach (ressources item in Enum.GetValues(typeof(ressources))) {
            transferSafe(target, item);
        }
    }

    public void transfer(inventory target, ressources type, float amount) {
        target.newContent[type] += amount;
        this.newContent[type] -= amount;
    }

    public void transfer(inventory target, ressourceStack stack) {
        target.add(stack);
        this.remove(stack);
    }

    public void add(ressourceStack stack) {
        this.newContent[stack.getRessource()] += stack.getAmount();
    }

    public void add(ressources type, float amount) {
        this.newContent[type] += amount;
    }

    public override string ToString() {
        string text = "";
        foreach (ressources item in Enum.GetValues(typeof(ressources))) {
            text += item.ToString() + ": " + this.newContent[item] + " ";
        }
        return "inventory: " + text + " unit:" + this.transform.gameObject.name;
    }

    public bool isFull() {
        return getAmount() >= maxSize;

    }

    public float getAmount() {
        float amount = 0;
        foreach (ressources item in Enum.GetValues(typeof(ressources))) {
            amount += newContent[item];
        }

        return amount;
    }

    //checks if there's enough ressources to be taken
    public bool canTake(ressourceStack stack) {

        return newContent[stack.getRessource()] >= stack.getAmount();
    }

    public bool canTake(ressources kind, float amount) {
        return newContent[kind] >= amount;
    }

    public float getFreeSpace() {
        return this.maxSize - this.getAmount();
    }

    public float getFillPercent() {
        return this.getAmount() / this.maxSize;
    }

    public float getFillPercent(ressources kind) {
        return this.getAmount(kind) / this.maxSize;
    }

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(maxSize, newContent);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        print("got deserialization for: " + info.scriptTarget);

        serializationData data = (serializationData)info;
        print("deserilazing...");
        this.maxSize = data.maxSize;
        this.newContent = data.newContent;
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {

        public float maxSize;
        public Dictionary<ressources, float> newContent = new Dictionary<ressources, float>();

        public serializationData(float maxSize, Dictionary<ressources, float> newContent) {
            this.maxSize = maxSize;
            this.newContent = newContent;
        }

        public override string scriptTarget {
            get {
                return "inventory";
            }
        }
    }
}
