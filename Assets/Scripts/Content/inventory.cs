using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour, SaveLoad.SerializableInfo {

    private List<HPHandler.ressourceStack> content = new List<HPHandler.ressourceStack>();
    public float maxSize = 50f;

    public void remove(HPHandler.ressourceStack stack) {
        foreach (HPHandler.ressourceStack item in content) {
            if (item.getRessource().Equals(stack.getRessource())) {
                item.addAmount(-stack.getAmount());
            }
        }
    }

    public float getAmount(HPHandler.ressources kind) {
        foreach (HPHandler.ressourceStack item in content) {
            if (item.getRessource().Equals(kind)) {
                return item.getAmount();
            }
        }

        return 0;
    }

    public void takeAmount(HPHandler.ressources kind, float amount) {
        foreach (HPHandler.ressourceStack item in content) {
            if (item.getRessource().Equals(kind)) {
                item.addAmount(-amount);
            }
        }
    }

    public void transferAll(inventory target) {
        foreach (HPHandler.ressourceStack item in content) {
            target.add(item.clone());
            item.setAmount(0);
        }
        
        Debug.Log("transfering all inventory to: " + target.gameObject + " from " + this.gameObject);
    }

    public void transferAllSafe(inventory target) {
        foreach (HPHandler.ressourceStack item in content) {
            transfer(target, new HPHandler.ressourceStack(getMaxTransfer(this.gameObject, target.gameObject, item.getRessource()), item.getRessource()));
        }
        
        Debug.Log("transfering all inventory to: " + target + " from" + this.gameObject);
    }

    public void transferTo(inventory target, HPHandler.ressources type, float amount) {
        Debug.Log("Transferring from " + this.gameObject.name + " to " + target.gameObject.name + " type: " + type + ":" + amount);
        transfer(target, new HPHandler.ressourceStack(amount, type));
    }

    public void transfer(inventory target, HPHandler.ressourceStack stack) {
        target.add(stack);
        this.remove(stack);
    }

    public void add(HPHandler.ressourceStack stack) {
        foreach (HPHandler.ressourceStack item in content) {
            if (item.getRessource().Equals(stack.getRessource())) {
                item.addAmount(stack.getAmount());
                return;
            }
        }

        content.Add(stack);
        //Debug.Log("Added to inv: " + stack + " total: " + this.ToString());
    }

    public override string ToString(){
        string text = "";
        foreach (HPHandler.ressourceStack item in content) {
            text += item.ToString();
        }
        return "inventory: " + text + " unit:" + this.transform.gameObject.name;    
    }

    public bool isFull() {
        return getAmount() >= maxSize;

    }

    public float getAmount() {
        float amount = 0;
        foreach (HPHandler.ressourceStack item in content) {
            amount +=  item.getAmount();
        }

        return amount;
    }

    public bool canTake(HPHandler.ressourceStack stack) {
        
        foreach (HPHandler.ressourceStack item in content) {
            if (item.getRessource().Equals(stack.getRessource()) && item.getAmount() >= stack.getAmount()) {
                return true;
            }
        }

        return false;
    }

    public bool canTake(HPHandler.ressources kind, float amount) {
        return canTake(new HPHandler.ressourceStack(amount, kind));
    }

    public float getFreeSpace() {
        return this.maxSize - this.getAmount();
    }

    public float getFillPercent() {
        return this.getAmount() / this.maxSize;
    }

    public static float getMaxTransfer(GameObject from, GameObject to, HPHandler.ressources type) {

        inventory fromInv = from.GetComponent<inventory>();
        inventory toInv = to.GetComponent<inventory>();
        float toLimit = toInv.getFreeSpace();

        if (fromInv.getAmount(type) > toLimit) {
            return toLimit;
        } else {
            return fromInv.getAmount(type);
        }
    }

    
    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(maxSize, content);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        print("got deserialization for: " + info.scriptTarget);

        serializationData data = (serializationData) info;
        print("deserilazing...");
        this.maxSize = data.maxSize;
        this.content = data.content;
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        
        public float maxSize;
        public List<HPHandler.ressourceStack> content;

        public serializationData(float maxSize, List<HPHandler.ressourceStack> content) {
            this.maxSize = maxSize;
            this.content = content;
        }

        public override string scriptTarget {
            get {
                return "inventory";
            }
        }
    }
}
