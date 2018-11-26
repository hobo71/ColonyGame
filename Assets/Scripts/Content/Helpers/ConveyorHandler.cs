using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorHandler : MonoBehaviour {

    public GameObject ConveyorBegin;
    public GameObject ConveyorPipe;
    public GameObject cancelBut;

    private static ConveyorHandler instance;
    private GameObject startObj = null;
    private GameObject endObj = null;
    private Outline start = null;

    // Use this for initialization
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {

    }

    public static ConveyorHandler getInstance() {
        return instance;
    }

    public void onConveyorButtonClicked() {
        print("pressed conveyor button");
        GameObject.Find("Main Camera").GetComponent<clickDetector>().setNextClickAction(onBuildingClick);
        Time.timeScale = 0.1f;
        Scene_Controller.getInstance().hideAllUI();
        cancelBut.SetActive(true);
    }

    private void onBuildingClick(GameObject target) {
        print("got next click on: " + target);
        if (target.GetComponent<DefaultStructure>() == null) {
            Notification.createNotification(target, Notification.sprites.Stopping, "Invalid", Color.red, false);
            GameObject.Find("Main Camera").GetComponent<clickDetector>().setNextClickAction(onBuildingClick);
            return;
        }

        GameObject.DestroyImmediate(target.GetComponent<Outline>());
        Outline effect = target.AddComponent<Outline>() as Outline;
        effect.OutlineColor = Color.magenta;
        effect.OutlineWidth = 8.0f;
        start = effect;

        startObj = target;
        GameObject.Find("Main Camera").GetComponent<clickDetector>().setNextClickAction(onEndBuildingClick);
    }

    private void onEndBuildingClick(GameObject target) {
        print("got End click on: " + target);
        if (target.GetComponent<DefaultStructure>() == null) {
            Notification.createNotification(target, Notification.sprites.Stopping, "Invalid", Color.red, false);
            GameObject.Find("Main Camera").GetComponent<clickDetector>().setNextClickAction(onEndBuildingClick);
            return;
        }

        endObj = target;

        var from = startObj;
        var to = endObj;

        Debug.Log("Creating pipes from " + from.transform.position + " to " + to.transform.position + " (" + from.gameObject.name + " to " + to.gameObject.name);
        float minDist = float.MaxValue;
        Transform fromPoint = null;
        Transform toPoint = null;

        foreach (Transform child in from.transform) {
            if (!child.gameObject.name.Contains("ConnectionBegin")) {
                continue;
            }

            foreach (Transform childTarget in to.transform) {

                if (!childTarget.gameObject.name.Contains("ConnectionBegin")) {
                    continue;
                }

                float dist = Vector3.Distance(child.position, childTarget.position);
                if (dist < minDist) {
                    minDist = dist;
                    fromPoint = child;
                    toPoint = childTarget;
                }
            }
        }

        Debug.Log("found closest Points between: from " + fromPoint.gameObject.name + " to " + toPoint.gameObject.name + " spawnAt=" + fromPoint);

        var stuff = new List<GameObject>();
        var bBox = GameObject.Instantiate(ConveyorBegin, fromPoint.position + new Vector3(0, -1, 0), ConveyorBegin.transform.rotation);
        var eBox = GameObject.Instantiate(ConveyorBegin, toPoint.position + new Vector3(0, -1, 0), ConveyorBegin.transform.rotation, bBox.transform);
        stuff.Add(bBox);
        stuff.Add(eBox);

        var dir = toPoint.position - fromPoint.position;
        Quaternion rotation = Quaternion.LookRotation(dir);

        var pipe = GameObject.Instantiate(ConveyorPipe, fromPoint.position + new Vector3(0, 1, 0), rotation, bBox.transform);
        pipe.transform.localScale = new Vector3(1, 1, minDist);

        var connection = new conveyorConnection(startObj, endObj, stuff);
        pipe.GetComponent<pipeHandler>().setData(connection);

        cancel();

    }

    public void cancel() {
        GameObject.Destroy(start);
        startObj = null;
        endObj = null;
        Time.timeScale = 1f;
        Scene_Controller.getInstance().restoreDefaultUI();
        Scene_Controller.getInstance().handleAny();
        cancelBut.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<clickDetector>().resetNextClick();
    }

    public class conveyorConnection {
        public GameObject from;
        public GameObject to;
        public List<GameObject> createdObjs = new List<GameObject>();
        public List<HPHandler.ressources> drainingLeft = new List<HPHandler.ressources>();
        public List<HPHandler.ressources> drainingRight = new List<HPHandler.ressources>();
		public bool drainAllLeft;
		public bool drainAllRight;
		public bool drainLeft;
		public bool drainRight;

        public conveyorConnection(GameObject from, GameObject to, List<GameObject> stuff) {
            this.from = from;
            this.to = to;
            this.createdObjs = stuff;
        }
    }
}
