using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultStructure : MonoBehaviour, EnergyContainer, Structure, clickable, SaveLoad.SerializableInfo {

	public HPHandler.ressourceStack[] ownResource = new HPHandler.ressourceStack[2];
    public List<EnergyContainer> connections = new List<EnergyContainer>();
    public List<EnergyContainer> network = null;

    public Sprite infoBut;
    
    EnergyContainer recievedFromLast = null;
    List<GameObject> pipeObjects = new List<GameObject>();
    public bool busy = false;
    public int counter = 0;
    public float storedEnergy = 0;

    public bool salvaging = false;
    private float salvageStartHP = 100f;

    // Use this for initialization
    public virtual void Start () {
        reloadConnections();
	}

    public virtual void FixedUpdate() {

        if (this.salvaging && this.getHP().HP < 3) {
            print("structure salvaged!");
            var pickup = Instantiate(GameObject.Find("Terrain").GetComponent<Scene_Controller>().pickupBox, this.transform.position, Quaternion.identity);
            pickup.GetComponent<inventory>().add(new HPHandler.ressourceStack(salvageStartHP, HPHandler.ressources.Scrap));
            GameObject.Destroy(this.gameObject);
        }

        if (salvaging) {
            this.getHP().HP -= 2.5f;
            return;
        }

        counter++;
        if (counter >= 5000) {
            counter = 0;
            reloadConnections();
        }

        if (counter % 20 == 0) {
            handleConnections();
        }
        
    }

    public bool isWorking() {
        return !salvaging;
    }

    abstract public HPHandler.ressourceStack[] getCost();/* {
        HPHandler.ressourceStack[] cost = new HPHandler.ressourceStack[2];

        cost[0] = new HPHandler.ressourceStack(100, HPHandler.ressources.Wood);
        cost[1] = new HPHandler.ressourceStack(50, HPHandler.ressources.Stone);

        return cost;
    }*/

    public HPHandler.ressourceStack[] getResources() {
        return this.ownResource;
    }

    public HPHandler getHP() {
        return this.GetComponent<HPHandler>();
    }

    public inventory getInv() {
        return this.GetComponent<inventory>();
    }

    abstract public int getMaxEnergy();/* {
        return 2000;
    }*/

    public int getCurEnergy() {
        return (int) storedEnergy;
    }

    abstract public int getMaxOutput();

    abstract public int getMaxInput(); /* {
        if (this.getMaxEnergy() - this.getCurEnergy() < 50) {
            return this.getMaxEnergy() - this.getCurEnergy();
        }
        return 50;
    }*/

    public GameObject GetGameObject() {
        return this.gameObject;
    }

    public List<EnergyContainer> getConnections() {
        return connections;
    }

    public void reloadConnections() {
        Debug.Log("reloading Connections for: " + this.gameObject);

        List<GameObject> available = Piping.getInRange(20.0f, this.transform);

        clearPipes();

        //Debug.Log("available: " + available);
        sortListByPriority(available);

        foreach(GameObject energyTarget in available) {
            if (connections.Contains(energyTarget.GetComponent<EnergyContainer>()) || inNetwork(energyTarget.GetComponent<EnergyContainer>()) || energyTarget.GetComponent<Structure>().isSalvaging() ) {
                continue;
            } else {
                connections.Add(energyTarget.GetComponent<EnergyContainer>());
                energyTarget.GetComponent<EnergyContainer>().addConnection(this);
                Piping.createPipes(this.transform, energyTarget.transform, pipeObjects);

                if (network == null && energyTarget.GetComponent<EnergyContainer>().getNetwork() == null) {
                    createNetwork(energyTarget.GetComponent<EnergyContainer>());
                    energyTarget.GetComponent<EnergyContainer>().setNetwork(network);
                } else if (network == null) {
                    Debug.Log("found existing energy network, adding self");
                    this.setNetwork(energyTarget.GetComponent<EnergyContainer>().getNetwork());
                    //network.Add(this);
                } else {
                     energyTarget.GetComponent<EnergyContainer>().setNetwork(network);
                }
                

            }
        }

    }

    private void clearPipes() {
        //delete all existing pipes
        foreach (GameObject pipe in pipeObjects) {
            Debug.Log("deleting pipe: " + pipe);
            Destroy(pipe);
        }
        pipeObjects.Clear();
        connections.Clear();
        network = null;
    }

    private void sortListByPriority(List<GameObject> list) {

        for (int i = 0; i < list.Count; i++) {

            bool changed = false;
            for (int j = i; j < list.Count - 1; j++) {
                EnergyContainer contA = list[j].GetComponent<EnergyContainer>();
                EnergyContainer contB = list[j + 1].GetComponent<EnergyContainer>();
                if (contA == null || contB == null) {
                    continue;
                }

                if (Vector3.Distance(contB.GetGameObject().transform.position, this.transform.position) < Vector3.Distance(contA.GetGameObject().transform.position, this.transform.position)) {
                    changed = true;
                    list.Remove(list[j + 1]);
                    list.Insert(j, contB.GetGameObject());
                    Debug.Log("changed list position of " + contA.GetGameObject().name + " and " + contB.GetGameObject().name);
                    
                }
            }

            if (!changed) {
                break;
            }
        }
    }

    private bool inNetwork(EnergyContainer container) {
        
        if (network != null && network.Contains(container)) {
            return true;
        }

        return false;
    }

    private void createNetwork(EnergyContainer target) {
        Debug.Log("Creating new network!");
        this.network = new List<EnergyContainer>();
        if (!network.Contains(this))
            network.Add(this);
        //network.Add(target);
    }

    abstract public int getPriority();

    public void handleConnections() {
        /*foreach (EnergyContainer connection in connections) {
            if (connection.getMaxInput() < 1 || connection.Equals(recievedFromLast)) {
                continue;
            }

            if (connection.getCurEnergy() < this.getCurEnergy() || connection.getPriority() > this.getPriority()) {

                if (this.getCurEnergy() - connection.getCurEnergy() < this.getMaxOutput()) {
                    continue;
                }

                float transferAmount = (connection.getMaxInput() > this.getMaxOutput()) ? connection.getMaxInput() : this.getMaxOutput();
                if (transferAmount > this.getCurEnergy()) {
                    transferAmount = this.getCurEnergy();
                }

                connection.addEnergy(transferAmount, this);
                this.addEnergy(-transferAmount, this);
            }
        }*/
    }

    public void addConnection(EnergyContainer container) {
        this.connections.Add(container);
    }

    public void addEnergy(float amount, EnergyContainer from) {
        this.storedEnergy += amount;
        if (!from.Equals(this)) {
            recievedFromLast = from;
        }
    }

    public List<EnergyContainer> getNetwork() {
        return network;
    }

    public void setNetwork(List<EnergyContainer> network) {
        if (salvaging) return;
        this.network = network;
        //Debug.Log("got new network, adding self!");
        if (!network.Contains(this))
            network.Add(this);
        printList(network);
    }

    private static void printList(List<EnergyContainer> list) {
        string text = "Items in list: " + list.Count + " | ";
        foreach (EnergyContainer container in list) {
            text += container.ToString() + "; ";
        }
        Debug.Log(text);
    }

    public virtual void handleClick() {
        Debug.Log("clicked structure");

        if (salvaging)
            return;

        if (Salvaging.isActive()) {
            Salvaging.createNotification(this.gameObject);
            return;
        }

    }

    public void handleLongClick() {
        Debug.Log("long clicked structure");
        
        if (salvaging)
            return;

        this.GetComponent<ClickOptions>().Create(); //TODO
    }

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();
        
        controller.setTitle(this.gameObject.name);
        controller.setDesc(getDesc());
    }

    public virtual string getDesc() {
        return Environment.NewLine
            + "Kind: " + this.GetComponent<HPHandler>().niceText();

    }

    public abstract PopUpCanvas.popUpOption[] getOptions(); 
    /*{
         PopUpCanvas.popUpOption[] options;

         PopUpCanvas.popUpOption info = new  PopUpCanvas.popUpOption("Info", infoBut);
         //PopUpCanvas.popUpOption goTo = new  PopUpCanvas.popUpOption("DoClone", cloneBut);
        
        options = new PopUpCanvas.popUpOption[]{info};
        return options;
    }*/

    public abstract void handleOption(string option); 
    /*{
        Debug.Log("handling option: " + option);

        switch (option) {
            default:
                displayInfo();
                break;
        }
    }*/

    public new string ToString {
        get {
            return this.gameObject.name;
        }
    }

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(storedEnergy, busy, salvaging, ownResource);
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData) info;
        this.storedEnergy = data.storedEnergy;
        this.busy = data.busy;
        this.ownResource = data.ownResource;
        this.salvaging = data.salvaging;

        if (salvaging) {
            Debug.Log("got salvaging info, creating particles....");
            //TODO, this gets called before terrain scripts are loaded
            //GameObject.Instantiate(ParticleHelper.getInstance().salvageParticles, this.gameObject.transform);
        }

        //reloadConnections();
    }
    
    public GameObject getGameobject() {
        return this.gameObject;
    }

    public void salvage() {
        Debug.Log("Got salvage request!");
        this.salvaging = true;
        Salvaging.displayIndicator(this.gameObject);
        this.salvageStartHP = getHP().HP;
        this.network.Remove(this);
    }

    public bool isSalvaging() {
        return salvaging;
    }

    public bool isBusy() {
        return isSalvaging() || this.busy;
    }

    [System.Serializable]
    public class serializationData : SaveLoad.SerializationInfo {
        public float storedEnergy;
        public bool busy;
        public bool salvaging;
        public HPHandler.ressourceStack[] ownResource;

        public serializationData(float storedEnergy, bool busy, bool salvaging, HPHandler.ressourceStack[] ownResource) {
            this.storedEnergy = storedEnergy;
            this.busy = busy;
            this.ownResource = ownResource;
            this.salvaging = salvaging;
        }

        
        public override string scriptTarget {
            get {
                return "DefaultStructure";
            }
        }
    }
}
