using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoClicked : MonoBehaviour {

    public static GameObject script;
    public static BuildingManager.structureData currentData;

    public GameObject ClickableInfo;
    public GameObject uiCanvas;
    public GameObject menuButton;
    public GameObject buildingMenu;

    public static InfoClicked getInstance() {
        return script.GetComponent<InfoClicked>();
    }

    void Start () {
	    ClickableInfo = uiCanvas;
        if (this.gameObject.transform.parent.name.Equals("ClickableInfo")) {
            script = this.gameObject;
        }

        close();
	}

    public void setPrice(List<ressourceStack> cost) {
        displayPrice(BuildingManager.getNiceString(cost));

    }

    void FixedUpdate() {
        try {
            if (!transform.parent.Find("Cost").gameObject.activeSelf) {
                return;
            }
        } catch (NullReferenceException ex) {
            return;
        }

        foreach (var elem in currentData.cost) {
            
            setBuildable(true);
            if (elem.getAmount() > ResourceHandler.getAmoumt(elem.getRessource())) {
                setBuildable(false);
                break;
            }
        }
    }

	public void closeClicked() {
        Debug.Log("Closing Info Window");
        clickDetector.overlayClicked = true;
        close();
    }

    private void setBuildable(bool buildable) {
        //Debug.Log("setting build interactible: " + buildable + " price: " + prices[0] + " available: " + ResourceHandler.totalWood);
        this.transform.parent.Find("DoBuild").GetComponent<UnityEngine.UI.Button>().interactable = buildable;
    }

    public void setTitle(string text) {
        transform.parent.Find("Title").GetComponent<UnityEngine.UI.Text>().text = text;
    }

    public void setDesc(string desc) {
        transform.parent.Find("Description").GetComponent<UnityEngine.UI.Text>().text = desc;
    }

    private void displayPrice(string price) {
        transform.parent.Find("Cost").GetComponent<UnityEngine.UI.Text>().text = price;
    }

    private void close() {
        ClickableInfo.SetActive(false);
        if (!buildingMenu.activeSelf) {
            menuButton.SetActive(true);
        }
        
    }

    public void show() {
        ClickableInfo.SetActive(true);
        menuButton.SetActive(false);
        GameObject.Find("Terrain").GetComponent<Building>().closeClicked();

    }

    public void setData(BuildingManager.structureData data) {
        currentData = data;
    }

    public void handleBuild() {
        
        Debug.Log("Clicked build button on preview");
        clickDetector.overlayClicked = true;
        ClickableInfo.SetActive(false);
        
        GameObject.Find("Terrain").GetComponent<Building>().buildClicked(currentData);

    }
}
