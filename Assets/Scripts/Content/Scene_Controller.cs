using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Scene_Controller : MonoBehaviour {

    public GameObject buildingMenu;
    public GameObject buildButton;
    public GameObject buildPreview;
    public GameObject PipeMiddle;
    public static GameObject pipeMiddle;
    public static string saveName = "default";
    public GameObject menuBut;
    public GameObject resourcesMenu;
    public GameObject pickupBox;
    public GameObject harvestParticle;
    public GameObject destroyParticle;
    public GameObject conveyorConfigurator;

    private static Scene_Controller instance;
    
    void Awake() {
        pipeMiddle = PipeMiddle;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
        instance = this;
    }

    public static Scene_Controller getInstance() {
        return instance;
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

        string[] hideable = {"BuildingMenu_Background", "MenuButton", "BuildNow", "RessourceDisplayLarge", "Resources", "BuildingPreview", "BuildingMenu"};

        foreach (string elem in hideable) {
            try {
                GameObject.Find(elem).SetActive(false);
            } catch (NullReferenceException ex){}
        }
    }

    public void restoreDefaultUI() {
        hideAllUI();
        menuBut.SetActive(false);
        menuBut.SetActive(true);
        resourcesMenu.SetActive(true);
    }

    private void showMenu(bool visible) {
        buildingMenu.SetActive(visible);
        menuBut.SetActive(!visible);
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

    public void onPipingPressed() {
        handleAny();
        ConveyorHandler.getInstance().onConveyorButtonClicked();
    }

}
