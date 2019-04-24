using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HPHandler : MonoBehaviour, SaveLoad.SerializableInfo {
    
    public float HP = 100;
    public ressources type = ressources.Stone;
    public Faction faction = Faction.Terran;
    private float initialHP;
    
    public static Dictionary<Faction, List<GameObject>> factionMembers = new Dictionary<Faction, List<GameObject>>();

    public interface IDestroyAction {
        void beforeDestroy();
    }

    public enum Faction {
        Terran, Hostile, Alien, Neutral, Ally
    }

    // Use this for initialization
	void Start () {
		initialHP = this.HP;
        try {
            factionMembers[faction].Add(this.gameObject);
        }
        catch (KeyNotFoundException ex) {
            foreach (HPHandler.Faction item in Enum.GetValues(typeof(HPHandler.Faction))) {
                factionMembers[item] = new List<GameObject>();
            }
            factionMembers[faction].Add(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.HP <= 0) {

            float size = this.gameObject.GetComponent<Collider>().bounds.size.magnitude;
            Debug.Log("destroying object: " + this.transform.gameObject.name + " size:" + size);
            var effect = GameObject.Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().destroyParticle, this.transform.position, this.transform.rotation);
            size = 0.2f + size * 0.1f;
            effect.transform.localScale = new Vector3(size, size, size);

            if (this.GetComponent<IDestroyAction>() != null) {
                this.GetComponent<IDestroyAction>().beforeDestroy();
            }

            factionMembers[faction].Remove(this.gameObject);

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
