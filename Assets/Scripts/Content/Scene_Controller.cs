using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Scene_Controller : MonoBehaviour {

    public GameObject buildingMenu;
    public GameObject openBuildingMenu;
    public GameObject buildButton;
    public GameObject buildPreview;
    public GameObject PipeMiddle;
    public static GameObject pipeMiddle;
    public static string saveName = "default";
    public GameObject menuBut;
    public GameObject resourcesMenu;
    public GameObject pickupBox;
    public GameObject harvestParticle;
    
    void Awake() {
        pipeMiddle = PipeMiddle;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
    }
	// Use this for initialization
	void Start () {
        pipeMiddle = PipeMiddle;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void handleAny() {
        clickDetector.overlayClicked = true;
    }

    public void handleBuildClick() {
        handleAny();
        Debug.Log("Clicked on build");
        showMenu(true);
    }

    public void buildingAClicked() {
        handleAny();
        Debug.Log("Clicked on building A");

        InfoClicked preview = buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle("Test Building", "base");
        preview.setDesc("Just some random building to test the system");
        float[] price = new float[]{100f, 50f, 0.0f, 0.0f};
        preview.setPrice("Cost: 100 Wood, 50 Stone");
        preview.setPrice(price);

        //GameObject.Find("Terrain").GetComponent<Building>().buildClicked("base");
        //buildButton.SetActive(true);
    }

    public void buildingBClicked() {
        handleAny();
        Debug.Log("Clicked on building B");

        InfoClicked preview = buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle("Solar Panel", "solarpanel");
        preview.setDesc("A solar panel to generate energy");
        preview.setPrice("Price: " + HPHandler.ressourceStack.getNice(Solar_Panel.getPrice()));
        preview.setPrice(HPHandler.ressourceStack.getFloats(Solar_Panel.getPrice()));
    }

    public void buildingCClicked() {
        handleAny();
        Debug.Log("Clicked on building C");

        InfoClicked preview = buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle("Basic Dome", "dome_basic");
        preview.setDesc("A small, very basic Dome that can store some energy");
        preview.setPrice("Price: " + HPHandler.ressourceStack.getNice(Dome_Basic.getPrice()));
        preview.setPrice(HPHandler.ressourceStack.getFloats(Dome_Basic.getPrice()));
    }

    public void buildingDClicked() {
        handleAny();
        Debug.Log("Clicked on building D");

        InfoClicked preview = buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle("Worker Salvager", "recycler");
        preview.setDesc("A way to get rid of workers you do not need anymore. Generates a small amount of scrap for each worker");
        preview.setPrice("Price: " + HPHandler.ressourceStack.getNice(WorkerRecycler.getPrice()));
        preview.setPrice(HPHandler.ressourceStack.getFloats(WorkerRecycler.getPrice()));
    }

    //DrillPlatformBasic
    public void buildingEClicked() {
        handleAny();
        Debug.Log("Clicked on building E");

        InfoClicked preview = buildPreview.GetComponent<InfoClicked>();
        preview.show();
        preview.setTitle("Basic Drill Platform", "drillPlatformBasic");
        preview.setDesc("A basic Drill dhat can automatically harvest nearby stones, ores and minerals (can also be focused by selected harvestable things)");
        preview.setPrice("Price: " + HPHandler.ressourceStack.getNice(DrillPlatformBasic.getPrice()));
        preview.setPrice(HPHandler.ressourceStack.getFloats(DrillPlatformBasic.getPrice()));
    }


    public void salvageClicked() {
        Debug.Log("Clicked Salvage");
        handleAny();
        this.gameObject.GetComponent<Salvaging>().startSalvageMode();
    }

    public void handleMenuClose() {
        handleAny();
        Debug.Log("closing building menu");
        showMenu(false);
        GameObject.Find("Terrain").GetComponent<Building>().closeClicked();
        buildButton.SetActive(false);
    }

    public void hideAllUI () {

        string[] hideable = {"BuildingMenu_Background", "MenuButton", "BuildNow", "RessourceDisplayLarge", "Resources", "BuildingPreview"};

        foreach (string elem in hideable) {
            try {
                GameObject.Find(elem).SetActive(false);
            } catch (NullReferenceException ex){}
        }
    }

    public void restoreDefaultUI() {
        hideAllUI();
        menuBut.SetActive(true);
        resourcesMenu.SetActive(true);
    }

    private void showMenu(bool visible) {
        buildingMenu.SetActive(visible);
        openBuildingMenu.SetActive(!visible);
    }

    public void buildNow() {
        handleAny();
        Debug.Log("building structure: ");
        GameObject.Find("Terrain").GetComponent<Building>().buildNow();
        buildButton.SetActive(false);
    }

    public void saveGame() {

        print("saving...");
        this.GetComponent<SaveLoad>().save(saveName);
    }

}
