using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Salvaging : MonoBehaviour {
    
    public static Sprite availSprites;
    public Sprite assignSprites;
    public GameObject assignCanvas;
    public static GameObject canvas;
    public GameObject assignImage;
    public static GameObject image;
    public GameObject salvageBut;

    private static GameObject curTarget = null;
    private GameObject meshBuilder;

    private static List<GameObject> popUps = new List<GameObject>();
    private static bool salvageMode = false;

	// Use this for initialization
	void Start () {
		meshBuilder = GameObject.Find("NavController");
		availSprites = assignSprites;
        canvas = assignCanvas;
        image = assignImage;

	}
	
	// Update is called once per frame
	void Update () {
		
        if (!salvageMode) {
            return;
        }

        float scale = 1 / (TouchCamera.getHeight() / 10);
        meshBuilder.GetComponent<MeshRenderer>().materials[0].SetTextureScale("_MainTex", new Vector2(scale, scale));

	}

    public void startSalvageMode() {

        this.gameObject.GetComponent<Scene_Controller>().hideAllUI();
        if (salvageBut == null) {
            Debug.Log("salvage but doesnt exist!");
            salvageBut = GameObject.Find("Canvas").transform.Find("SalvageDone").gameObject;
        }
        this.salvageBut.SetActive(true);

        if (salvageMode) {
            return;
        }
        Debug.Log("Starting Salvage Mode!");
        salvageMode = true;
        clickDetector.clearPopUps();
        //GameObject.Find("Terrain").GetComponent<Scene_Controller>().buildButton.SetActive(true);

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        
        meshBuilder.GetComponent<MeshRenderer>().enabled = true;
        //meshBuilder.AddComponent<MeshFilter>();
        //meshBuilder.AddComponent<MeshRenderer>();
        Mesh mesh = meshBuilder.GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        mesh.vertices = triangulation.vertices;

        mesh.triangles = triangulation.indices;
        Vector2[] uvs = new Vector2[triangulation.vertices.Length];

        for (int i = 0; i < uvs.Length; i++) {
            uvs[i] = new Vector2(triangulation.vertices[i].x, triangulation.vertices[i].z);
        }
        mesh.uv = uvs;
        meshBuilder.GetComponent<MeshRenderer>().enabled = true;
        meshBuilder.GetComponent<MeshCollider>().sharedMesh = mesh;

        Time.timeScale = 0.1f;
    }

    public static bool isActive() {
        return salvageMode;
    }

    public static void createNotification(GameObject on) {

        Debug.Log("Creating Click options for: " + on.name);
        clearPopUps();
        curTarget = on;

        GameObject cam = Camera.main.gameObject;

        Vector3 createAt = on.transform.position;
        Vector3 above = createAt;
        above.y += 30;

        RaycastHit hit;
        Physics.Raycast(above, createAt - above, out hit, 30.0f);
        Debug.Log("Top of element is at: " + hit.point);

        Vector3 camUp = cam.transform.up;
        camUp.Normalize();
        camUp *= 2.0f;

        createAt = hit.point + camUp;

        GameObject parent = GameObject.Instantiate(canvas, createAt, cam.transform.rotation);
        popUps.Add(parent);

        GameObject iconCreated = GameObject.Instantiate(image, parent.transform);
        iconCreated.GetComponent<UnityEngine.UI.Image>().sprite = availSprites;
        iconCreated.transform.localPosition = Vector3.zero;
    }

    public static void clearPopUps() {
        foreach (GameObject obj in popUps) {
            GameObject.Destroy(obj);
        }
    }

    public static void salvageTriggered() {
        clearPopUps();
        try {
            curTarget.GetComponent<Structure>().salvage();
        } catch (NullReferenceException ex) {
            if (curTarget.GetComponent("building_marker") != null) {
                GameObject.Destroy(curTarget);
            }
        }
        
    }

    public void SalvageDone() {

        salvageMode = false;
        Time.timeScale = 1.0f;

        this.salvageBut.SetActive(false);
        this.gameObject.GetComponent<Scene_Controller>().restoreDefaultUI();
        meshBuilder.GetComponent<MeshRenderer>().enabled = false;
        clearPopUps();
    }

    public static void displayIndicator(GameObject obj) {
        Vector3 rayStart = obj.transform.position + new Vector3(0, 80f, 0);
        RaycastHit hit;
        Physics.Raycast(rayStart, Vector3.down, out hit, 80f);
       
        var part = GameObject.Instantiate(ParticleHelper.getInstance().salvageParticles, hit.point, Quaternion.identity, obj.transform);
        float totalTime = obj.GetComponent<Structure>().getHP().HP;
        totalTime /= 2.5f;
        Debug.Log("Total time of salvage particle: " + totalTime);
        part.GetComponent<ParticleUVOffset>().totalTime = totalTime;

    }

}
