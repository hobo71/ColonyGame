using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorLogic : MonoBehaviour {

    public GameObject connectionPrefab;
    private List<GameObject> linesDrawn = new List<GameObject>();
    private List<HeatableStructure> allStructures = new List<HeatableStructure>();
    private bool active = false;

    public void activate() {

        active = true;

        print("activating reactor, searching all belonging reactor parts!");
        //find all structures that belong to the reactor
        var all = GameObject.FindGameObjectsWithTag("reactorPart");
        var found = new List<GameObject>();
        found.Add(this.gameObject);

        //x iterations -> max number of buildings to go get linked through
        for (int i = 0; i < 6; i++) {
            addToReactorList(all, found);
        }
        allStructures.Clear();

        print("got list! length: " + found.Count);
        foreach (var item in found) {
            HeatableStructure elem = null;
            if (item.gameObject.name.Contains("Cooling")) {
                elem = new coolingGrid();
            } else if (item.gameObject.name.Contains("HeatReflector")) {
                elem = new heatReflector();
            } else if (item.gameObject.name.Contains("Core")) {
                elem = new reactorCore();
            } else if (item.gameObject.name.Contains("Wall")) {
                elem = new reactorWall();
            } else if (!item.gameObject.name.Contains("Controller") && !item.gameObject.name.Contains("Boiler")) {
                elem = new HeatableStructure();
                //not used atm
            }

            if (elem == null) {
                continue;
            }

            elem.gameObject = item;
            elem.temperature = 0;
            allStructures.Add(elem);
            elem.gameObject.GetComponent<reactorPart.IHeatableElement>().setData(elem);
            print("generated reactor data: " + elem.GetType());
        }

        //find and display connections for all reactor parts within 5 range
        foreach (var origin in allStructures) {
            foreach (var target in allStructures) {
                //skip self
                if (target.Equals(origin) || origin.gameObject.name.Contains("Controller") || origin.connecteds.Contains(target)) {
                    continue;
                }

                var origColl = origin.gameObject.GetComponent<Collider>();
                var tarColl = target.gameObject.GetComponent<Collider>();

                var sampleOrig = origColl.ClosestPoint(target.gameObject.transform.position);
                var sampleTar = tarColl.ClosestPoint(origin.gameObject.transform.position);

                //check if close enough
                if (Vector3.Distance(sampleOrig, sampleTar) < 2.5) {
                    origin.connecteds.Add(target);
                    target.connecteds.Add(origin);
                    print("found connection between: " + origin.gameObject.name + " and " + target.gameObject.name);
                    drawConnection(sampleOrig, sampleTar, origin.gameObject);
                }
            }
        }
    }

    public void deactivate() {
        foreach (var item in linesDrawn) {
            GameObject.Destroy(item);
        }
        linesDrawn.Clear();
        allStructures.Clear();
        active = false;
    }

    private void drawConnection(Vector3 origin, Vector3 target, GameObject parent) {
        var line = GameObject.Instantiate(connectionPrefab, origin, Quaternion.identity, parent.transform);
        var lineC = line.GetComponent<LineRenderer>();

        var dir = target - origin;
        dir.Normalize();

        lineC.SetPosition(0, origin + new Vector3(0, 1f, 0) - dir);
        lineC.SetPosition(1, target + new Vector3(0, 1f, 0) + dir);
        linesDrawn.Add(line);

    }

    private void addToReactorList(GameObject[] all, List<GameObject> found) {

        foreach (var item in all) {
            //ignore already found objects
            if (found.Contains(item)) {
                continue;
            }

            //loop through already found items
            foreach (var elem in found) {
                if (Vector3.Distance(elem.transform.position, item.transform.position) < 8) {
                    found.Add(item);
                    break;
                }
            }
        }
    }

    void FixedUpdate() {
        if (!active)
            return;

        //update heat items
        foreach (var item in allStructures) {
            item.update();
        }
    }

    public class HeatableStructure {
        //all speeds are in seconds, time scaled
        public float temperature;
        public virtual float getExchangeBonus() {
            return 0f;
        }
        public virtual float getDisperseBonus() {
            return 0f;
        }
        public GameObject gameObject;
        public List<HeatableStructure> connecteds = new List<HeatableStructure>();
        public virtual void update() {

            this.temperature -= 2f * Time.deltaTime * (1 + getDisperseBonus());

            if (this.temperature < 0) {
                this.temperature = 0f;
            }

            //exchange heat based on deltas with connected things
            foreach (var item in connecteds) {

                var delta = this.temperature - item.temperature;
                var change = delta / 15;
                change *= Time.deltaTime * (1 + this.getExchangeBonus() + item.getExchangeBonus());

                this.temperature -= change;
                //ignore target, since it'll also get updated
                //item.temperature += change;                

            }

            //print(this.temperature + " of " + this.gameObject.name);
        }
    }

    private class coolingGrid : HeatableStructure {
        //cooling Grid have 1 extra exchange with the "air" at 0 degrees
        //cooling grid will be counted by the steam boiler -> every cooled degree will be given to the connected boiler

        public SteamBoiler connectedBoiler = null;
        public override void update() {

            if (connectedBoiler == null) {
                var all = GameObject.FindObjectsOfType<SteamBoiler>();
                var closestDist = 1000f;
                SteamBoiler closest = null;
                foreach (var item in all) {
                    var dist = Vector3.Distance(this.gameObject.transform.position, item.gameObject.transform.position);
                    if (dist < closestDist) {
                        closestDist = dist;
                        closest = item;
                    }
                }

                connectedBoiler = closest;
            }

            var delta = this.temperature - 0;
            var change = delta / 25;
            change *= Time.deltaTime * (1 + this.getExchangeBonus() + 0);

            this.temperature -= change;
            connectedBoiler.addSteam(change);
            base.update();
        }
    }


    private class heatReflector : HeatableStructure {
        public override float getExchangeBonus() {
            return 0.5f;
        }
    }

    private class reactorWall : HeatableStructure {
        public override float getExchangeBonus() {
            return -0.9f;
        }
    }

    private class reactorCore : HeatableStructure {
        private bool inited = false;
        private float generateMultiplier = 1f;
        private void init() {
            foreach (var item in connecteds) {
                if (item.GetType().Equals(typeof(heatReflector))) {
                    generateMultiplier *= 1.25f;
                } else if (item.GetType().Equals(typeof(reactorCore))) {
                    generateMultiplier *= 1.75f;
                }
            }

            inited = true;
            print("final multiplier for reactor: " + generateMultiplier);
        }
        public override void update() {
            if (!inited) {
                init();
            }
            this.temperature += 10f * Time.deltaTime * (generateMultiplier);
            base.update();
        }
    }
}
