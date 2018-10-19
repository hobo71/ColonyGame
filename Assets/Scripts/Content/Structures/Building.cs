using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour {

    public static bool buildingMode = false;
    public GameObject baseBuilding;
    public GameObject baseBuildingReal;
    public GameObject solarpanel;
    public GameObject solarpanelReal;
    public GameObject recycler;
    public GameObject recyclerReal;
    public GameObject dome_basic;
    public GameObject dome_basicReal;
    public GameObject drillBasicPlacement;
    public GameObject drillBasicReal;
    public GameObject scrapBurnerReal;
    public GameObject scrapBurnerPlacement;
    public GameObject buildButton;
    public GameObject buildingMarker;

    private GameObject currentlyBuilding = null;
    private bool onUI = false;
    private GameObject holoPlacement = null;
    public GameObject rotationbar;
    private Quaternion initialRotation = Quaternion.identity;
    private int barWidth;
    private int barHeight;

    private static float deviceWidth = 0;
    private static float deviceHeight = 0;

    // Use this for initialization
    void Start() {
        barWidth = (int) rotationbar.GetComponent<RectTransform>().sizeDelta.x * 2;
        barHeight = (int) rotationbar.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update() {

        /*if ((((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended)) || Input.GetMouseButtonUp(0)) && clickDetector.overlayClicked) {
            clickDetector.overlayClicked = false;
        }*/

        if (!buildingMode || Camera.main.GetComponent<TouchCamera>().zoomActive/* || onUI*/) {
            return;
        }

        //check if on slider

        var inputX = Input.mousePosition.x;
        var inputY = Input.mousePosition.y;
        if (Input.touchSupported) {
            inputX = Input.GetTouch(0).position.x;
            inputY = Input.GetTouch(0).position.y;
        }

        print("inputX=" + inputX + " inputY=" + inputY + " barPos=" + rotationbar.transform.position + " barWidth=" + barWidth);

        var grow = 15;
        if (inputX >= rotationbar.transform.position.x - grow && inputX < rotationbar.transform.position.x + barWidth + grow) {    //x matching
            if (inputY >= rotationbar.transform.position.y - barHeight * 2 - grow && inputY < rotationbar.transform.position.y + grow)
                return;

        }

        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Application.platform != RuntimePlatform.Android) {
            handleEdgeMove(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }

        if (Input.touchCount > 0) {
            raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            handleEdgeMove(new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
        }

        int layerMask = LayerMask.GetMask("PlacementMask");
        RaycastHit raycastHit;
        bool valid = Physics.Raycast(raycast, out raycastHit, 200.0f/*, layerMask*/);

        Vector3 origPos = holoPlacement.transform.position;
        Vector3 early = raycastHit.point;
        //early.y -= 1.0f;
        holoPlacement.transform.position = early;


        Transform[] samples = holoPlacement.GetComponentsInChildren<Transform>();

        NavMeshHit hit;
        bool canBuild = true;

        if (valid) {
            foreach (Transform sample in samples) {
                if (!sample.gameObject.name.Contains("GameObject")) continue;   //all markers are named gameobject (x)
                int lMask = LayerMask.GetMask("PlacementMask");
                bool isValid = Physics.Raycast(sample.position + Vector3.up * 1, Vector3.down, 1.8f, lMask);
                //bool isValid = NavMesh.SamplePosition(sample.transform.position, out hit, 0.5f, NavMesh.AllAreas);
                if (!isValid) {
                    canBuild = false;
                }
            }
        }

        if (!valid) {
            holoPlacement.transform.position = origPos;
            canBuild = false;
        }

        var renderers = holoPlacement.GetComponentsInChildren<MeshRenderer>();

        if (holoPlacement.GetComponent<MeshRenderer>() != null) {
            Array.Resize(ref renderers, renderers.Length);
            renderers[renderers.Length - 1] = holoPlacement.GetComponent<MeshRenderer>();
        }

        foreach (MeshRenderer renderer in renderers) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                if (canBuild) {
                    renderer.materials[i].SetColor("_Color", Color.blue);
                    renderer.materials[i].SetColor("Rim Color", Color.blue);
                    renderer.materials[i].SetColor("Emission Color", new Color(0, 11, 9, 68));
                } else {
                    renderer.materials[i].SetColor("_Color", Color.red);
                    renderer.materials[i].SetColor("Rim Color", Color.red);
                    renderer.materials[i].SetColor("Emission Color", new Color(40, 5, 5, 68));
                }
            }
        }


        buildButton.SetActive(canBuild);

    }

    private void startbuildingMode() {
        buildingMode = true;
        rotationbar.SetActive(true);
        clickDetector.clearPopUps();
        GameObject.Find("Terrain").GetComponent<Scene_Controller>().buildButton.SetActive(true);

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        GameObject meshBuilder = GameObject.Find("NavController");
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

    private void stopBuildingMode() {
        Debug.Log("stopping building mode");
        buildingMode = false;
        rotationbar.SetActive(false);

        GameObject.Destroy(holoPlacement);
        holoPlacement = null;

        GameObject meshBuilder = GameObject.Find("NavController");
        meshBuilder.GetComponent<MeshRenderer>().enabled = false;
        Time.timeScale = 1.0f;
    }

    public void buildClicked(BuildingManager.structureData data) {
        if (!buildingMode) {
            startbuildingMode();
        }

        GameObject.Destroy(holoPlacement);
        holoPlacement = null;
        RaycastHit raycastHit;
        int layerMask = LayerMask.GetMask("PlacementMask");
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, 200.0f, layerMask);
        Vector3 startPos = raycastHit.point;
        startPos.y -= 1.0f;

        holoPlacement = GameObject.Instantiate(data.placement, startPos, data.placement.transform.rotation);
        currentlyBuilding = data.prefab;

        initialRotation = holoPlacement.transform.rotation;

        Debug.Log("placing holo for " + name + " at" + startPos + " hitinfo: " + raycastHit);

    }

    public void rotateChanged(float value) {
        var rot = initialRotation * Quaternion.Euler(Vector3.forward * (value - 0.5f) * 360);
        holoPlacement.transform.rotation = rot;

    }

    public void closeClicked() {
        stopBuildingMode();
    }

    public void buildNow() {
        Debug.Log("creating beacon...");
        Vector3 finalPos = holoPlacement.transform.position;
        finalPos.y -= 1.0f;
        Quaternion rotation = holoPlacement.transform.rotation;
        if (buildingMode)
            stopBuildingMode();

        //Quaternion rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        GameObject beacon = GameObject.Instantiate(buildingMarker, finalPos, rotation);

        beacon.GetComponent<MeshCollider>().sharedMesh = currentlyBuilding.GetComponent<MeshFilter>().sharedMesh;
        beacon.GetComponent<MeshFilter>().sharedMesh = currentlyBuilding.GetComponent<MeshFilter>().sharedMesh;

        beacon.GetComponent<building_marker>().buildTo = currentlyBuilding;
    }

    public void onGui(bool state) {
        this.onUI = state;
    }

    private void handleEdgeMove(Vector2 pos) {

        if (deviceHeight == 0) deviceHeight = Screen.height;
        if (deviceWidth == 0) deviceWidth = Screen.width;

        Vector3 move = Vector3.zero;

        //left
        if (pos.x / deviceWidth < 0.15f) move.x -= 0.05f;
        if (pos.x / deviceWidth > 0.85f) move.x += 0.05f;
        if (pos.y / deviceHeight < 0.15) move.y -= 0.05f;
        if (pos.y / deviceHeight > 0.85f) move.y += 0.05f;

        move = GameObject.Find("Main Camera").transform.rotation * move;
        move.y = 0;

        GameObject.Find("Main Camera").transform.localPosition += move;

    }
}
