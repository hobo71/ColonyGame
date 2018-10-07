using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piping {

    private static Transform wireParent = null;

	public static void createPipes(Transform from, Transform to, List<GameObject> created) {

        if (wireParent == null) {
            wireParent = GameObject.Find("Wires_Parent").transform;
        }

        Debug.Log("Creating pipes from " + from.transform.position + " to " + to.transform.position + " (" + from.gameObject.name + " to " + to.gameObject.name);
        List<Transform> fromTargets = new List<Transform>();
        float minDist = float.MaxValue;
        Transform fromPoint = null;
        Transform toPoint = null;

        foreach (Transform child in from) {
            if (!child.gameObject.name.Contains("ConnectionBegin")) {
                continue;
            }

            foreach(Transform childTarget in to) {
                
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

        Vector3 spawnAt = fromPoint.position;
        Vector3 dir = toPoint.position - spawnAt;
        Quaternion rotation = Quaternion.LookRotation(dir);

        int pipes = (int) minDist;
        float lastScale = 1f + minDist - pipes;
        Debug.Log("Pipes needed: " + pipes + " lastScale " + lastScale + " spawnAt=" + spawnAt + "wireParent=" + wireParent);

        Transform firstPipe = null;

        for (int i = 0; i < pipes; i++) {
            GameObject pipe;
            if (firstPipe == null) {
                pipe = GameObject.Instantiate(Scene_Controller.pipeMiddle, spawnAt, rotation, wireParent);

                firstPipe = pipe.transform;
            } else {
                pipe = GameObject.Instantiate(Scene_Controller.pipeMiddle, spawnAt, rotation, firstPipe);
            }

            if (i == pipes - 1) {
                pipe.transform.localScale = new Vector3(lastScale, 1.0f, 1.0f);
            }
                
            pipe.transform.Rotate(0f, 90f, 0f);
            created.Add(firstPipe.gameObject);
            
            spawnAt += dir.normalized;
        }
        
    }

    public static List<GameObject> getInRange(float range, Transform origin) {
        Collider[] hitColliders = Physics.OverlapSphere(origin.position, range);
        List<GameObject> elements = new List<GameObject>();

        foreach (Collider collider in hitColliders) {
            if (collider.gameObject.GetComponent<EnergyContainer>() != null && !collider.gameObject.transform.Equals(origin)) {
                Debug.Log("Found energy container in range: " + collider.gameObject);
                elements.Add(collider.gameObject);
            }
        }

        return elements;
    }
}
