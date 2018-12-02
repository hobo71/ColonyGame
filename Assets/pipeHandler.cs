using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipeHandler : MonoBehaviour, clickable, SaveLoad.SerializableInfo {

    public Sprite infoBut;
    public ConveyorCreator.conveyorConnection data;
    public GameObject ConveyorBegin;
    private serializationData loaded = null;

    public void displayInfo() {
        InfoClicked controller = InfoClicked.getInstance();
        controller.show();

        controller.setTitle(this.gameObject.name);
        controller.setDesc("Used to move items between structures");
    }
    
    public void salvage() {
        Debug.Log("Got salvage request!");
        foreach (var obj in data.createdObjs) {
            GameObject.Destroy(obj);
        }
    }

    public PopUpCanvas.popUpOption[] getOptions() {
        PopUpCanvas.popUpOption[] options;

        PopUpCanvas.popUpOption conf = new PopUpCanvas.popUpOption("conf", infoBut);

        options = new PopUpCanvas.popUpOption[] {conf};
        return options;
    }

    public void handleLongClick() {
        this.GetComponent<ClickOptions>().Create();
        print("data: " + this.data);
    }

    public void handleOption(string option) {

        Debug.Log("handling option: " + option);

        switch (option) {
            case "conf":
                doConf();
                break;
            default:
                displayInfo();
                break;
        }
    }

    private void doConf() {
        Scene_Controller.getInstance().conveyorConfigurator.SetActive(true);
        Scene_Controller.getInstance().conveyorConfigurator.GetComponent<ConveyorConfigurator>().setInstance(this);
        clickDetector.menusOpened++;
        //TODO
    }

    void FixedUpdate() {

        var data = this.getData();

        if (data.from == null || data.to == null) {
            print("from or to gameobj is missing, attempting to restore from save");
            if (loaded != null) {
                var from = findClosestConnection(loaded.from).transform.parent.gameObject;
                var to = findClosestConnection(loaded.to).transform.parent.gameObject;
                data.from = from;
                data.to = to;
                print("found: " + from + " to=" + to);
                return;
            }

            print("unable to load restored configs, deleting...");
            //salvage();
            return;
        }

        //all from origin to target
        if (data.drainAllLeft) {
            data.from.GetComponent<inventory>().transferAllSafe(data.to.GetComponent<inventory>());
        }

        //all from target to origin
        if (data.drainAllRight) {
            data.to.GetComponent<inventory>().transferAllSafe(data.from.GetComponent<inventory>());
        }

        if (data.drainLeft) {
            foreach (var elem in data.drainingLeft) {
                data.from.GetComponent<inventory>().transferTo(data.to.GetComponent<inventory>(), elem, data.from.GetComponent<inventory>().getAmount(elem));
            }
        }
        
        if (data.drainRight) {
            foreach (var elem in data.drainingRight) {
                data.to.GetComponent<inventory>().transferTo(data.from.GetComponent<inventory>(), elem, data.to.GetComponent<inventory>().getAmount(elem));
            }
        }
    }

    public void setData(ConveyorCreator.conveyorConnection data) {
        this.data = data;
        print("connection got data: " + data.from + " -> " + data.to);
    }

    public ConveyorCreator.conveyorConnection getData() {
        return data;
    }

    public void handleClick() {
        
        if (Salvaging.isActive()) {
            salvage();
            return;
        }

    }

    

    public SaveLoad.SerializationInfo getSerialize() {
        return new serializationData(this.getData());
    }

    public void handleDeserialization(SaveLoad.SerializationInfo info) {
        serializationData data = (serializationData) info;
        
        //create new data
        var from = findClosestConnection(data.from).transform.parent.gameObject;
        var to = findClosestConnection(data.to).transform.parent.gameObject;
        var connection = data.connection.getOriginal(from, to);
        print("deserialized data: " + connection);
        this.setData(connection);
        this.loaded = data;

        //create conveyor boxes at end
        var bBox = GameObject.Instantiate(ConveyorBegin, data.from, ConveyorBegin.transform.rotation);
        var eBox = GameObject.Instantiate(ConveyorBegin, data.to, ConveyorBegin.transform.rotation);
        connection.createdObjs.Add(bBox);
        connection.createdObjs.Add(eBox);
        connection.createdObjs.Add(this.gameObject);
        
    }

    private GameObject findClosestConnection(Vector3 pos) {
        float minDist = float.MaxValue;
        var objs = GameObject.FindGameObjectsWithTag("connectionPoint");
        print("searching for closest connection for conveyor!");

        GameObject res = null;
        foreach (var obj in objs) {
            var dist = Vector3.Distance(obj.transform.position, pos);
            if (dist < minDist) {
                minDist = dist;
                res = obj;
            }
        }

        if (res == null) {
            print("no connectionPoint found!");
        }

        print("found res: " + res + " at pos: " + pos);

        return res;
    }
    

    [System.Serializable]
    public class serializationData : SaveLoad.SerializationInfo {
        public conveyorConnectionSave connection;
        public SaveLoad.SerializableVector3 from;
        public SaveLoad.SerializableVector3 to;

        public serializationData(ConveyorCreator.conveyorConnection connection) {
            this.connection = new conveyorConnectionSave(connection);
            this.from = connection.createdObjs[0].transform.position;
            this.to = connection.createdObjs[1].transform.position;
        }

        
        public override string scriptTarget {
            get {
                return "pipeHandler";
            }
        }


    }

    [System.Serializable]
    public class conveyorConnectionSave {
        public List<HPHandler.ressources> drainingLeft = new List<HPHandler.ressources>();
        public List<HPHandler.ressources> drainingRight = new List<HPHandler.ressources>();
		public bool drainAllLeft;
		public bool drainAllRight;
		public bool drainLeft;
		public bool drainRight;

        public conveyorConnectionSave(ConveyorCreator.conveyorConnection conn) {
            drainAllLeft = conn.drainAllLeft;
            drainAllRight = conn.drainAllRight;
            drainingLeft = conn.drainingLeft;
            drainingRight = conn.drainingRight;
            drainLeft = conn.drainLeft;
            drainRight = conn.drainRight;
        }

        public ConveyorCreator.conveyorConnection getOriginal(GameObject from, GameObject to) {
            var res =  new ConveyorCreator.conveyorConnection(from, to, new List<GameObject>());
            res.drainAllLeft = drainAllLeft;
            res.drainAllRight = drainAllRight;
            res.drainingLeft = drainingLeft;
            res.drainingRight = drainingRight;
            res.drainLeft = drainLeft;
            res.drainRight = drainRight;
            return res;
        }
    }
}
