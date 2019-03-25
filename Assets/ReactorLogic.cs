using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorLogic : MonoBehaviour {

    private List<GameObject> cores = new List<GameObject>();
    private List<GameObject> steamBoilers = new List<GameObject>();
    private List<HeatableStructure> allStructures = new List<HeatableStructure>();
    private bool active = false;

    public void activate() {
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
			} else if (item.gameObject.name.Contains("ReactorCore")) {
				elem = new reactorCore();
			} else if (item.gameObject.name.Contains("Wall")) {
				elem = new reactorWall();
			} else if (item.gameObject.name.Contains("Boiler")) {
				elem = new reactorBoiler();
			}

			if (elem == null) {
				continue;
			}

			elem.gameObject = item;
			elem.ownHeat = 0;
			allStructures.Add(elem);
			print("generated reactor data: " + elem.GetType());
        }
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

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {

    }

    private abstract class HeatableStructure {
        //all speeds are in seconds, time scaled
        public float ownHeat;
        public abstract float intakeAmount();
        public abstract float giveAmount();
        public abstract float disperseAmount();
        public GameObject gameObject;
        public List<HeatableStructure> connecteds = new List<HeatableStructure>();
    }

    private class reactorCore : HeatableStructure {
        //most efficient at 600 Degrees,  melts at 1000, giveAmount increases with own heat, using an environmental temp of 0 Degrees

        public override float disperseAmount() {
            return ownHeat / 100;
        }

        public override float giveAmount() {
            var x = ownHeat;
            var amount = 0.2280702f * x - 0.0002921053f * Mathf.Pow(x, 2) + 1.140351e-7 * Mathf.Pow(x, 3);
            return (float)amount;
        }

        public override float intakeAmount() {
            return 20f;
        }
    }

    private class heatReflector : HeatableStructure {

        public override float disperseAmount() {
            return ownHeat / 100;
        }

        public override float giveAmount() {
            var x = ownHeat;
            var amount = 125f - 0.265f * x + 0.00015 * Mathf.Pow(x, 2);
            return (float)amount;
        }

        public override float intakeAmount() {
            return -0.09f * ownHeat + 98.33333f;
        }
    }

    private class coolingGrid : HeatableStructure {

        public override float disperseAmount() {
            return ownHeat / 10;
        }

        public override float giveAmount() {
            return ownHeat / 500;
        }

        public override float intakeAmount() {
            return (float) (142.1663 - 0.3330368f * ownHeat + 0.0002017408f * Mathf.Pow(ownHeat, 2));
        }
    }

    private class reactorBoiler : HeatableStructure {

        public override float disperseAmount() {
            return 0.2105263f*ownHeat + 78.94737f;
        }

        public override float giveAmount() {
			return 0;
        }

        public override float intakeAmount() {
			return 400f;
        }
    }

    private class reactorWall : HeatableStructure {

        public override float disperseAmount() {
			return ownHeat / 100;
        }

        public override float giveAmount() {
			return ownHeat / 50;
        }

        public override float intakeAmount() {
			return 20f;
        }
    }
}
