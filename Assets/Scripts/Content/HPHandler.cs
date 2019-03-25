using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPHandler : MonoBehaviour, SaveLoad.SerializableInfo {
    
    public float HP = 100;
    public ressources type = ressources.Stone;
    private float initialHP;

    [System.Serializable]
    public class ressourceStack {
        private float amount;
        private readonly ressources type;

        public ressourceStack clone() {
            return new ressourceStack(amount, type);
        }

        public ressourceStack(float amount, ressources type) {
            this.amount = amount;
            this.type = type;
        }

        public float getAmount() {
            return amount;
        }

        public ressources getRessource() {
            return type;
        }

        public void addAmount(float amount) {
            this.amount += amount;
        }

        public override string ToString() {
            return type + ": " + amount;
        }

        public void setAmount(float amount) {
            this.amount = amount;
        }

        public string getNice() {
            return type.ToString() + ": " + (int) amount;
        }

        public static string getNice(ressourceStack[] stacks) {
            string toReturn = "";

            bool firstElem = false;
            foreach (ressourceStack elem in stacks) {
                if (firstElem) {
                    toReturn += ", ";
                } else {
                    firstElem = true;
                }

                toReturn += elem.getNice();
            }

            return toReturn;
        }

        public static float[] getFloats(ressourceStack[] stacks) {
            float[] floats = new float[stacks.Length];
            for(int i = 0; i < stacks.Length; i++) {
                floats[i] = stacks[i].getAmount();
            }

            return floats;
        }
    }

    //edit in editor: ressource handler list length (both ressource display and ressource handler) in terrain + ui elem
    //also, add 2 entries in the resourceDisplay.cs script
    public enum ressources {Wood, Stone, Scrap, Trees, Iron, OreIron, Gold, OreGold, Iridium, OreIridium, Water, Electrum, Uranium, Thorium};

	// Use this for initialization
	void Start () {
		initialHP = this.HP;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.HP <= 0) {

            float size = this.gameObject.GetComponent<Collider>().bounds.size.magnitude;
            Debug.Log("destroying object: " + this.transform.gameObject.name + " size:" + size);
            var effect = GameObject.Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().destroyParticle, this.transform.position, this.transform.rotation);
            size = 0.2f + size * 0.1f;
            effect.transform.localScale = new Vector3(size, size, size);

            Destroy(this.transform.gameObject);
        }
	}

    public ressourceStack inflictDamage(float amount, ActionController attacker) {
        //Debug.Log("taking damage: " + amount + " hp: " + HP);

        float multiplier = 1f;
        if (this.GetComponent<harvestableRessource>() != null)
            multiplier = this.GetComponent<harvestableRessource>().getHarvestMultiplier();

        this.HP -= amount * multiplier;
        return getReturn(amount * multiplier);
    }

    private ressourceStack getReturn(float amount) {
        return new ressourceStack(amount, this.type);
    }

    public string niceText() {
        return this.type.ToString() + ": " + this.HP.ToString() +  "/" + this.initialHP.ToString();
    }

    public float getInitialHP() {
        return initialHP;
    }

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(HP, type, initialHP);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData) info;
        print("deserilazing HP handler..., HP=" + data.HP);
        this.HP = data.HP;
        this.initialHP = data.initialHP;
        this.type = data.type;
    }

    [System.Serializable]
    class serializationData : SaveLoad.SerializationInfo {
        public float HP;
        public ressources type;
        public float initialHP;

        public serializationData(float HP, ressources type, float initialHP) {
            this.HP = HP;
            this.type = type;
            this.initialHP = initialHP;
        }

        
        public override string scriptTarget {
            get {
                return "HPHandler";
            }
        }
    }
}
